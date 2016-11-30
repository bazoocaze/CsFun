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
#include "FdSelect.h"


CTcpListener::CTcpListener()
{
}


CTcpListener::~CTcpListener()
{
	Stop();
}

int CTcpListener::GetFd() const
{
	return m_sock.GetFd();
}

bool CTcpListener::Start(int port)
{
	CIPEndPoint localEP = CIPEndPoint(CIPAddress::Any, port);
	return Start(localEP);
}


bool CTcpListener::Start(const CIPAddress& addr, int port)
{
	CIPEndPoint localEP = CIPEndPoint(addr, port);
	return Start(localEP);
}


bool CTcpListener::Start(const CIPEndPoint& localEP)
{
	CSockAddr addr = localEP.GetSockAddr();
	m_endpoint = localEP;

	m_sock.Close();

	if(!addr.IsValid())
	{
		/* Invalid endpoint */
		return false;
	}

	/* create TCP socket for the family. */
	if(!m_sock.Create(addr.GetFamily(), SOCK_STREAM))
	{
		LogErr("socket()");
		return false;
	}
	
	// /* adjust socket to non-blocking */
	// m_sock.SetNonBlock(true);
	
	/* bind into address_any and port */
	if(!m_sock.Bind(addr))
	{
		m_sock.Close();
		LogErr("bind()");
		return false;
	}

	if(!m_sock.Listen(P_NETWORK_SOCK_BACKLOG))
	{
		m_sock.Close();
		LogErr("listen()");
		return false;
	}

	CLogger::LogMsg(
			LEVEL_DEBUG,
			"TCP listener started on %P (fd %d)",
			&m_endpoint,
			m_sock.GetFd());

	m_stopped = false;
	
	return true;
}


bool CTcpListener::Available() const {
	if(m_sock.IsClosed())
		return false;
	return m_sock.IsReadable();
}


bool CTcpListener::WaitAvailable(int timeoutMillis) const {
	if(m_sock.IsClosed())
		return false;
	int status = CFdSelect::Wait(m_sock.GetFd(), SEL_READ | SEL_ERROR, timeoutMillis);
	return (HAS_FLAG(status, SEL_READ));
}


bool CTcpListener::InternalAccept(CTcpClient& cliente)
{
int clientfd;
CSockAddr addr;

	if(!IsListening())
		return false;

	clientfd = m_sock.Accept(addr);
	if(clientfd == RET_ERROR) {
		CLogger::LogMsg(
				LEVEL_ERROR,
				"TcpListener: error accepting client on %P: %d-%s",
				&m_endpoint,
				m_sock.LastErr,
				m_sock.GetLastErrMsg());
		return false;
	}

	CIPEndPoint remoteEP = CIPEndPoint(addr);
	CLogger::LogMsg(
			LEVEL_DEBUG,
			"TCP client %P accepted on %P (fd %d)",
			&remoteEP,
			&m_endpoint,
			clientfd);
	cliente.SetFd(clientfd);

	return true;
}


bool CTcpListener::Accept(CTcpClient &cliente) {
	if(!IsListening())
		return false;
	m_sock.SetNonBlock(false);
	return InternalAccept(cliente);
}


bool CTcpListener::TryAccept(CTcpClient &cliente, int timeoutMillis)
{
	if(!IsListening())
		return false;

	int status = CFdSelect::Wait(m_sock.GetFd(), SEL_READ | SEL_ERROR, timeoutMillis);

	if(!IsListening())
		return false;

	if(!HAS_FLAG(status, SEL_READ))
		return false;

	return InternalAccept(cliente);
}


bool CTcpListener::AcceptNonBlock(CTcpClient &cliente) {
	if(!IsListening())
		return false;
	m_sock.SetNonBlock(true);
	return InternalAccept(cliente);
}


void CTcpListener::Stop()
{
	CLogger::LogMsg(LEVEL_DEBUG, "Listener.Stop: BEGIN");

	if(!m_stopped) {
		m_stopped = true;

		CLogger::LogMsg(LEVEL_DEBUG, "Listener.Stop: ENTER");

		if(!m_sock.IsClosed())
		{
			// connect on self listener to weak the listener thread
			CTcpClient dummy;
			CIPEndPoint selfEP = CIPEndPoint(CIPAddress::Loopback, m_endpoint.GetPort());
			dummy.Connect(selfEP);
			dummy.Close();

			CLogger::LogMsg(
				LEVEL_DEBUG,
				"Shutdown TCP listener on %P (fd %d)",
				&m_endpoint,
				m_sock.GetFd());

			m_sock.Close();
		}
	}
}


bool CTcpListener::IsListening() const
{
	return (!(m_stopped || m_sock.IsClosed()));
}


int CTcpListener::GetLastErr() const
{
	return m_sock.LastErr;
}


const char * CTcpListener::GetLastErrMsg() const
{
	return m_sock.GetLastErrMsg();
}


void CTcpListener::LogErr(const char* msg)
{
	CLogger::LogMsg(
		LEVEL_ERROR,
		"TCP listener (on %P, fd %d): %s - err %d-%s",
		&m_endpoint,
		m_sock.GetFd(),
		msg,
		m_sock.LastErr,
		m_sock.GetLastErrMsg());
}
