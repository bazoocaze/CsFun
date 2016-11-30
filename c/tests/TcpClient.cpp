/*
 * Files....: TcpClient.cpp
 * Author...: Jose Ferreira
 * Date.....: 2015-08-12 - 20:48
 * Purpose..: Network client connection.
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
// #include <arpa/inet.h>

#include <errno.h>

#include "TcpClient.h"
#include "Config.h"
#include "Logger.h"
#include "Socket.h"
#include "Dns.h"



CTcpClient::CTcpClient()
{
	m_state = STATE_NOT_CONNECTED;
}


CTcpClient::CTcpClient(int clientfd)
{
	m_sock.SetFd(clientfd);
	m_stream.SetFd(clientfd);
	CLogger::LogMsg(LEVEL_VERBOSE, "Created network client (fd %d)", GetFd());
	m_state = STATE_CONNECTED;
}


bool CTcpClient::Connect(const char * hostName, int port)
{
	CIPAddressList list = CDns::GetHostAddresses(hostName, true, false);
	if(list.Count > 0)
	{
		CIPEndPoint remoteEP = CIPEndPoint((CIPAddress)list.Items[0], port);
		return Connect(remoteEP);
	}
	else
	{
		m_state = STATE_DNS_ERROR;
		return false;
	}
}


bool CTcpClient::Connect(const CIPEndPoint& remoteEP)
{
	CSockAddr addr = remoteEP.GetSockAddr();
	
	int ret;

	m_sock.Close();
	m_stream.SetFd(CLOSED_FD);

	m_state = STATE_CONNECT_ERROR;

	if(!addr.IsValid())
	{
		m_state = STATE_DNS_ERROR;
		return false;
	}

	/* cria o socket */
	if(!m_sock.Create(addr.GetFamily(), SOCK_STREAM))
		return false;

	// /* ajusta o socket para nao-bloqueante */
	// m_sock.SetNonBlock(true);
	
	/* tenta conectar o socket */
	errno = RET_OK;
	ret = connect(m_sock.GetFd(), addr.GetSockAddr(), addr.GetSize());
	if (ret == RET_OK || errno == EINPROGRESS) {
		if(ret == RET_OK)
			m_state = STATE_CONNECTED;
		else
			m_state = STATE_CONNECTING;
		errno = RET_OK;
		m_sock.LastErr = RET_OK;
		CLogger::LogMsg(LEVEL_VERBOSE, "Connected on %P (fd %d)", &remoteEP, GetFd());
		return true;
	} else {
		ret = errno;
		m_sock.Close();
		m_sock.LastErr = ret;
		return false;
	}
}


void CTcpClient::Close()
{
	InternalClose(STATE_NOT_CONNECTED, RET_OK);
}


void CTcpClient::InternalClose(ConnectionStateEnum newState, int errorCode)
{
	if(!m_sock.IsClosed())	{
		CLogger::LogMsg(LEVEL_VERBOSE, "Closing TCP client (fd %d)", GetFd());
		m_sock.Close();
	}
	m_sock.LastErr = errorCode;
	m_state = newState;
}


ConnectionStateEnum CTcpClient::State()
{
	if(m_state == STATE_CONNECTING)
	{
		// Check de sanidade: o socket esta desconectado
		if(m_sock.IsClosed())
		{
			InternalClose(STATE_DISCONNECTED, ECONNABORTED);
			return m_state;
		}
		
		if(m_sock.IsWritable())
		{		
			int ret = m_sock.GetSockError();
			if(ret != RET_OK) {
				InternalClose(STATE_CONNECT_ERROR, ret);
			} else {
				m_sock.SetNonBlock(false);
				m_state = STATE_CONNECTED;
			}
			return m_state;
		}
	}
	
	if(m_state == STATE_CONNECTED)
	{
		if(m_sock.IsReadable()) {
			int bytesAv = m_sock.BytesAvailable();
			if(bytesAv == 0) {
				/* Detectada desconexao pela outra parte / EOF / (tcp CLOSE_WAIT) */
				InternalClose(STATE_DISCONNECTED, ENOTCONN);
			}
		}
	}
	
	return m_state;
}


bool CTcpClient::IsConnected()
{
	int e = State();
	return (e == STATE_CONNECTED || e == STATE_CONNECTING);
}


bool CTcpClient::IsReady()
{
	int e = State();
	return (e == STATE_CONNECTED);
}


int CTcpClient::GetLastErr()
{
	return m_sock.LastErr;
}


const char * CTcpClient::GetLastErrMsg()
{
int e = State();
	if(e == STATE_DNS_ERROR) return "DNS error";
	return m_sock.GetLastErrMsg();
}


int CTcpClient::GetFd() const
{
	return m_sock.GetFd();
}


/*
int CTcpClient::Write(uint8_t c) {
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsWritable())
			return write(GetFd(), &c, 1);
	}
	return 0;
} */

/*
int CTcpClient::Write(uint8_t *ptr, int size) {
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsWritable())
			return write(GetFd(), ptr, size);
	}
	return 0;
} */


/*
int CTcpClient::Available() {
	if(State() == STATE_CONNECTED && m_sock.IsReadable())
		return 1;
	return 0;
} */


/*
int CTcpClient::Read() {
unsigned char c;
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsReadable())
		{
			int lido = read(GetFd(), &c, 1);
			if(lido != 1)
				return -1;
			return c;
		}
	}
	return -1;
} */

/*
int TcpClient::Peek() {
	Logger::LogMsg(LEVEL_ERROR, "TcpClient::Peek() not implemented");
	return -1;
} */

/*
void TcpClient::Flush() {
char buffer[16];
int lidos = 1;
	while(State() == STATE_CONNECTED && m_sock.IsReadable() && lidos) {
		lidos = read(GetFd(), buffer, sizeof(buffer));
	}
} */


void CTcpClient::SetFd(int fd)
{
	if(State() == STATE_CONNECTED)
		return;
	InternalClose(STATE_NOT_CONNECTED, RET_OK);
	m_sock.SetFd(fd);
	m_stream.SetFd(fd);
	m_state = STATE_CONNECTED;
	m_state = State();
}


CStream* CTcpClient::GetStream()
{
	if(m_stream.GetFd() != m_sock.GetFd())
		m_stream.SetFd(m_sock.GetFd());
	return &m_stream;
}
