/*
 * Arquivo....: TcpClient.cpp
 * Autor......: José Ferreira - olvebra
 * Data.......: 12/08/2015 - 20:48
 * Objetivo...: Conexao de rede cliente.
 */


#pragma once
 
 
#include "IPAddress.h"


class Dns
{
public:
	static int  LastError;
	static cstr GetMsgError();
	static void LogErr(cstr msg);

	static IPAddressList GetHostAddresses(const char * hostName);
	static IPAddressList GetHostAddresses(const char * hostName, bool ipv4, bool ipv6);
};

