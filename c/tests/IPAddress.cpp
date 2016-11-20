/*
 * File....: IPAddress.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-16 - 16:42
 * Purpose.: IPAdress helper classes.
 */
 
#include <stdlib.h>
#include <string.h>
#include <arpa/inet.h>

#include "IPAddress.h"
#include "Print.h"
#include "Util.h"
#include "Logger.h"


//////////////////////////////////////////////////////////////////////
// SockAddr
//////////////////////////////////////////////////////////////////////


SockAddr::SockAddr()
{
	m_size = 0;
}

SockAddr::SockAddr(sockaddr *sa, int size)
{
	SetAddress(sa, size);
}

int SockAddr::GetFamily() const
{
	return m_addr.sa.sa_family;
}

void SockAddr::SetAddress(sockaddr *sa, int size)
{
	int f = sa->sa_family;
	if(size > sizeof(m_addr)) return;
	if(f != AF_INET && f != AF_INET6) return;
	if(f == AF_INET && size != sizeof(m_addr.in4)) return;
	if(f == AF_INET6 && size != sizeof(m_addr.in6)) return;

	m_size = size;
	memcpy(&m_addr, sa, size);
}

void SockAddr::SetPort(int port)
{
	int f = GetFamily();
	if(f == AF_INET)
		m_addr.in4.sin_port = htons(port);
	else if(f = AF_INET6)
		m_addr.in6.sin6_port = htons(port);
}

int SockAddr::GetPort() const
{
	int f = GetFamily();
	if(f == AF_INET)
		return ntohs(m_addr.in4.sin_port);
	else if(f = AF_INET6)
		return ntohs(m_addr.in6.sin6_port);
	return 0;
}

const sockaddr * SockAddr::GetSockAddr() const
{
	return &(m_addr.sa);
}

int SockAddr::GetSize() const
{
	return m_size;
}

bool SockAddr::IsValid() const
{
	int f = GetFamily();
	if(f != AF_INET && f != AF_INET6) return false;
	if(f == AF_INET  && m_size != sizeof(m_addr.in4)) return false;
	if(f == AF_INET6 && m_size != sizeof(m_addr.in6)) return false;
	return true;
}


//////////////////////////////////////////////////////////////////////
// IPAddress
//////////////////////////////////////////////////////////////////////


IPAddress::IPAddress()
{
}

IPAddress::IPAddress(in_addr_t addr)
{
	sockaddr_in s;
	s.sin_family = AF_INET;
	s.sin_addr.s_addr = addr;
	m_sockaddr.SetAddress((sockaddr*)&s, sizeof(s));
}

IPAddress::IPAddress(const SockAddr& addr)
{
	m_sockaddr = addr;
}

IPAddress::IPAddress(const char *addr)
{
int ret;
	// ipv4
	sockaddr_in s4;
	s4.sin_family = AF_INET;
	ret = inet_aton(addr, &s4.sin_addr);
	if(ret != 0)
	{
		m_sockaddr.SetAddress((sockaddr*)&s4, sizeof(s4));
		return;
	}
	// ipv6
	sockaddr_in6 s6;
	s6.sin6_family = AF_INET6;
	ret = inet_pton(AF_INET6, addr, &s6.sin6_addr);
	if(ret == 0) s6.sin6_family = 0;
	m_sockaddr.SetAddress((sockaddr*)&s6, sizeof(s6));
}

const SockAddr IPAddress::GetSockAddr() const
{
	return m_sockaddr;
}

size_t IPAddress::printTo(Print &p) const
{
int f = m_sockaddr.GetFamily();
char dst[MAX(INET_ADDRSTRLEN, INET6_ADDRSTRLEN)];
cstr ret;
	if(!m_sockaddr.IsValid())
		return p.print("<invalid ip address>");
	const sockaddr     * sa = m_sockaddr.GetSockAddr();
	const sockaddr_in  * in4 = (sockaddr_in *)sa;
	const sockaddr_in6 * in6 = (sockaddr_in6 *)sa;
	if(f == AF_INET)
	{
		ret = inet_ntop(f, &in4->sin_addr, dst, sizeof(dst));
	}
	else
	{
		ret = inet_ntop(f, &in6->sin6_addr, dst, sizeof(dst));
	}
	if(ret == NULL) return p.print("<ip address error>");
	return p.print(dst);
}

IPAddress IPAddress::Any          = IPAddress(htonl(INADDR_ANY));
IPAddress IPAddress::Loopback     = IPAddress(htonl(INADDR_LOOPBACK));
IPAddress IPAddress::IPv6Loopback = IPAddress("::1");



//////////////////////////////////////////////////////////////////////
// IPEndPoint
//////////////////////////////////////////////////////////////////////



IPEndPoint::IPEndPoint()
{
}

IPEndPoint::IPEndPoint(const SockAddr& addr)
{
	m_sockaddr = addr;
}

IPEndPoint::IPEndPoint(const IPAddress& address, int port)
{
	m_sockaddr = address.GetSockAddr();
	m_sockaddr.SetPort(port);
}

IPEndPoint::IPEndPoint(const char *addr, int port)
{
int ret;
   sockaddr_in s;
   s.sin_family = AF_INET;
	s.sin_port = htons(port);
   ret = inet_aton(addr, &s.sin_addr);
   if(ret==0) s.sin_family = 0;
   m_sockaddr.SetAddress((sockaddr*)&s, sizeof(s));
}

void IPEndPoint::SetPort(int port)
{
	m_sockaddr.SetPort(port);
}

int IPEndPoint::GetPort() const
{
	return m_sockaddr.GetPort();
}

void IPEndPoint::SetAddress(const IPAddress& address)
{
	int port = m_sockaddr.GetPort();
	m_sockaddr = address.GetSockAddr();
	m_sockaddr.SetPort(port);
}

IPAddress IPEndPoint::GetAddress() const
{
	return IPAddress(m_sockaddr);
}

size_t IPEndPoint::printTo(Print &p) const
{
int ret = 0;
IPAddress ipAddress;
   if(!m_sockaddr.IsValid())
      return p.print("<invalid ip endpoint>");
	ipAddress = IPAddress(m_sockaddr);
	ret += p.print(ipAddress);
	ret += p.print(":");
	ret += p.print(m_sockaddr.GetPort());
	return ret;
}

bool IPEndPoint::IsValid() const
{
	return m_sockaddr.IsValid();
}

SockAddr IPEndPoint::GetSockAddr() const
{
	return m_sockaddr;
}


//////////////////////////////////////////////////////////////////////
// IPAddressList
//////////////////////////////////////////////////////////////////////


IPAddressList::IPAddressList()
{
	Items = NULL;
	Count = 0;
}


void IPAddressList::Add(const IPAddress& address)
{
size_t size;
void *newptr;
	size = (Count + 1)*sizeof(IPAddress);
	if(!m_Items.Realloc(size))
	{
		OutOffMemoryHandler("IPAddressList", "realloc", size);
		return;
	}
	Items = (IPAddress*)m_Items.Get();
	Items[Count] = address;
	Count++;
}

