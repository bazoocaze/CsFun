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


class SockAddr;


/* Represents a network socket descriptor. */
class Socket : public Fd
{
public:
	// Default constructor for a closed socket.
	Socket();
	
	// Constructor for the socket informed.
	Socket(int fd);

	/* Release the current fd and creates a
	 * new socket with the family and types informed.
	 *  family can be: AF_INET or AF_INET6.
	 *  type can be: SOCK_STREAM or SOCK_DGRAM.
	 * Return true/false if created the socket with success.*/
	bool Create(int family, int type);
	
	/* Binds the current socket to the address informed.
	 * Returns true/false if success.*/
	bool Bind(const SockAddr& sockAddr);
	
	/* Puts the socket in listening state.
	 * Returns true/false if success.*/
	bool Listen(int backlog);
	
	/* Accept one pending client connection.
	 * Return the socket fd for the new client,
	 * or RET_ERR in case of error. */
	int  Accept();
	
	/* Reads the error code from kernel socket state. */
	int  GetSockError() const;
	
	/* Enable/disable UDP broadcast state for the socket.
	 * Returns true/false if success in changing state. */
	int  SetBroadcast(int enabled);

	/* Reads the error code from kernel socket state. */
 	static int  GetSockError(int sockdfd);
};
