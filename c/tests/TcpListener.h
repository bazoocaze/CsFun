/*
 * File....: TcpListener.h
 * Author..: Jose Ferreira
 * Date....: 2015-08-12 - 20:52
 * Purpose.: TCP connection server.
 */


#pragma once


#include "TcpClient.h"
#include "IPAddress.h"
#include "Socket.h"


/* Represents a TCP connection server. */
class TcpListener
{
private:
	void LogErr(const char *msg);
	bool InternalAccept(TcpClient &cliente);
	
protected:
	// Listener socket.
	Socket m_sock;
	
	// Listening endpoint.
	IPEndPoint m_endpoint;
	
public:
	// Default constructor.
	TcpListener();
	
	/* Default destructor.
	 * Closes the listener on destruction. */
	~TcpListener();

	/* Returns the socket fd for the listener. */
	int  GetFd() const;

	/* Bind on the TCP port and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(int port);
	
	/* Bind on the IP address and TCP port and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(const IPAddress & addr, int port);
	
	/* Bind on the endpoint and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(const IPEndPoint & localEP);

	/* Stop the listener. */
	void Stop();

	/* Returns true/false if the server is listening for connections. */
	bool IsListening() const;

	/* Returns true/false if the server has an connection pending for Accept(). */
	bool Available() const;

	/* Accepts a pending connection.
	 * Puts the accepted connection on the TcpClient.
	 * If no connection is pending, then awaits for a new connection and accepts it.
	 * Returns true/false on success accepting. */
	bool Accept(TcpClient & client);
	
	/* Accepts a pending connection.
	 * Puts the accepted connection on the TcpClient.
	 * If no connection is pending, return immediately.
	 * Returns true/false on success accepting. */
	bool AcceptNonBlock(TcpClient & client);

	/* Last error code, or RET_OK if no error. */
	int GetLastErr() const;
	
	/* Message string for the last error code. */
	cstr GetLastErrMsg() const;
};
