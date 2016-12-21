/*
 * File....: TcpClient.h
 * Author..: Jose Ferreira
 * Date....: 2015-08-12 - 20:48
 * Purpose.: Network client connection.
 */


#pragma once


#include "Util.h"
#include "Stream.h"
#include "Socket.h"
#include "IPAddress.h"


/* Represents a TCP network client connection. */
class CTcpClient
{
private:
	/* Finalize the current connection setting the new state and errorCode. */
	void InternalClose(ConnectionStateEnum newState, int errorCode);
	
	// int  ResolveName(const char *hostName, int port, void *targetAddress, int *targetAddressSize);
	
protected:
	// Data stream
	CFdStream m_stream;
	
	// Connection socket fd
	CSocket   m_sock;
	
	// Connection state
	ConnectionStateEnum m_state;
	
public:
	/* Process/update and returns the current connection state. */
	ConnectionStateEnum State();

	// Default constructor for a closed connection.
	CTcpClient();
	
	// Constructor for an open connection on the fd.
	explicit CTcpClient(int clientfd);
	
	/* Sets the connection fd. */
	void SetFd(int fd);
	
	/* Returns the connection fd. */
	int  GetFd() const;
	
	/* Close the connection. */
	void Close();
	
	/* Try to connect to the host name and port.
	 * Resolves the host name via DNS if necessary.
	 * Returns true/false if connected. */
	bool Connect(const char * hostName, int port);
	
	/* Try to connect to the endpoint.
	 * Returns true/false if connected. */
	bool Connect(const CIPEndPoint& remoteEP);

	/* Returns true/false if the socket is connected or connecting. */
	bool IsConnected();
	
	/* Returns true/false if the socket is connected to the remote endpoint. */
	bool IsReady();
	
	/* Last error code, or RET_OK if no error. */
	int  GetLastErr();
	
	/* String message for the last error code. */
	cstr GetLastErrMsg();
	
	// int  Write(uint8_t c);
	// int  Write(uint8_t *ptr, int size);
	// int  Available();
	// int  Read();
	// int  Peek();
	// void Flush();

	CStream* GetStream();
};
