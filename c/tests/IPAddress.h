/*
 * File....: IPAddress.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-16 - 16:42
 * Purpose.: IPAdress helper classes.
 */


#pragma once


#include <arpa/inet.h>
#include "Ptr.h"
#include "Text.h"


// Abstraction of the sockaddr structure around the IPv4 and IPv6 variants.
class CSockAddr
{
private:
	union {
		sockaddr     sa;
		sockaddr_in  in4;
		sockaddr_in6 in6;
	}   m_addr;
	
	// Size of the *m_addr* structure.
	int m_size;

public:
	// Default constructor for an empty address and port.
	CSockAddr();
	
	// Constructor for an existing sockaddr structure.
	CSockAddr(sockaddr *sa, int size);
	
	// Sets the instance address as sockaddr informed.
	void SetAddress(sockaddr *sa, int size);
	
	// Sets the TCP port number on the sockaddr structure to the port informed.
	void SetPort(int port);
	
	/* Returns the protocol family for the instance address.
	 * Can be: AF_INET for IPv4, ou AF_INET6 for IPv6. */	
	int  GetFamily() const;
	
	// Returns the TCP port number on the sockaddr.
	int  GetPort()   const;
	
	// Returns true/false if the current address is valid.
	bool IsValid()   const;
	
	// Returns true/false if the current address is an IPv4 address.
	bool IsIPv4()    const;
	
	// Returns true/false if the current address is an IPv6 address.
	bool IsIPv6()    const;

	/* Returns a pointer to the internal sockaddr structure.
	 * The pointer is valid until the next change on the instance. */
	sockaddr* GetSockAddr() const;
	
	/* Sets the size of the internal sockaddr. For use with accept(). */
	void SetSize(int size);

	// Returns the size of the internal sockaddr.
	int GetSize()     const;

	/* Returns the max size of the internal sockaddr. For use with accept(). */
	int GetMaxSize()  const;
};


// Represents and IP address (both IPv4 or IPv6)
class CIPAddress : public CPrintable
{
private:
	// sockaddr structure holding the address/port
	CSockAddr m_sockaddr;

public:
	// Default constructor for an empty/not informed/invalid address
	CIPAddress();
	
	/* Constructor for the INADDR_* IPv4 address.
	 * Common values: INADDR_ANY, INADDR_LOOPBACK, INADDR_BROADCAST. */
	explicit CIPAddress(in_addr_t addr);
	
	// Constructor using the SockAddr informed.
	explicit CIPAddress(const CSockAddr& addr);
	
	/* Constructor using the textual representantion of the ip address (IPv4 ou IPv6).
	 * No name resolution is done on the textual representantion. */
	explicit CIPAddress(const char *addr);
	
	// Returns the SockAddr for the current address;
	const CSockAddr GetSockAddr() const;

	// IPv4 ip address for any target (0.0.0.0)
	static CIPAddress Any;
	
	// IPv4 ip address for loopback target (127.0.0.1)
	static CIPAddress Loopback;
	
	// IPv6 ip address for loopback (::1)
	static CIPAddress IPv6Loopback;

	// Writes the string representation of the address to the target writer.
	int printTo(CTextWriter &p) const;
};


// Represents an IP addres and TCP port number endpoint pair.
class CIPEndPoint : public CPrintable
{
private:
	// sockaddr structure holding the address/port
	CSockAddr m_sockaddr;

public:
	// Default constructor for an empty endpoint (no addres and port).
	CIPEndPoint();
	
	// Constructor using the SockAddr informed.
	explicit CIPEndPoint(const CSockAddr& addr);
	
	// Constructor using the address and tcp port.
	CIPEndPoint(const CIPAddress& address, int port);
	
	/* Constructor using the textual representation of the IPv4 (no IPv6 support)
	 * and the TCP port informed. */
	CIPEndPoint(const char *addr, int port);
	
	// Returns true/false if the endpoint is valid.
	bool      IsValid() const;
	
	// Sets the TCP port portion of the endpoint.
	void      SetPort(int port);
	
	// Gets the TCP port of the endpoint.
	int       GetPort() const;
	
	// Sets the address portion of the endpoint.
	void      SetAddress(const CIPAddress& address);
	
	// Gets the address of the endpoint.
	CIPAddress GetAddress() const;
	
	// Gets the SockAddr of the endpoint.
	CSockAddr  GetSockAddr() const;

	// Writes the string representation of the endpoint to the target writer.
	int printTo(CTextWriter &p) const;
};


// Represents a list of IP addresses.
class CIPAddressList
{
protected:
	// Pointer for the dynamic memory holding the list.
	CMemPtr m_Items;

public:
	// Number of elements on the list.
	int Count;
	
	// List of addresses.
	CIPAddress* Items;

	// Default constructor for an empty list.
	CIPAddressList();

	// Adds an address to the end of the list.
	void Add(const CIPAddress& address);
};
