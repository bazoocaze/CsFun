/*
 * File......: Dns.h
 * Author....: Jose Ferreira
 * Date......: 2015-08-12 - 20:48
 * Purpose...: Dns name resolution services.
 */


#pragma once
 
 
#include "IPAddress.h"


// Dns service tools.
class CDns
{
protected:
	static void LogErr(cstr msg, cstr extra);

public:
	// Last dns error code.
	static int LastErr;
	
	// String message for the last error code.
	static cstr GetLastErrMsg();

	/* Return a list of resolved IP addresses for the *hostName*, 
	 * including IPv4 and IPv6 addresses.*/
	static CIPAddressList GetHostAddresses(const char * hostName);
	
	/* Return a list of resolved IP addresses for the *hostName*.
	 * You can select if you want IPv4 and/or IPv6 addresses on the response.*/
	static CIPAddressList GetHostAddresses(const char * hostName, bool ipv4, bool ipv6);
};
