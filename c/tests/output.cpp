#include <stdio.h>

#include "Config.h"
#include "Logger.h"
#include "TcpListener.h"
#include "Util.h"


#include <sys/socket.h>
#include <arpa/inet.h>

#include "IPAddress.h"
#include "Dns.h"
#include "TcpListener.h"
#include "TcpClient.h"
#include "StringBuilder.h"
#include "Print.h"
#include "Logger.h"
#include "Debug.h"


class CMyStdOut : public Print
{
public:
   CMyStdOut() {

   }

   virtual size_t Write(uint8_t c) {
      putchar(c);
      return 1;
   }
};


CMyStdOut MyStdOut;


int OutOffMemoryHandler(const char *modulo, const char *assunto, int tamanho)
{
	Logger::LogMsg(
		LEVEL_FATAL,
		"Memory allocation failure in %s/%s for %d bytes", 
		modulo, assunto, tamanho);
	return RET_OK;
}


int lastId = 0;


class Teste
{
public:
	int Id;
	int Extra;

	Teste()
	{
		Id = ++lastId;
		Extra = 0;
		printf("Teste:ctor %d %d\n", Id, Extra);
	}

	~Teste()
	{
		printf("Teste:dtor %d %d\n", Id, Extra);
	}
};


bool Teste2(Teste& teste)
{
	printf("Teste2: begin\n");
	printf("Teste2: end\n");
	return true;
}


void Teste1()
{
	printf("Teste1: begin\n");
	Teste teste;
	Teste2(teste);
	printf("Teste1: end\n");
}


void TesteDnsForHost(const char * hostname)
{
	IPAddressList list;
	list = Dns::GetHostAddresses(hostname);
	printf("Dns result for %s\n", hostname);
	if(list.Count == 0)
	{
		printf(" Error: %d-%s\n", Dns::LastError, Dns::GetMsgError());
	}
	for(int n = 0; n < list.Count; n++)
	{
		printf(" IP[%d]: ", n);
		MyStdOut.print((IPAddress)list.Items[n]);
		printf("\n");
	}
}


void TesteDns()
{
	TesteDnsForHost("192.1.1.50");
	TesteDnsForHost("dinfo01");
	TesteDnsForHost("www.bradesco.com.br");
	TesteDnsForHost("www.terra.com.br");
	TesteDnsForHost("sntolvebra");
	TesteDnsForHost("192.1.1.60");
	TesteDnsForHost("ducker");
}


IPAddressList GetList()
{
	IPAddressList list;
	list.Add(IPAddress::Any);
	list.Add(IPAddress::Loopback);
	list.Add(IPAddress("192.1.1.50"));
	return list;
}


void TesteIPAddress()
{
	printf("Teste:begin()\n");
	IPAddress addr = IPAddress("192.1.1.50");
	IPEndPoint localEP = IPEndPoint("192.1.1.60", 12345);
	// MyStdOut.println(addr);
	// MyStdOut.println(localEP);
	// MyStdOut.println(IPAddress::Any);
	// MyStdOut.println(IPAddress::Loopback);
	// MyStdOut.println(IPAddress::IPv6Loopback);
	// IPAddressList list = GetList();
	IPAddressList list;
	list.Add(IPAddress::Any);
	list.Add(IPAddress::Loopback);
	list.Add(IPAddress("192.1.1.50"));
	printf("add ok: Items=%p\n\n", list.Items);
	for(int n = 0; n < list.Count; n++)
	{
		printf("Item[%d]: ", n);
		MyStdOut.print((IPAddress)list.Items[n]);
		printf("\n");
	}
	printf("Teste:end()\n");
}


void TesteListener()
{
int count = 10;
	TcpListener listener;
	listener.Start(IPAddress::Any, 12345);
	while(--count > 0)
	{
		if(listener.Available())
		{
			TcpClient client;
			if(listener.Accept(client))
			{
				client.println("Alfa");
				client.println("Bravo");
				client.Close();
			}
		}
		delay(1000);
	}
	listener.Stop();
}


void TesteCliente()
{
	TcpClient client;
	client.Connect("sntolvebra", 8080);
	delay(1000);
	if(client.IsReady())
	{
		printf("Connected\n");
		char linha[1000];
		client.println("GET /index.html HTTP/1.0");
		client.println("");
		delay(100);
		int lidos = client.ReadBytesUntil('\n', linha, sizeof(linha));
		linha[lidos] = 0;
		printf("Lidos:[%s]\n", linha);
	}
	else
	{
		printf("Failed do connect: %s\n", ConnectionStateStr(client.State()));
	}
	client.Close();
}


void TesteSb()
{
	IPEndPoint localEP = IPEndPoint("192.1.1.60", 12345);
	StringBuilder sb;
	sb.print(localEP);
	printf("sb=[%s]\n", sb.GetStr());
}


void TesteHexDump()
{
char msg[] = "the quick brown for jumps over the lazy dog";
	Debug::HexDump(MyStdOut, msg, sizeof(msg));
}


Print* p;


int main(int argc, char **argv)
{
	printf("Inicio\n");	
	Logger::LogLevel = LEVEL_VERBOSE;
	
	p = &MyStdOut;

	TesteIPAddress();
	TesteDns();
	// TesteListener();
	// TesteCliente();
	TesteSb();
	// TesteHexDump();

	Socket sock = 13;
	
	printf("Fim\n");	
	return 0;
}
