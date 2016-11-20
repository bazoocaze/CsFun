/*
 * Arquivo....: TcpClient.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 12/08/2015 - 20:48
 * Objetivo...: Conexao de rede cliente.
 */


#pragma once


#include "Util.h"
#include "Stream.h"
#include "Socket.h"
#include "IPAddress.h"


/* Representa uma conexao cliente de rede TCP IPv4 */
class TcpClient : public Stream
{
private:
	/* finaliza a conexao atual com o estado e erro informados */
	void InternalClose(ConnectionStateEnum newState, int errorCode);
	int  ResolveName(const char *hostName, int port, void *targetAddress, int *targetAddressSize);
	
protected:	
	Socket m_sock;
	ConnectionStateEnum m_state;       /* estado da conexao */
	
public:
	/* Atualiza e retorna o estado atual do socket (enum E_ESTADO_CONEXAO) */
	ConnectionStateEnum State();

	TcpClient();
	TcpClient(int clientfd);
	
	/* Ajusta o FD conexao atual */
	void SetFd(int fd);
	/* retorna o fd da conexao atual */
	int  GetFd() const;
	/* Finaliza a conexao atual */
	void Close();
	
	/* Conecta no servidor e porta informados */
	bool Connect(const char * hostName, int port);
	bool Connect(const IPEndPoint& remoteEP);

	/* Retorna TRUE/FALSE se o socket esta conectando ou conectado */
	int  IsConnected();
	/* Retorna TRUE/FALSE se o socket esta conectado na outra ponta */
	int  IsReady();
	
	/* Numero do ultimo erro. Use GetMsgError() para obter a mensagem de erro. */
	int    LastError;
	cstr   GetMsgError();
	
	size_t Write(uint8_t c);
	size_t Write(uint8_t *ptr, size_t size);
	int    Available();
	int    Read();
	int    Peek();
	void   Flush();
};
