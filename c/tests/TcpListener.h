/*
 * Arquivo....: TcpListener.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 12/08/2015 - 20:52
 * Objetivo...: Servidor de conexoes TCP.
 */


#pragma once

#include "TcpClient.h"
#include "IPAddress.h"
#include "Socket.h"


class TcpListener
{
private:
	void LogErr(const char *msg);
	bool InternalAccept(TcpClient &cliente);
	
protected:
	Socket m_sock;
	IPEndPoint m_endpoint;
	
public:
	TcpListener();
	~TcpListener();

	/* Retorna o filedescriptor do listener */
	int  GetFd() const;

	/* Comeca a aguardar conexoes na porta informada */
	bool Start(int port);
	bool Start(const IPAddress & addr, int port);
	bool Start(const IPEndPoint & localEP);

	/* Finaliza a conexao atual */
	void Stop();

	/* Retorna TRUE/FALSE se o socket esta conectando ou conectado */
	bool IsListening() const;

	/* Retorna TRUE/FALSE se existe uma nova conexao pendente para Accept */
	bool Available() const;

	/* Aceita uma nova conexao pendente */
	bool Accept(TcpClient & client);
	bool AcceptNonBlock(TcpClient & client);

	/* Numero do ultimo erro. Use GetMsgError() para obter a mensagem de erro. */
	int  LastError;
	/* Retorna a mensagem de erro referente ao LastError */
	cstr GetMsgError() const;   /* Retorna a mensagem de erro referente ao LastError */
};
