/*
 * Arquivo....: Socket.cpp
 * Autor......: Jose Ferreira
 * Data.......: 2016-11-22 - 22:26
 * Objetivo...: Network sockets.
 */
 
 
#include <errno.h>
#include <string.h>

#include "TcpClient.h"
#include "Config.h"
#include "Logger.h"
#include "FdSelect.h"


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


int CSocket::Connect(const CSockAddr& address)
{
	int ret = connect(m_fd, address.GetSockAddr(), address.GetSize());
	if(ret == RET_ERROR)
		LastErr = errno;
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


const char * CSocket::GetLastErrMsg() const
{
	return strerror(LastErr);
}


void CSocket::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


int CSocket::GetFd() const
{
	return m_fd;
}


void CSocket::Close()
{
	m_fd = CLOSED_FD;
}


bool CSocket::IsClosed() const
{
	return (m_fd == CLOSED_FD);
}


bool CSocket::IsReadable() const
{
	int ret = CFdSelect::Wait(m_fd, SEL_READ | SEL_ERROR, 0);
	return (ret > 0) && (HAS_FLAG(ret, SEL_READ));
}


bool CSocket::IsWritable() const
{
	int ret = CFdSelect::Wait(m_fd, SEL_WRITE | SEL_ERROR, 0);
	return (ret > 0) && (HAS_FLAG(ret, SEL_WRITE));
}


bool CSocket::SetNonBlock(int enabled)
{
	return CFd::SetNonBlock(m_fd, enabled);
}


int CSocket::BytesAvailable() const
{
	return CFd::BytesAvailable(m_fd);
}


//////////////////////////////////////////////////////////////////////
// CSocketHandle
//////////////////////////////////////////////////////////////////////


CSocketHandle::CSocketHandle()
{
	this->Fd  = CLOSED_FD;
	Set(CLOSED_FD);
}

CSocketHandle::CSocketHandle(int fd)
{
	this->Fd  = CLOSED_FD;
	Set(fd);
}

CSocketHandle::~CSocketHandle()
{
	Close();
}

void CSocketHandle::Close()
{
	Set(CLOSED_FD);
}

void CSocketHandle::Set(int fd)
{
	if(fd == Fd) return;
	this->Fd = fd;
	intptr_t p = (intptr_t)(fd + 1);
	SetDataPtr((void*)p);
}

void CSocketHandle::Debug()
{
	CRefPtr::Debug();
}

void CSocketHandle::ReleaseData(void * data)
{
	intptr_t fd = ((intptr_t)data) - 1;
	ReleaseFd(fd);
}

void CSocketHandle::ReleaseFd(int fd)
{
	CLogger::LogMsg(LEVEL_VERBOSE, "CFdPtr: close(%d)", fd);
	close(fd);
}
