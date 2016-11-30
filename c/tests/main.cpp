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


CList<Item> m_items;


int OutOffMemoryHandler(const char *modulo, const char *assunto, int tamanho)
{
	CLogger::LogMsg(
		LEVEL_FATAL,
		"Memory allocation failure in %s/%s for %d bytes", 
		modulo, assunto, tamanho);
	return RET_OK;
}


int lastId = 0;


void TesteDnsForHost(const char * hostname)
{
	CIPAddressList list;
	list = CDns::GetHostAddresses(hostname);
	printf("Dns result for %s\n", hostname);
	if(list.Count == 0)
	{
		printf(" Error: %d-%s\n", CDns::LastErr, CDns::GetLastErrMsg());
	}
	for(int n = 0; n < list.Count; n++)
	{
		printf(" IP[%d]: ", n);
		StdOut.print((CIPAddress)list.Items[n]);
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


CIPAddressList GetList()
{
	CIPAddressList list;
	list.Add(CIPAddress::Any);
	list.Add(CIPAddress::Loopback);
	list.Add(CIPAddress("192.1.1.50"));
	return list;
}


void TesteIPAddress()
{
	printf("Teste:begin()\n");
	CIPAddressList list;
	list.Add(CIPAddress::Any);
	list.Add(CIPAddress::Loopback);
	list.Add(CIPAddress("192.1.1.50"));
	printf("add ok: Items=%p\n\n", list.Items);
	for(int n = 0; n < list.Count; n++)
	{
		printf("Item[%d]: ", n);
		StdOut.print((CIPAddress)list.Items[n]);
		printf("\n");
	}
	printf("Teste:end()\n");
}


void TesteListener()
{
int count = 3;
	StdOut.println("TesteListener:BEGIN");
	CTcpListener listener;
	listener.Start(CIPAddress::Any, 12345);
	if(!listener.IsListening())
	{
		return;
	}
	while(--count > 0)
	{
		// if(listener.Available())
		// {
			CTcpClient client;
			StdOut.println("Waiting for client...");
			if(listener.TryAccept(client, 10000))
			{
				StdOut.println("Client connected...");

				CFdWriter wr(client.GetFd());
				CFdReader rd(client.GetFd());

				CString linha;

				while(!(rd.IsEof() || rd.IsError()))
				{
					wr.println("Alfa");
					rd.ReadLine(linha, sizeof(linha));
					StdOut.printf("[Lido:%P]", &linha);
				}

				client.Close();

				StdOut.println("Client disconnected...");
			} else {
				StdOut.println("Failed to accept client");
			}
		delay(1000);
	}
	listener.Stop();
	StdOut.println("TesteListener:END");
}


void TesteCliente()
{
	CTcpClient client;
	client.Connect("www.olvebra.com.br", 80);
	delay(1000);
	if(client.IsReady())
	{
		printf("Connected\n");
		CStream * s = client.GetStream();
		CStreamWriter sw = CStreamWriter(s);
		CStreamReader sr = CStreamReader(s);
		sw.println("GET /index.html HTTP/1.0");
		sw.println("");
		delay(100);
		while(!(sr.IsEof() || sr.IsError()))
		{
			CString linha;
			sr.ReadLine(linha, 1024);
			StdOut.printf("[Lido:%P]", &linha);
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
	CIPEndPoint localEP = CIPEndPoint("192.1.1.60", 12345);
	CStringBuilder sb;
	sb.print(localEP);
	CString s = sb.GetString();
	printf("sb=[%s]\n", s.c_str);
}


void TesteHexDump()
{
char msg[] = "the quick brown for jumps over the lazy dog";
	CDebug::HexDump(StdOut, msg, sizeof(msg));
}


void TesteBinary()
{
CMemoryStream stream;
CBinaryWriter writer(&stream);
CBinaryReader reader(&stream);
	writer.WriteInt32(42);
	writer.WriteString("olvebra industrial sa", 128);
	writer.WriteInt32(12345);

int val1;
CString val2;
int val3;
	reader.TryReadInt32(&val1);
	reader.TryReadString(val2, 16);
	reader.TryReadInt32(&val3);
	reader.TryReadInt32(&val3);

	StdOut.printf("\nval1=[%d], val2=[%P], val3=[%d], eof=%d, error=%d\n", val1, &val2, val3, reader.Eof, reader.Error);
}


void TesteFileStream()
{
CFileStream fs;
CStreamWriter sw;

	if(!CFileStream::Create("/etc/teste.txt", fs))
	{
		CLogger::LogMsg(LEVEL_ERROR, "Falha criando o arquivo: %s", fs.GetLastErrMsg());
		return;
	}

	sw = CStreamWriter(&fs);
	

	sw.println("Teste");
	
	CLogger::LogMsg(LEVEL_INFO, "sw.LastErr:%d-%P", sw.GetLastErr(), sw.GetLastErrMsg());
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
	CString Nome;
	bool   Flag;
};


CString m_MeuNome;
CString m_MeuSobrenome;


void TesteString()
{
	StringDucker sd;
	sd.Valor = 12345;
	sd.Nome  = CString(UTIL_MEM_STRDUP("teste"), true);
	sd.Flag  = true;
	
	sd.Nome.CDebug();
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
	CLogger::LogLevel = LEVEL_VERBOSE;

	// TesteIPAddress();
	// TesteDns();
	// TesteListener();
	TesteCliente();
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
	// main_main();
	teste_main();
	// gpb_main();
	// outro_main();
	
	util_mem_debug();
}
