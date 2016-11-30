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
#include "Stream.h"
#include "Util.h"
#include "Logger.h"
#include "Config.h"



//////////////////////////////////////////////////////////////////////
// SockAddr
//////////////////////////////////////////////////////////////////////



CSockAddr::CSockAddr()
{
	m_size = 0;
}

CSockAddr::CSockAddr(sockaddr *sa, int size)
{
	SetAddress(sa, size);
}

int CSockAddr::GetFamily() const
{
	return m_addr.sa.sa_family;
}

void CSockAddr::SetAddress(sockaddr *sa, int size)
{
	int f = sa->sa_family;

	if(size > sizeof(m_addr))
		return;

	if(f != AF_INET && f != AF_INET6)
		return;

	if(f == AF_INET && size != sizeof(m_addr.in4))
		return;

	if(f == AF_INET6 && size != sizeof(m_addr.in6))
		return;

	m_size = size;
	memcpy(&m_addr, sa, size);
}

void CSockAddr::SetPort(int port)
{
	int f = GetFamily();

	if(f == AF_INET)
	{
		m_addr.in4.sin_port = htons(port);
	}
	else if(f == AF_INET6)
	{
		m_addr.in6.sin6_port = htons(port);
	}
}

int CSockAddr::GetPort() const
{
	int f = GetFamily();

	if(f == AF_INET)
		return ntohs(m_addr.in4.sin_port);
	else if(f == AF_INET6)
		return ntohs(m_addr.in6.sin6_port);

	return 0;
}

sockaddr * CSockAddr::GetSockAddr() const
{
	return (sockaddr*)&(m_addr.sa);
}

int CSockAddr::GetSize() const
{
	return m_size;
}

int CSockAddr::GetMaxSize() const
{
	return sizeof(m_addr);
}

void CSockAddr::SetSize(int size)
{
	m_size = size;
}

bool CSockAddr::IsValid() const
{
	int f = GetFamily();

	if(f != AF_INET && f != AF_INET6)
		return false;

	if(f == AF_INET  && m_size != sizeof(m_addr.in4))
		return false;

	if(f == AF_INET6 && m_size != sizeof(m_addr.in6))
		return false;

	return true;
}


//////////////////////////////////////////////////////////////////////
// IPAddress
//////////////////////////////////////////////////////////////////////


CIPAddress::CIPAddress()
{
}

CIPAddress::CIPAddress(in_addr_t addr)
{
	sockaddr_in s;
	s.sin_family = AF_INET;
	s.sin_addr.s_addr = addr;
	m_sockaddr.SetAddress((sockaddr*)&s, sizeof(s));
}

CIPAddress::CIPAddress(const CSockAddr& addr)
{
	m_sockaddr = addr;
}

CIPAddress::CIPAddress(const char *addr)
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
	if(ret == 0)
		s6.sin6_family = 0;

	m_sockaddr.SetAddress((sockaddr*)&s6, sizeof(s6));
}

const CSockAddr CIPAddress::GetSockAddr() const
{
	return m_sockaddr;
}

int CIPAddress::printTo(CTextWriter &p) const
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

	if(ret == NULL)
		return p.print("<ip address error>");

	return p.print(dst);
}

CIPAddress CIPAddress::Any          = CIPAddress(htonl(INADDR_ANY));
CIPAddress CIPAddress::Loopback     = CIPAddress(htonl(INADDR_LOOPBACK));
CIPAddress CIPAddress::IPv6Loopback = CIPAddress("::1");



//////////////////////////////////////////////////////////////////////
// IPEndPoint
//////////////////////////////////////////////////////////////////////



CIPEndPoint::CIPEndPoint()
{
}

CIPEndPoint::CIPEndPoint(const CSockAddr& addr)
{
	m_sockaddr = addr;
}

CIPEndPoint::CIPEndPoint(const CIPAddress& address, int port)
{
	m_sockaddr = address.GetSockAddr();
	m_sockaddr.SetPort(port);
}

CIPEndPoint::CIPEndPoint(const char *addr, int port)
{
int ret;
	sockaddr_in s;
	s.sin_family = AF_INET;
	s.sin_port = htons(port);

	// interprets addr as dotted IPv4
	ret = inet_aton(addr, &s.sin_addr);

	if(ret==0)
		s.sin_family = 0;

	m_sockaddr.SetAddress((sockaddr*)&s, sizeof(s));
}

void CIPEndPoint::SetPort(int port)
{
	m_sockaddr.SetPort(port);
}

int CIPEndPoint::GetPort() const
{
	return m_sockaddr.GetPort();
}

void CIPEndPoint::SetAddress(const CIPAddress& address)
{
	int port = m_sockaddr.GetPort();
	m_sockaddr = address.GetSockAddr();
	m_sockaddr.SetPort(port);
}

CIPAddress CIPEndPoint::GetAddress() const
{
	return CIPAddress(m_sockaddr);
}

int CIPEndPoint::printTo(CTextWriter &p) const
{
int ret = 0;
CIPAddress ipAddress;

	if(!m_sockaddr.IsValid())
		return p.print("<invalid ip endpoint>");

	ipAddress = CIPAddress(m_sockaddr);
	ret += p.print(ipAddress);
	ret += p.print(":");
	ret += p.print(m_sockaddr.GetPort());
	return ret;
}

bool CIPEndPoint::IsValid() const
{
	return m_sockaddr.IsValid();
}

CSockAddr CIPEndPoint::GetSockAddr() const
{
	return m_sockaddr;
}


//////////////////////////////////////////////////////////////////////
// IPAddressList
//////////////////////////////////////////////////////////////////////


CIPAddressList::CIPAddressList()
{
	Items = NULL;
	Count = 0;
}


void CIPAddressList::Add(const CIPAddress& address)
{
	if(Count >= P_DNS_MAX_IPS)
		return;

	size_t size;
	size = (Count + 1)*sizeof(CIPAddress);

	if(!m_Items.Resize(size))
	{
		OutOffMemoryHandler("IPAddressList", "Add", size, true);
		return;
	}

	Items = (CIPAddress*)m_Items.Get();
	Items[Count] = address;
	Count++;
}
