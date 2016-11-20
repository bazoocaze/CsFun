/*
 * Arquivo....: TcpClient.cpp
 * Autor......: José Ferreira - olvebra
 * Data.......: 12/08/2015 - 20:48
 * Objetivo...: Conexao de rede cliente.
 */
 
 
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>
#include <sys/time.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <errno.h>
#include <arpa/inet.h>

// #include "comum.h"
#include "TcpClient.h"
#include "Config.h"
#include "Logger.h"
#include "Dns.h"
#include "IPAddress.h"


int Dns::LastError = 0;



cstr Dns::GetMsgError()
{
	return gai_strerror(Dns::LastError);
}


void Dns::LogErr(cstr msg)
{
	printf("DnsError: %s - err %d - %s\n", msg, Dns::LastError, GetMsgError());
}


IPAddressList Dns::GetHostAddresses(const char * hostName)
{
	return GetHostAddresses(hostName, true, true);
}


IPAddressList Dns::GetHostAddresses(const char * hostName, bool ipv4, bool ipv6)
{
int ret;

	IPAddressList retList;

	if(!(ipv4 || ipv6)) return retList;

   if(true) {
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
         Dns::LastError = ret;
         return retList;
      }

      /* Para cada ip/dns encontrado */
      for (rp = dnsResult; rp != NULL; rp = rp->ai_next) {
			SockAddr sockAddr = SockAddr(rp->ai_addr, rp->ai_addrlen);
			IPAddress addr = IPAddress(sockAddr);
			retList.Add(addr);
      }

      /* libera os recursos da pesquisa DNS */
      freeaddrinfo(dnsResult);
	} else {
		IPAddress addr = IPAddress(hostName);
		if(addr.GetSockAddr().IsValid())
		{
			retList.Add(addr);
		}
		else
		{
			Dns::LastError = -1;
		}
	}

	Dns::LastError = RET_OK;
	return retList;
}

