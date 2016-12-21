#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>


#include "Ptr.h"
#include "Protobuf.h"
#include "TcpListener.h"
#include "Stream.h"
#include "Binary.h"
#include "Threading.h"
#include "Logger.h"
#include "IO.h"
#include "Util.h"
#include "FdSelect.h"
#include "Text.h"


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
		listener.Stop();
		Join();
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
	int lido1 = 0;
	int lido2 = 0;
	bool b = false;
	// lido1 = br.ReadInt32();
	b = br.TryReadInt32(&lido2);
	CLogger::LogMsg(LEVEL_INFO, "Client: recebido lido1=%d lido2=%d b=%d", lido1, lido2, b);
}


void teste_server()
{
	server.Start();
	// delay(1000);
	// teste_client();
	delay(30000);
	server.Stop();
	server.Join();
}


void teste_stdin()
{
	while(!(StdIn.IsEof() || StdIn.IsError()))
	{
		char buffer[256];
		int lidos = StdIn.Read((void*)buffer, (int)sizeof(buffer));
		if(lidos >= 0) buffer[lidos] = 0;
		StdOut.printf("lidos = %d, buffer = [%s]\n", lidos, buffer);
	}
}


void teste_handle()
{
	CLogger::LogMsg(LEVEL_INFO, "-- TESTE: P10 --");

	CString str1 = "aula";
	CString str2;
	CString str3;

	str2.Set(UTIL_MEM_STRDUP("bravo"), true);
	str2.Set(UTIL_MEM_STRDUP("ducker"), true);

	StdErr.printf("str1 = %P\nstr2 = %P\nstr3 = %P\n", &str1, &str2, &str3);

	CLogger::LogMsg(LEVEL_INFO, "-- TESTE: P999 --");
}


class TesteMain
{
public:
	int Valor1;
	virtual ~TesteMain()
	{
		StdErr.print("[TesteMain:dtor]");
	}
	void kill()
	{
		delete this;
	}
};


class TesteSub : public TesteMain
{
public:
	int Valor2;
	~TesteSub()
	{
		StdErr.print("[TesteSub:dtor]");
	}
};


void teste_class()
{
	CLogger::LogMsg(LEVEL_INFO, "-- TESTE1:BEGIN --");

	// TesteSub* teste = new TesteSub();
	// TesteMain* ptr = teste;
	// delete ptr;
	// ptr->kill();

	StdOut.print("[ STDOUT P10]");
	StdErr.print("[ STDERR P10]");

	CLogger::LogMsg(LEVEL_INFO, "-- TESTE1:P10 --");

	StdOut.print("[ STDOUT P20]");
	StdErr.print("[ STDERR P20]");

	// TesteSub teste2;

	CLogger::LogMsg(LEVEL_INFO, "-- TESTE1:BEGIN --");

	StdOut.print("[ STDOUT P30]");
	StdErr.print("[ STDERR P30]");
}


void teste_udp()
{
	CIPEndPoint localEP = CIPEndPoint(CIPAddress::Any, 12347);
	CIPEndPoint remoteEP = CIPEndPoint("200.203.44.6", 12346);

	CSocket socket;
	if(!socket.Create(AF_INET, SOCK_STREAM))
	{
		StdErr.printf("socket() error: %d-%s", socket.LastErr, socket.GetLastErrMsg());
		return;
	}

	/*
	if(!socket.Bind(localEP.GetSockAddr()))
	{
		StdErr.printf("bind() error: %d-%s", socket.LastErr, socket.GetLastErrMsg());
		return;
	} */

	socket.SetNonBlock(true);

	if(socket.Connect(remoteEP.GetSockAddr()) != RET_OK)
	{
		StdErr.printf("connect() error: %d-%s", socket.LastErr, socket.GetLastErrMsg());
		if(socket.LastErr != EINPROGRESS)
			return;
	}

	bool connected = false;

	NetworkStream ns(socket);
	CStreamWriter sw(&ns);
	CStreamReader sr(&ns);

	while(true)
	{
		StdOut.print("[local]Digite:");

		CFdSelect sel;
		sel.Add(socket.GetFd(), SEL_READ | SEL_ERROR | (!connected ? SEL_WRITE : 0) );
		sel.Add(0, SEL_READ | SEL_ERROR);
		int ret = sel.WaitAll(180000);

		if(ret > 0)
		{
			int localStatus = sel.GetStatus(0);
			int remoteStatus = sel.GetStatus(socket.GetFd());

			if(HAS_FLAG(remoteStatus, SEL_WRITE))
			{
				connected = true;
				StdOut.println("[Connected]\n");
			}

			if(HAS_FLAG(localStatus, SEL_READ))
			{
				CString linha;
				if(!StdIn.ReadLine(linha, 1024)) break;
				if(linha.c_str[0] == 0) break;
				sw.println(linha);

				if(sw.IsError())
				{
					StdErr.printf("Write error: %d-%s\n", sw.GetLastErr(), sw.GetLastErrMsg());
				}
			}

			if(HAS_FLAG(remoteStatus, SEL_ERROR))
			{
				StdErr.printf("Remote error: %d-%s\n", sw.GetLastErr(), sw.GetLastErrMsg());
				break;
			}

			if(HAS_FLAG(remoteStatus, SEL_READ))
			{
				char buffer[1024];
				int lidos = sr.Read(buffer, sizeof(buffer)-1);
				if(lidos >= 0)
				{
					buffer[lidos] = 0;
					StdOut.printf("\n[remoto][%s]\n", buffer);
				}
				if(lidos <= 0)
				{
					if(lidos == 0)
						StdOut.printf("\n[remoto EOS]\n");
					else
						StdOut.printf("\n[remoto error %d-%s]\n", sr.GetLastErr(), sr.GetLastErrMsg());

					break;
				}
			}

		}

		if(ret == 0)
		{
			StdOut.print("[select timeout]\n");
			break;
		}

		if(ret < 0)
		{
			StdOut.print("[select error]\n");
			break;
		}
	}

	// ret = socket.Write("ducker\n", 7);
	// if(ret == RET_ERR)
	// {
	// 	StdErr.printf("send() error: %d-%s", socket.LastErr, socket.GetLastErrMsg());
	// 	return;
	// }
}


void teste_main()
{
	CLogger::LogMsg(LEVEL_INFO, "-- TESTE:BEGIN --");

	teste_udp();
	// teste_server();
	//teste_handle();
	// teste_class();

	CLogger::LogMsg(LEVEL_INFO, "-- TESTE:END --");
}
