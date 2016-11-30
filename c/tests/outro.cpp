/*
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unistd.h>
#include <errno.h>


#include "Stream.h"
#include "Binary.h"
#include "Ptr.h"
#include "Text.h"
#include "StringBuilder.h"
#include "Fd.h"
#include "IO.h"
 *
 * */


#include <stdio.h>
#include <stdlib.h>
#include <string.h>




#include "IO.h"
#include "Threading.h"
#include "TcpListener.h"



class MyThread : public CThread
{
protected:
	CTcpListener listener;

public:
	bool CancelRequested;

	void ExecuteThread()
	{
		StdOut.println("[ServidorThread:iniciado");

		listener.Start(12345);

		while(listener.IsListening() && !CancelRequested)
		{
			CTcpClient client;
			StdOut.println("[ServidorThread:Accept");
			if(listener.TryAccept(client, 1000))
			{
				StdOut.println("[ServidorThread:client accepted");
				CStreamWriter sw = CStreamWriter(client.GetStream());
				CStreamReader sr = CStreamReader(client.GetStream());
				sw.println("Bem vindo");
				while(!(sr.IsEof() || sr.IsError()))
				{
					sw.println("Digite um comando: ");
					CString ret;
					if(sr.ReadLine(ret, 1024))
					{
						StdOut.printf("Comando digitado:[%r]", &ret);
					}
				}
			} else {
				StdOut.println("[ServidorThread:no client accepted");
			}
		}

		listener.Stop();

		StdOut.println("[ServidorThread:finalizado");
	}

	void Stop()
	{
		CancelRequested = true;
		listener.Stop();
	}
};


MyThread myThread;


void TestePrintf()
{
	char ptr[128];

	int      vsi1 = 12345;
	int      vsi2 = -12345;
	int      vsi3 =  2012345678;
	int      vsi4 = -2012345678;
	uint32_t vui1 = 12345;
	uint32_t vui2 = -1;
	uint32_t vui3 = 2012345678;
	uint32_t vui4 = 4012345678;

	printf       ("p ptr = %p\n", ptr);
	StdOut.printf("s ptr = %p\n", ptr);

	printf("p vsi1 = %d\n", vsi1);
	printf("p vsi2 = %d\n", vsi2);
	printf("p vsi3 = %d\n", vsi3);
	printf("p vsi4 = %d\n", vsi4);

	printf("p vui1 = %u\n", vui1);
	printf("p vui2 = %u\n", vui2);
	printf("p vui3 = %u\n", vui3);
	printf("p vui4 = %u\n\n", vui4);

	StdOut.printf("s vsi1 = %d\n", vsi1);
	StdOut.printf("s vsi2 = %d\n", vsi2);
	StdOut.printf("s vsi3 = %d\n", vsi3);
	StdOut.printf("s vsi4 = %d\n", vsi4);

	StdOut.printf("s vui1 = %u\n", vui1);
	StdOut.printf("s vui2 = %u\n", vui2);
	StdOut.printf("s vui3 = %u\n", vui3);
	StdOut.printf("s vui4 = %u\n\n", vui4);

	int64_t  vsl1 = 12345;
	int64_t  vsl2 = -12345;
	int64_t  vsl3 =  2012345678;
	int64_t  vsl4 = -2012345678000000;

	uint64_t vul1 = 12345;
	uint64_t vul2 = -1;
	uint64_t vul3 = 2012345678000000;
	uint64_t vul4 = 0xF0FFFFFFFFFFFFF0;

	printf("p vsl1 = %ld\n", vsl1);
	printf("p vsl2 = %ld\n", vsl2);
	printf("p vsl3 = %ld\n", vsl3);
	printf("p vsl4 = %ld\n", vsl4);

	printf("p vul1 = %lu\n", vul1);
	printf("p vul2 = %lu\n", vul2);
	printf("p vul3 = %lu\n", vul3);
	printf("p vul4 = %lu\n\n", vul4);

	StdOut.printf("s vsl1 = %ld\n", vsl1);
	StdOut.printf("s vsl2 = %ld\n", vsl2);
	StdOut.printf("s vsl3 = %ld\n", vsl3);
	StdOut.printf("s vsl4 = %ld\n", vsl4);

	StdOut.printf("s vul1 = %lu\n", vul1);
	StdOut.printf("s vul2 = %lu\n", vul2);
	StdOut.printf("s vul3 = %lu\n", vul3);
	StdOut.printf("s vul4 = %lu\n\n", vul4);

	int ret = StdOut.printf("%lb", vul4);
	StdOut.printf("\nret = %d\n", ret);

}


void outro_main()
{
	TestePrintf();
	return;

	myThread.Start();
	delay(60000);
	myThread.Stop();
	StdOut.println("outro:begin join");
	myThread.Join();
	StdOut.println("outro:end join");
	StdOut.println("outro:End");
}

