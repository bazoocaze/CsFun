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
#include "Logger.h"
#include "Debug.h"
#include "List.h"
#include "MemoryStream.h"
#include "Binary.h"
#include "Fd.h"
#include "IO.h"


class Item
{
};


List<Item> m_items;


int OutOffMemoryHandler(const char *modulo, const char *assunto, int tamanho)
{
	Logger::LogMsg(
		LEVEL_FATAL,
		"Memory allocation failure in %s/%s for %d bytes", 
		modulo, assunto, tamanho);
	return RET_OK;
}


int lastId = 0;


void TesteDnsForHost(const char * hostname)
{
	IPAddressList list;
	list = Dns::GetHostAddresses(hostname);
	printf("Dns result for %s\n", hostname);
	if(list.Count == 0)
	{
		printf(" Error: %d-%s\n", Dns::LastErr, Dns::GetLastErrMsg());
	}
	for(int n = 0; n < list.Count; n++)
	{
		printf(" IP[%d]: ", n);
		StdOut.print((IPAddress)list.Items[n]);
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
	IPAddressList list;
	list.Add(IPAddress::Any);
	list.Add(IPAddress::Loopback);
	list.Add(IPAddress("192.1.1.50"));
	printf("add ok: Items=%p\n\n", list.Items);
	for(int n = 0; n < list.Count; n++)
	{
		printf("Item[%d]: ", n);
		StdOut.print((IPAddress)list.Items[n]);
		printf("\n");
	}
	printf("Teste:end()\n");
}


void TesteListener()
{
int count = 3;
	StdOut.println("TesteListener:BEGIN");
	TcpListener listener;
	listener.Start(IPAddress::Any, 12345);
	if(!listener.IsListening())
	{
		return;
	}
	while(--count > 0)
	{
		// if(listener.Available())
		// {
			TcpClient client;
			StdOut.println("Waiting for client...");
			if(listener.Accept(client))
			{
				StdOut.println("Client connected...");

				FdWriter wr(client.GetFd());
				FdReader rd(client.GetFd());

				String linha;

				while(!(rd.IsEof() || rd.IsError()))
				{
					wr.println("Alfa");
					rd.ReadLine(linha, sizeof(linha));
					StdOut.printf("[Lido:%z]", &linha);
				}

				client.Close();

				StdOut.println("Client disconnected...");
			}
		// }
		delay(1000);
	}
	listener.Stop();
	StdOut.println("TesteListener:END");
}


void TesteCliente()
{
	TcpClient client;
	client.Connect("www.olvebra.com.br", 80);
	delay(1000);
	if(client.IsReady())
	{
		printf("Connected\n");
		Stream * s = client.GetStream();
		StreamWriter sw = StreamWriter(s);
		StreamReader sr = StreamReader(s);
		char linha[1000];
		sw.println("GET /index.html HTTP/1.0");
		sw.println("");
		delay(100);
		while(!(sr.IsEof() || sr.IsError()))
		{
			sr.ReadLine(linha, sizeof(linha));
			StdOut.printf("[Lido:%z]", &linha);
		}
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
	String s = sb.GetString();
	printf("sb=[%s]\n", s.c_str);
}


void TesteHexDump()
{
char msg[] = "the quick brown for jumps over the lazy dog";
	Debug::HexDump(StdOut, msg, sizeof(msg));
}


void TesteBinary()
{
MemoryStream stream;
BinaryWriter writer(&stream);
BinaryReader reader(&stream);
	writer.WriteInt32(42);
	writer.WriteString("olvebra industrial sa", 128);
	writer.WriteInt32(12345);

int val1;
String val2;
int val3;
	reader.TryReadInt32(&val1);
	reader.TryReadString(val2, 16);
	reader.TryReadInt32(&val3);
	reader.TryReadInt32(&val3);

	Console.printf("\nval1=[%d], val2=[%z], val3=[%d], eof=%d, error=%d\n", val1, &val2, val3, reader.Eof, reader.Error);
}


void TesteFileStream()
{
FileStream fs;
StreamWriter sw;

	if(!FileStream::Create("/etc/teste.txt", fs))
	{
		Logger::LogMsg(LEVEL_ERROR, "Falha criando o arquivo: %s", fs.GetLastErrMsg());
		return;
	}

	sw = &fs;
	

	sw.println("Teste");
	
	Logger::LogMsg(LEVEL_INFO, "sw.LastErr:%d-%z", sw.GetLastErr(), sw.GetLastErrMsg());
}


void TesteValPtr(int32_t val)
{	printf("[Val32=%d]\n", val); }

void TesteValPtr(int64_t val)
{	printf("[Val64=%ld]\n", val); }

void TesteValPtr(uint32_t val)
{	printf("[UVal32=%d]\n", val); }

void TesteValPtr(uint64_t val)
{	printf("[UVal64=%ld]\n", val); }


class StringDucker
{
public:
	int    Valor;
	String Nome;
	bool   Flag;
};


String m_MeuNome;
String m_MeuSobrenome;


void TesteString()
{
	StringDucker sd;
	sd.Valor = 12345;
	sd.Nome  = String(UTIL_MEM_STRDUP("teste"), true);
	sd.Flag  = true;
	
	sd.Nome.Debug();
}


int ReturnReadByte()
{
	uint8_t v1 = 255;
	int8_t v2 = v1;
	return v2;
}


void main_main()
{
	printf("Main/inicio\n");	
	Logger::LogLevel = LEVEL_VERBOSE;

	// TesteIPAddress();
	// TesteDns();
	TesteListener();
	// TesteCliente();
	// TesteSb();
	// TesteHexDump();
	// TesteBinary();
	// TesteFileStream();
	// TesteString();
	
	printf("Fim\n");	
}

extern void teste_main();
extern void outro_main();
extern void gpb_main();

int main(int argc, char **argv)
{
	main_main();
	// teste_main();
	// gpb_main();
	// outro_main();
	
	util_mem_debug();
}
