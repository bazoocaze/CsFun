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


const char* ConnectionStateStr(ConnectionStateEnum state) {
	RET_ENUM_STR(state, STATE_NOT_CONNECTED);
	RET_ENUM_STR(state, STATE_LISTENING);
	RET_ENUM_STR(state, STATE_CONNECTING);
	RET_ENUM_STR(state, STATE_DNS_ERROR);
	RET_ENUM_STR(state, STATE_CONNECT_ERROR);
	RET_ENUM_STR(state, STATE_CONNECTED);
	RET_ENUM_STR(state, STATE_DISCONNECTED);
	return "<unknow>";
}



//////////////////////////////////////////////////////////////////////
// Socket
//////////////////////////////////////////////////////////////////////



Socket::Socket()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


Socket::Socket(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


bool Socket::Create(int family, int type)
{
int ret;
	m_fd = CLOSED_FD;
	ret = socket(family, type, 0);
	if(ret == RET_ERROR)
	{
		LastErr = errno;
		return false;
	}
	m_fd = ret;
	return true;
}


bool Socket::Bind(const SockAddr& sockAddr)
{
int ret = bind(m_fd, sockAddr.GetSockAddr(), sockAddr.GetSize());
	if(ret != RET_ERROR) return true;
	LastErr = errno;
	return false;
}


bool Socket::Listen(int backlog)
{
int ret = listen(m_fd, backlog);
	if(ret != RET_ERROR) return true;
	LastErr = errno;
	return false;
}


int Socket::Accept()
{
int ret = accept(m_fd, NULL, NULL);
	if(ret == RET_ERROR) LastErr = errno;
	return ret;
}


int Socket::GetSockError() const
{
	return Socket::GetSockError(m_fd);
}


int Socket::SetBroadcast(int enabled)
{
	return setsockopt(m_fd, SOL_SOCKET, SO_BROADCAST, &enabled, sizeof(enabled));
}


int Socket::GetSockError(int sockfd)
{
int retval = 0;
socklen_t len = sizeof(retval);
	int ret = getsockopt(sockfd, SOL_SOCKET, SO_ERROR, &retval, &len);
	if (ret == RET_OK) return retval;
	return EFAULT;
}
