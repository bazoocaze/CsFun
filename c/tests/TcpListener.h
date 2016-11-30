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
class CTcpListener
{
private:
	void LogErr(const char *msg);
	bool InternalAccept(CTcpClient &cliente);
	
protected:
	// Listener was stopped.
	bool m_stopped;

	// Listener socket.
	CSocket m_sock;
	
	// Listening endpoint.
	CIPEndPoint m_endpoint;
	
public:
	// Default constructor.
	CTcpListener();
	
	/* Default destructor.
	 * Closes the listener on destruction. */
	~CTcpListener();

	/* Returns the socket fd for the listener. */
	int  GetFd() const;

	/* Bind on the TCP port and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(int port);
	
	/* Bind on the IP address and TCP port and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(const CIPAddress & addr, int port);
	
	/* Bind on the endpoint and starts listening for connections.
	 * Return true/false on success.*/
	bool Start(const CIPEndPoint & localEP);

	/* Stop the listener. */
	void Stop();

	/* Returns true/false if the server is listening for connections. */
	bool IsListening() const;

	/* Returns true/false if the server has an connection pending for Accept(). */
	bool Available() const;

	/* Waits the timeout for a connection availability.
	 * Returns true/false if the server has an connection pending for Accept(). */
	bool WaitAvailable(int timeoutMillis) const;

	/* Accepts a pending connection.
	 * Puts the accepted connection on the TcpClient.
	 * If no connection is pending, then awaits for a new connection and accepts it.
	 * Returns true/false on success accepting. */
	bool Accept(CTcpClient & client);

	/* Try to accept a pending connection.
	 * Puts the accepted connection on the TcpClient.
	 * If no connection is pending, then awaits the timeout
	 * periodo for a new connection and accepts it.
	 * Returns true/false on success accepting. */
	bool TryAccept(CTcpClient & client, int timeoutMillis);
	
	/* Accepts a pending connection.
	 * Puts the accepted connection on the TcpClient.
	 * If no connection is pending, return immediately.
	 * Returns true/false on success accepting. */
	bool AcceptNonBlock(CTcpClient & client);

	/* Last error code, or RET_OK if no error. */
	int GetLastErr() const;
	
	/* Message string for the last error code. */
	cstr GetLastErrMsg() const;
};
