/*
 * File....: IPAddress.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-16 - 16:42
 * Purpose.: IPAdress helper classes.
 */


#pragma once


#include <arpa/inet.h>
#include "Printable.h"
#include "Ptr.h"


class SockAddr
{
private:
	union {
		sockaddr     sa;
		sockaddr_in  in4;
		sockaddr_in6 in6;
	}   m_addr;
	int m_size;

public:
	SockAddr();
	SockAddr(sockaddr *sa, int size);
	void SetAddress(sockaddr *sa, int size);
	void SetPort(int port);
	int  GetFamily() const;
	int  GetPort()   const;
	bool IsValid()   const;
	bool IsIPv4()    const;
	bool IsIPv6()    const;

	const sockaddr* GetSockAddr() const;
	int             GetSize()     const;
};



class IPAddress : public Printable
{
private:
	SockAddr m_sockaddr;

public:
	IPAddress();
	IPAddress(in_addr_t addr);
	IPAddress(const SockAddr& addr);
	IPAddress(const char *addr);
	const SockAddr GetSockAddr() const;

	static IPAddress Any;
	static IPAddress Loopback;
	static IPAddress IPv6Loopback;

	size_t printTo(Print &p) const;
};



class IPEndPoint : public Printable
{
private:
	SockAddr m_sockaddr;

public:
	IPEndPoint();
	IPEndPoint(const SockAddr& addr);
	IPEndPoint(const IPAddress& address, int port);
	IPEndPoint(const char *addr, int port);
	bool      IsValid() const;
	void      SetPort(int port);
	int       GetPort() const;
	void      SetAddress(const IPAddress& address);
	IPAddress GetAddress() const;
	SockAddr  GetSockAddr() const;

	size_t printTo(Print &p) const;
};


class IPAddressList
{
protected:
	MemPtr m_Items;

public:
	int Count;
	IPAddress* Items;

	IPAddressList();

	void Add(const IPAddress& address);	
};

