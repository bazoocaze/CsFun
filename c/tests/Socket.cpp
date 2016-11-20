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



Socket::Socket()
{
	m_fd = CLOSED_FD;
	LastError = RET_OK;
}


Socket::Socket(int fd)
{
	m_fd = fd;
	LastError = RET_OK;
}


void Socket::SetFd(int fd)
{
	m_fd = fd;
	LastError = RET_OK;
}


int Socket::GetFd() const
{
	return m_fd;
}


void Socket::Close()
{
	m_fd = CLOSED_FD;
	LastError = RET_OK;
}


// Family: AF_INET or AF_INET6
// Type:   SOCK_STREAM or SOCK_DGRAM
bool Socket::Create(int family, int type)
{
int ret = socket(family, SOCK_STREAM, 0);
	if(ret == RET_ERROR)
	{
		LastError = errno;
		return false;
	}
	m_fd = ret;
	return true;
}


bool Socket::Bind(const SockAddr& sockAddr)
{
int ret = bind(m_fd, sockAddr.GetSockAddr(), sockAddr.GetSize());
	if(ret != RET_ERROR) return true;
	LastError = errno;
	return false;
}


bool Socket::Listen(int backlog)
{
int ret = listen(m_fd, backlog);
	if(ret != RET_ERROR) return true;
	LastError = errno;
	return false;
}


int Socket::Accept()
{
int ret = accept(m_fd, NULL, NULL);
	if(ret == RET_ERROR) LastError = errno;
	return ret;
}


int Socket::GetError() const
{
	return GetError(m_fd);
}


bool Socket::IsReadable() const
{
	return IsReadable(m_fd);
}


bool Socket::IsWritable() const
{
	return IsWritable(m_fd);
}


bool Socket::IsClosed() const
{
	return m_fd == CLOSED_FD;
}


bool Socket::SetNonBlock(int enabled)
{
	return SetNonBlock(m_fd, enabled);
}


int Socket::SetBroadcast(int enabled)
{
	return setsockopt(m_fd, SOL_SOCKET, SO_BROADCAST, &enabled, sizeof(enabled));
}


int Socket::BytesAvailable() const
{
	return BytesAvailable(m_fd);
}


int Socket::GetError(int sockfd)
{
int retval = 0;
socklen_t len = sizeof(retval);
	int ret = getsockopt(sockfd, SOL_SOCKET, SO_ERROR, &retval, &len);
	if (ret == RET_OK) return retval;
	return EFAULT;
}


int Socket::IsReadable(int sockfd)
{
fd_set fdread;
int ret;
struct timeval t;
	FD_ZERO(&fdread);
	FD_SET(sockfd, &fdread);
	t.tv_sec=0;
	t.tv_usec=0;
	ret = select(sockfd + 1, &fdread, NULL, NULL, &t);
	return (ret == 1);
}


int Socket::IsWritable(int sockfd)
{
fd_set fdwrite;
int ret;
struct timeval t;
	FD_ZERO(&fdwrite);
	FD_SET(sockfd, &fdwrite);
	t.tv_sec=0;
	t.tv_usec=0;
	ret = select(sockfd + 1, NULL, &fdwrite, NULL, &t);
	return (ret == 1);
}




/* ajusta o socket para nao-bloqueante sim/nao */
bool Socket::SetNonBlock(int sockfd, int enable)
{
	int ret;
	int fdfl = fcntl(sockfd, F_GETFL);
	if(fdfl != RET_ERROR)	{
		if(enable)
			fdfl |= O_NONBLOCK;
		else
			fdfl &= (~O_NONBLOCK);
		ret = fcntl(sockfd, F_SETFL, fdfl);
		return ret != RET_ERROR;
	}
	return false;
}


// Get the number of bytes that are immediately available for reading.
int Socket::BytesAvailable(int fd) {
int ret;
int bytesAv;
	ret = ioctl(fd, FIONREAD, &bytesAv);
	if(ret != RET_OK)
		return ret;
	return bytesAv;
}


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

