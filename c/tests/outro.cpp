#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "IO.h"
#include "Threading.h"
#include "TcpListener.h"
#include "FdSelect.h"


CMonitor myLock;
int valor = 0;
bool m_abort = false;


void rotina()
{
	myLock.Enter();
	if(valor != 0) {
		m_abort = true;
		return;
	}
	valor = 1;
	delay(10);
	valor = 0;
	myLock.Exit();
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
			// rotina();
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

	StdOut.print("Join t1\n"); t1.Join();
	StdOut.print("Join t2\n"); t2.Join();
	StdOut.print("Join t3\n"); t3.Join();
	StdOut.print("Join t4\n"); t4.Join();
	StdOut.print("Join t5\n"); t5.Join();

	StdOut.print("\nFinalizado\n");
}

