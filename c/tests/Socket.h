/*
 * File....: Socket.h
 * Author..: Jose Ferreira
 * Date....: 2015-08-12 - 20:48
 * Purpose.: Network sockets
 */


#pragma once


#include "Ptr.h"
#include "Fd.h"


/* Connection state enumeration. */
enum ConnectionStateEnum {
	STATE_NOT_CONNECTED = 0,
	STATE_LISTENING     = 1,
	STATE_CONNECTING    = 2,
	STATE_DNS_ERROR     = 3,
	STATE_CONNECT_ERROR = 4,
	STATE_CONNECTED     = 5,
	STATE_DISCONNECTED  = 6,
};


/* Return a string representation of the connection state enumeration. */
const char* ConnectionStateStr(ConnectionStateEnum state);


class CSockAddr;


/* Provides automatic management of socket file descriptor including reference counting.
 * The descriptor is automatic closed when needed. */
class CSocketHandle : CRefPtr
{
private:
	virtual void ReleaseData(void *);
	void ReleaseFd(int fd);

public:
	/* Cached read-only copy of the managed file descriptor. */
	int  Fd;

	// Default empty constructor.
	CSocketHandle();

	// Constructor for automatic management of the fd descriptor.
	CSocketHandle(int fd);

	// Automatic return Fd on conversion to int.
	operator int() const { return Fd; }

	// Default destructor. Releases the fd.
	~CSocketHandle();

	// Release the managed fd, closing the fd if the reference count reaches 0.
	void Close();

	// Release the current fd and begin management for the new fd.
	void Set(int fd);

	void Debug();
};


/* Represents a network socket descriptor. */
class CSocket
{
protected:
	// Internal file descriptor.
	CSocketHandle m_fd;

public:
	// Last error code, or RET_OK in case of no errors.
	int LastErr;

	// String message for the last error code.
	const char * GetLastErrMsg() const;

	// Default constructor for a closed socket.
	CSocket();
	
	// Constructor for the socket informed.
	explicit CSocket(int fd);

	/* Release the current fd and creates a
	 * new socket with the family and types informed.
	 *  family can be: AF_INET or AF_INET6.
	 *  type can be: SOCK_STREAM or SOCK_DGRAM.
	 * Return true/false if created the socket with success.*/
	bool Create(int family, int type);
	
	/* Binds the current socket to the address informed.
	 * Returns true/false if success.*/
	bool Bind(const CSockAddr& sockAddr);
	
	/* Puts the socket in listening state.
	 * Returns true/false if success.*/
	bool Listen(int backlog);
	
	/* Accept one pending client connection.
	 * Return the socket fd for the new client,
	 * or RET_ERR in case of error. */
	int Accept();

	/* Accept one pending client connection.
	 * Fills the SockAddr with the addres of the remote point.
	 * Return the socket fd for the new client,
	 * or RET_ERR in case of error. */
	int Accept(CSockAddr& address);

	int Connect(const CSockAddr& address);

	int Write(const void * buffer, int size);
	int Send(const void * buffer, int size, int flags);
	int SendTo(const void * buffer, int size, int flags, const CSockAddr& address);

	int Read(void * buffer, int size);
	int Recv(void * buffer, int size, int flags);
	int RecvFrom(void * buffer, int size, int flags, CSockAddr& address);
	
	/* Reads the error code from kernel socket state. */
	int GetSockError() const;
	
	/* Enable/disable UDP broadcast state for the socket.
	 * Returns true/false if success in changing state. */
	int  SetBroadcast(int enabled);

	/* Release the current fd and
	 * sets the new fd for the instance. */
	void SetFd(int fd);

	// Returns the current file descriptor.
	int  GetFd() const;

	/* Release the current open file descriptor.
	 * Closes it if it's the last reference.
	 * Keeps LastErr state. */
	void Close();

	/* Release the current open file descriptor.
	 * Closes it if it's the last reference.
	 * Clear LastErr state to RET_OK. */
	void Clear();

	/* Returns true/false if the fd is readable (i.e. can read data without blocking). */
	bool IsReadable() const;

	/* Returns true/false if the fd is writeable (i.e. can write data without blocking). */
	bool IsWritable() const;

	/* Returns true/false if the fd is closed. */
	bool IsClosed() const;

	/* Returns the number of bytes immediately available for reading. */
	int  BytesAvailable() const;

	/* Enable/disable non-blocking state for the fd.
	 * Returns true/false if success in changing state. */
	bool SetNonBlock(int enabled);

	/* Reads the error code from kernel socket state. */
 	static int GetSockError(int sockdfd);
};


class NetworkStream : public CStream
{
protected:
	CSocket m_socket;
	bool m_Eof = false;
	bool m_Error = false;

public:
	NetworkStream();
	NetworkStream(const CSocket& socket);

	int Write(uint8_t t) override;
	int Write(const void * buffer, int size) override;
	int ReadByte() override;
	int Read(void * buffer, int size) override;

	bool IsEof();
	bool IsError();
	int GetLastErr();
	const char * GetLastErrMsg();
};
