/*
 * Arquivo....: TcpClient.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 12/08/2015 - 20:48
 * Objetivo...: Conexao de rede cliente.
 */


#pragma once

#include "Ptr.h"


enum ConnectionStateEnum {
	STATE_NOT_CONNECTED = 0,
	STATE_LISTENING     = 1,
	STATE_CONNECTING    = 2,
	STATE_DNS_ERROR     = 3,
	STATE_CONNECT_ERROR = 4,
	STATE_CONNECTED     = 5,
	STATE_DISCONNECTED  = 6,
};


const char* ConnectionStateStr(ConnectionStateEnum state);


class SockAddr;


class Socket
{
private:
	FdPtr m_fd;
public:
	int  LastError;

	Socket();
	Socket(int fd);
	void SetFd(int fd);
	int  GetFd() const;
	void Close();

	bool Create(int family, int type);
	bool Bind(const SockAddr& sockAddr);
	bool Listen(int backlog);
	int  Accept();
	

	int  GetError() const;
	bool IsReadable() const;
	bool IsWritable() const;
	bool IsClosed() const;
	int  BytesAvailable() const;
	bool SetNonBlock(int enabled);
	int  SetBroadcast(int enabled);

	static int  GetError(int sockdfd);
	static int  IsReadable(int sockdfd);
	static int  IsWritable(int sockfd);
	static bool SetNonBlock(int sockfd, int enable);

	/* Get the number of bytes that are immediately available for reading. */
	static int  BytesAvailable(int fd);
};
