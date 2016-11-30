/*
 * Arquivo....: Socket.cpp
 * Autor......: Jose Ferreira
 * Data.......: 2016-11-22 - 22:26
 * Objetivo...: Network sockets.
 */
 
 
#include <errno.h>

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



CSocket::CSocket()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


CSocket::CSocket(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


bool CSocket::Create(int family, int type)
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


bool CSocket::Bind(const CSockAddr& sockAddr)
{
int ret = bind(m_fd, sockAddr.GetSockAddr(), sockAddr.GetSize());
	if(ret != RET_ERROR)
		return true;
	LastErr = errno;
	return false;
}


bool CSocket::Listen(int backlog)
{
int ret = listen(m_fd, backlog);
	if(ret != RET_ERROR)
		return true;
	LastErr = errno;
	return false;
}


int CSocket::Accept()
{
int ret = accept(m_fd, NULL, NULL);
	if(ret == RET_ERROR)
		LastErr = errno;
	return ret;
}


int CSocket::Accept(CSockAddr& address)
{
	socklen_t addrSize = address.GetMaxSize();
	int ret = accept(m_fd, address.GetSockAddr(), &addrSize);
	if(ret == RET_ERROR)
		LastErr = errno;
	address.SetSize(addrSize);
	return ret;
}


int CSocket::GetSockError() const
{
	return CSocket::GetSockError(m_fd);
}


int CSocket::SetBroadcast(int enabled)
{
	return setsockopt(m_fd, SOL_SOCKET, SO_BROADCAST, &enabled, sizeof(enabled));
}


int CSocket::GetSockError(int sockfd)
{
int retval = 0;
socklen_t len = sizeof(retval);
	int ret = getsockopt(sockfd, SOL_SOCKET, SO_ERROR, &retval, &len);
	if (ret == RET_OK)
		return retval;
	return EFAULT;
}
