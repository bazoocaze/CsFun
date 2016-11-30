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



int CDns::LastErr = RET_OK;



cstr CDns::GetLastErrMsg()
{
	return gai_strerror(CDns::LastErr);
}


void CDns::LogErr(cstr msg, cstr extra)
{
	CLogger::LogMsg(
		LEVEL_WARN, 
		"Dns: %s-%s / Error [0x%x]:%s\n",
		msg,
		extra,
		LastErr,
		GetLastErrMsg());
}


CIPAddressList CDns::GetHostAddresses(cstr hostName)
{
	return GetHostAddresses(hostName, true, true);
}


CIPAddressList CDns::GetHostAddresses(cstr hostName, bool ipv4, bool ipv6)
{
int ret;

	CIPAddressList retList;

	if(!(ipv4 || ipv6))
		return retList;

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
		CDns::LastErr = ret;
		return retList;
	}

	/* For each ip/dns found */
	for (rp = dnsResult; rp != NULL; rp = rp->ai_next) {
		CSockAddr sockAddr = CSockAddr(rp->ai_addr, rp->ai_addrlen);
		CIPAddress addr = CIPAddress(sockAddr);
		retList.Add(addr);
	}

	CDns::LastErr = RET_OK;

	/* Free the DNS search resources */
	freeaddrinfo(dnsResult);

#else

	CIPAddress addr = CIPAddress(hostName);
	if(addr.GetSockAddr().IsValid())
	{
		retList.Add(addr);
		CDns::LastErr = RET_OK;
	}
	else
	{
		CDns::LastErr = -1;
	}

#endif

	return retList;
}
