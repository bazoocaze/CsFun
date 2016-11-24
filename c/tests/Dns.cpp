/*
 * File....: Dns.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-16 - 20:47
 * Purpose.: Dns util.
 */
 

#include <string.h> 
#include <netdb.h>

#include "Dns.h"
#include "Config.h"
#include "Logger.h"
#include "IPAddress.h"



int Dns::LastErr = RET_OK;



cstr Dns::GetLastErrMsg()
{
	return gai_strerror(Dns::LastErr);
}


void Dns::LogErr(cstr msg, cstr extra)
{
	Logger::LogMsg(
		LEVEL_WARN, 
		"Dns: %s-%s / Error [0x%x]:%s\n",
		msg,
		extra,
		LastErr,
		GetLastErrMsg());
}


IPAddressList Dns::GetHostAddresses(cstr hostName)
{
	return GetHostAddresses(hostName, true, true);
}


IPAddressList Dns::GetHostAddresses(cstr hostName, bool ipv4, bool ipv6)
{
int ret;

	IPAddressList retList;

	if(!(ipv4 || ipv6)) return retList;

#if defined HAVE_DNS_getaddrinfo

	/* resolve o nome do servidor usando DNS */

	struct addrinfo *dnsResult, *rp;
	struct addrinfo hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_family = AF_INET;

	if(ipv4 && ipv6)
		hints.ai_family = AF_UNSPEC;
	else if(ipv6)
		hints.ai_family = AF_INET6;

	ret = getaddrinfo(hostName, NULL, &hints, &dnsResult);
	if (ret != RET_OK) {
		Dns::LastErr = ret;
		return retList;
	}

	/* For each ip/dns found */
	for (rp = dnsResult; rp != NULL; rp = rp->ai_next) {
		SockAddr sockAddr = SockAddr(rp->ai_addr, rp->ai_addrlen);
		IPAddress addr = IPAddress(sockAddr);
		retList.Add(addr);
	}

	Dns::LastErr = RET_OK;

	/* Free the DNS search resources */
	freeaddrinfo(dnsResult);

#else

	IPAddress addr = IPAddress(hostName);
	if(addr.GetSockAddr().IsValid())
	{
		retList.Add(addr);
		Dns::LastErr = RET_OK;
	}
	else
	{
		Dns::LastErr = -1;
	}

#endif

	return retList;
}
