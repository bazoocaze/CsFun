#include "Config.h"
#include "Logger.h"
#include "TcpListener.h"
#include "Util.h"
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


int OutOffMemoryHandler(const char *modulo, const char *assunto, int tamanho)
{
	CLogger::LogMsg(
		LEVEL_FATAL,
		"Memory allocation failure in %s/%s for %d bytes", 
		modulo, assunto, tamanho);
	return RET_OK;
}


extern void teste_main();
extern void outro_main();
extern void gpb_main();

int main(int argc, char **argv)
{
	static_cast<void>(argc);
	static_cast<void>(argv);

	// main_main();
	teste_main();
	// gpb_main();
	// outro_main();
	
	util_mem_debug();

	char linha[10];
	StdIn.ReadLine(linha, 1);
}
