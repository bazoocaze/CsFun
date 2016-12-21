#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "IO.h"
#include "Threading.h"
#include "TcpListener.h"
#include "FdSelect.h"


#define TESTLOCK(l, c) do{ StdErr.print("[BEG]"); c(); StdErr.print("[END]"); }while(false);


CMonitor myLock;
int valor = 0;
bool m_abort = false;


void rotina2()
{
	LOCK(myLock, []{
		if(valor != 0) {
			m_abort = true;
			return;
		}
		valor = 1;
		delay(10);
		valor = 0;
	});
}


class MonTest : public CThread
{
protected:
	void ExecuteThread()
	{
		StdOut.print("[Thread begin]\n");
		delay(10);

		for(int n = 0; n < 100; n ++)
		{
			if(m_abort) break;
				rotina2();
		}

		StdOut.print("[Thread end]\n");
	}
};


MonTest t1;
MonTest t2;
MonTest t3;
MonTest t4;
MonTest t5;




void outro_main()
{


	StdOut.print("Iniciado\n");

	t1.Start();
	t2.Start();
	t3.Start();
	t4.Start();
	t5.Start();

	StdOut.print("Begin Join\n");

	t1.Join();
	t2.Join();
	t3.Join();
	t4.Join();
	t5.Join();

	StdOut.printf("Abort = %d\n", (int)m_abort);

	StdOut.print("\nFinalizado\n");
}

