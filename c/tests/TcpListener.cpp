/*
 * File.....: TcpListener.cpp
 * Author...: Jose Ferreira
 * Date.....: 2015-08-12 - 20:52
 * Purpose..: TCP server.
 */


// #include <stdio.h>
// #include <stdlib.h>
// #include <sys/types.h>
// #include <sys/socket.h>
// #include <netdb.h>
// #include <string.h>
// #include <unistd.h>
// #include <errno.h>
// #include <sys/time.h>
// #include <fcntl.h>
// #include <sys/ioctl.h>
// #include <errno.h>
// #include <arpa/inet.h>

#include <errno.h>

#include "TcpListener.h"
#include "TcpClient.h"
#include "Config.h"
#include "Logger.h"


TcpListener::TcpListener()
{
}


TcpListener::~TcpListener()
{
	Stop();
}

int TcpListener::GetFd() const
{
	return m_sock.GetFd();
}

bool TcpListener::Start(int port)
{
	IPEndPoint localEP = IPEndPoint(IPAddress::Any, port);
	return Start(localEP);
}


bool TcpListener::Start(const IPAddress& addr, int port)
{
	IPEndPoint localEP = IPEndPoint(addr, port);
	return Start(localEP);
}


bool TcpListener::Start(const IPEndPoint& localEP)
{
	SockAddr addr = localEP.GetSockAddr();
	m_endpoint = localEP;

	m_sock.Close();

	if(!addr.IsValid())
	{
		/* Invalid endpoint */
		return false;
	}

	if(!m_sock.Create(addr.GetFamily(), SOCK_STREAM))
	{
		LogErr("socket()");
		return false;
	}
	
	/* adjust socket to non-blocking */
	m_sock.SetNonBlock(true);
	
	/* bind into address_any and port */
	if(!m_sock.Bind(addr))
	{
		m_sock.Close();
		LogErr("bind()");
		return false;
	}

	if(!m_sock.Listen(1))
	{
		m_sock.Close();
		LogErr("listen()");
		return false;
	}

	Logger::LogMsg(
			LEVEL_DEBUG,
			"TCP listener started on %z (fd %d)",
			&m_endpoint,
			m_sock.GetFd());
	
	return true;
}


bool TcpListener::Available() const {
	if(m_sock.IsClosed()) return false;
	return m_sock.IsReadable();
}


bool TcpListener::InternalAccept(TcpClient& cliente)
{
int clientfd;
SockAddr addr;
	clientfd = m_sock.Accept(addr);
	if(clientfd == RET_ERROR) {
		Logger::LogMsg(
				LEVEL_ERROR,
				"TcpListener: error accepting client on %z: %d-%s",
				&m_endpoint,
				m_sock.LastErr,
				m_sock.GetLastErrMsg());
		return false;
	}
	IPEndPoint remoteEP = IPEndPoint(addr);
	Logger::LogMsg(
			LEVEL_DEBUG,
			"TCP client %z accepted on %z (fd %d)",
			remoteEP,
			&m_endpoint,
			clientfd);
	cliente.SetFd(clientfd);
	return true;
}


bool TcpListener::Accept(TcpClient &cliente) {
	if(!IsListening()) return false;
	m_sock.SetNonBlock(false);
	return InternalAccept(cliente);
}


bool TcpListener::AcceptNonBlock(TcpClient &cliente) {
	if(!IsListening()) return false;
	m_sock.SetNonBlock(true);
	return InternalAccept(cliente);
}


void TcpListener::Stop()
{
	if(IsListening())	{
		Logger::LogMsg(
			LEVEL_DEBUG,
			"Shutdown TCP listener on %z (fd %d)",
			&m_endpoint,
			m_sock.GetFd());
		m_sock.Close();
	}
}


bool TcpListener::IsListening() const
{
	return (!m_sock.IsClosed());
}


// const char * TcpListener::GetLastErrMsg() const
// {
// 	return strerror(LastErr);
// }

int TcpListener::GetLastErr() const
{
	return m_sock.LastErr;
}


const char * TcpListener::GetLastErrMsg() const
{
	return m_sock.GetLastErrMsg();
}


void TcpListener::LogErr(const char* msg)
{
	Logger::LogMsg(
		LEVEL_ERROR,
		"TCP listener (on %z, fd %d): %s - err %d-%s",
		&m_endpoint,
		m_sock.GetFd(),
		msg,
		m_sock.LastErr,
		m_sock.GetLastErrMsg());
}

