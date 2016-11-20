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
#include "Socket.h"
#include "Dns.h"



TcpClient::TcpClient() : Stream()
{
	// m_sockfd = CLOSED_FD;
	m_state = STATE_NOT_CONNECTED;
	LastError = RET_OK;
}


TcpClient::TcpClient(int clientfd) : Stream()
{
	m_sock.SetFd(clientfd);
	// m_sockfd = clientfd;
	Logger::LogMsg(LEVEL_VERBOSE, "TcpClient:create with fd=%d", GetFd());
	m_state = STATE_CONNECTED;
	LastError = RET_OK;
}


bool TcpClient::Connect(const char * hostName, int port)
{
	IPAddressList list = Dns::GetHostAddresses(hostName, true, false);
	if(list.Count > 0)
	{
		IPEndPoint remoteEP = IPEndPoint((IPAddress)list.Items[0], port);
		Connect(remoteEP);
	}
	else
	{
		m_state = STATE_DNS_ERROR;
	}
}


bool TcpClient::Connect(const IPEndPoint& remoteEP)
{
	SockAddr addr = remoteEP.GetSockAddr();
	
	int tempfd, ret;

	m_sock.Close();

	// if (m_sockfd != CLOSED_FD) {
	// 	// TODO: tratar a situação: socket já criado
	// 	return;
	// }

	LastError = RET_OK;
	m_state = STATE_CONNECT_ERROR;

	if(!addr.IsValid())
	{
		m_state = STATE_DNS_ERROR;
		return false;
	}

	/* cria o socket */
	if(!m_sock.Create(addr.GetFamily(), SOCK_STREAM))
	{
		LastError = m_sock.LastError;
		return false;
	}

	/* ajusta o socket para nao-bloqueante */
	m_sock.SetNonBlock(true);
	
	/* tenta conectar o socket */
	errno = RET_OK;
	ret = connect(m_sock.GetFd(), addr.GetSockAddr(), addr.GetSize());
	if (ret == RET_OK || errno == EINPROGRESS) {
		if(ret == RET_OK)
			m_state = STATE_CONNECTED;
		else
			m_state = STATE_CONNECTING;
		errno = RET_OK;
		LastError = RET_OK;
		// m_sockfd = tempfd;
		Logger::LogMsg(LEVEL_VERBOSE, "TcpClient:connected with fd=%d", GetFd());
		return true;
	} else {
		LastError = errno;
		m_sock.Close();
		return false;
	}
}


void TcpClient::Close()
{
	InternalClose(STATE_NOT_CONNECTED, RET_OK);
}


void TcpClient::InternalClose(ConnectionStateEnum novoEstado, int errorCode)
{
	if(!m_sock.IsClosed())	{
		Logger::LogMsg(LEVEL_VERBOSE, "TcpClient:closefd(%d)", GetFd());
		m_sock.Close();
	}
	LastError = errorCode;
	m_state = novoEstado;
}


ConnectionStateEnum TcpClient::State()
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
			int ret = m_sock.GetError();
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


int TcpClient::IsConnected()
{
	int e = State();
	if(e == STATE_CONNECTED || e == STATE_CONNECTING)
		return TRUE;
	return FALSE;
}


int TcpClient::IsReady()
{
	int e = State();
	if(e == STATE_CONNECTED)
		return TRUE;
	return FALSE;
}


const char * TcpClient::GetMsgError()
{
	int e = State();
	if(e == STATE_DNS_ERROR)
		return gai_strerror(LastError);
	return strerror(LastError);
}


int TcpClient::ResolveName(const char *nome, int porta, void *enderDest, int *enderSize)
{
int ret;

	memset(enderDest, 0, sizeof(sockaddr));
	*enderSize=0;
	
	if(TCP_DNS_USE_getaddrinfo) {
		/* resolve o nome do servidor usando DNS */
		
		struct addrinfo *dnsResult, *rp;
		struct addrinfo hints;
		
		memset(&hints, 0, sizeof(hints));
		hints.ai_family = AF_INET;
		hints.ai_socktype = SOCK_STREAM;

		ret = getaddrinfo(nome, NULL, &hints, &dnsResult);
		if (ret != RET_OK) {
			this->LastError = ret;
			return FALSE;
		}
		
		/* Para cada ip/dns encontrado */
		for (rp = dnsResult; rp != NULL; rp = rp->ai_next) {
			if (rp->ai_family == AF_INET) {
				/* resultado IPv4: ajusta o numero da porta */
				sockaddr_in *in = (sockaddr_in*)rp->ai_addr;
				in->sin_port = htons(porta);
				*enderSize = sizeof(sockaddr_in);
				memcpy(enderDest, in, sizeof(sockaddr_in));
				break;
			}
		}
		
		/* libera os recursos da pesquisa DNS */
		freeaddrinfo(dnsResult);
		
		return (enderSize > 0);
		
	} else {
		
		/* resolve tratando como IP direto */
		sockaddr_in *in = (sockaddr_in *)(enderDest);
		*enderSize = sizeof(sockaddr_in);
		in->sin_family = AF_INET;
		in->sin_port = htons(porta);
		in->sin_addr.s_addr = inet_addr(nome);
		
		return (in->sin_addr.s_addr != 0xFFFF);
	}
}


int TcpClient::GetFd() const
{
	return m_sock.GetFd();
}


size_t TcpClient::Write(uint8_t c) {
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsWritable())
			return write(GetFd(), &c, 1);
	}
	return 0;
}


size_t TcpClient::Write(uint8_t *ptr, size_t size) {
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsWritable())
			return write(GetFd(), ptr, size);
	}
	return 0;
}


int TcpClient::Available() {
	if(State() == STATE_CONNECTED && m_sock.IsReadable())
		return 1;
	return 0;
}


int TcpClient::Read() {
unsigned char c;
int lido;
	if(State() == STATE_CONNECTED) {
		if(m_sock.IsReadable())
			lido = read(GetFd(), &c, 1);
			if(lido != 1) return -1;
			if(c < 0) {
				Logger::LogMsg(LEVEL_DEBUG, "c < 0 (c=%d)", c);
			}
			return c;
	}
	return -1;
}


int TcpClient::Peek() {
	Logger::LogMsg(LEVEL_ERROR, "TcpClient::Peek() not implemented");
	return -1;
}


void TcpClient::Flush() {
char buffer[16];
int lidos = 1;
	while(State() == STATE_CONNECTED && m_sock.IsReadable() && lidos) {
		lidos = read(GetFd(), buffer, sizeof(buffer));
	}
}


void TcpClient::SetFd(int fd)
{
	if(State() == STATE_CONNECTED)
		return;
	InternalClose(STATE_NOT_CONNECTED, RET_OK);
	m_sock.SetFd(fd);
	m_state = STATE_CONNECTED;
	m_state = State();
}
