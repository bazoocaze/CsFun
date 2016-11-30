#include <stdio.h>
#include <stdlib.h>
#include <string.h>


#include "Ptr.h"
#include "Protobuf.h"
#include "TcpListener.h"
#include "Stream.h"
#include "Binary.h"
#include "Threading.h"
#include "Logger.h"
#include "IO.h"




class CServerThread : public CThread
{
public:
	bool StopServer;
	CTcpListener listener;

	void ExecuteThread()
	{
		StopServer = false;

		if(!listener.Start(12345))
			return;

		CLogger::LogMsg(LEVEL_INFO, "Server: iniciado");

		while(listener.IsListening() && !StopServer)
		{
			CTcpClient client;
			if(listener.TryAccept(client, 50000))
			{
				CLogger::LogMsg(LEVEL_INFO, "Server: conexao recebida");
				ClientConnectedCallback(client);
			}
		}

		listener.Stop();

		CLogger::LogMsg(LEVEL_INFO, "Server: finalizado");
	}

	void ClientConnectedCallback(CTcpClient & client)
	{
		CStream * stream = client.GetStream();
		CBinaryReader br = CBinaryReader(stream);
		int req = br.ReadInt32();
		CLogger::LogMsg(LEVEL_INFO, "Server: received req=%d", req);
		CBinaryWriter bw = CBinaryWriter(stream);
		bw.WriteInt32(req * 2);
		client.Close();
	}

	void Stop()
	{
		StopServer = true;
		// CTcpClient dummy;
		// dummy.Connect("127.0.0.1", 12345);
		// dummy.Close();
		listener.Stop();
	}
};


CServerThread server;


void teste_client()
{
	CTcpClient client;
	if(!client.Connect("127.0.0.1", 12345))
	{
		CLogger::LogMsg(LEVEL_INFO, "Client: failed to connect to server");
		return;
	}
	CBinaryWriter bw = CBinaryWriter(client.GetStream());
	CBinaryReader br = CBinaryReader(client.GetStream());
	bw.WriteInt32(42);
	int lido1;
	int lido2;
	bool b;
	// lido1 = br.ReadInt32();
	b = br.TryReadInt32(&lido2);
	CLogger::LogMsg(LEVEL_INFO, "Client: recebido lido1=%d lido2=%d b=%d", lido1, lido2, b);
}


void teste_server()
{
	server.Start();
	delay(1000);
	teste_client();
	delay(3000);
	server.Stop();
	server.Join();
}


void teste_stdin()
{
	while(!(StdIn.IsEof() || StdIn.IsError()))
	{
		char buffer[256];
		int lidos = StdIn.Read(buffer, sizeof(buffer));
		if(lidos >= 0) buffer[lidos] = 0;
		StdOut.printf("lidos = %d, buffer = [%s]\n", lidos, buffer);
	}
}


void teste_main()
{
	CLogger::LogMsg(LEVEL_INFO, "TESTE:BEGIN");

	teste_server();

	CLogger::LogMsg(LEVEL_INFO, "TESTE:END");
}
