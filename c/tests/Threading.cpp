/*
 * File....: Threading.cpp
 * Author..: Jose Ferreira
 * Data....: 2016-11-22 09:02
 * Purpose.: Threading support
 */



#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <string.h>
#include <signal.h>


#include "Threading.h"
#include "Util.h"
#include "IO.h"
#include "Config.h"



//////////////////////////////////////////////////////////////////////
// Monitor
//////////////////////////////////////////////////////////////////////



CMonitor::CMonitor()
{
	current = 0;
	counter = 0;
}


void CMonitor::Enter()
{
#if HAVE_THREAD_pthreads == 1

	pthread_t id = pthread_self();

	if(pthread_equal(id, current))
	{
		counter++;
		return;
	}

	while(!__sync_bool_compare_and_swap(&current, 0, id))
	{
		delay(1);
	}

	counter = 1;

#endif
}


void CMonitor::Exit()
{
#if HAVE_THREAD_pthreads == 1
	if(--counter == 0)
		current = 0;
#endif
}



//////////////////////////////////////////////////////////////////////
// Thread
//////////////////////////////////////////////////////////////////////



CThread::CThread()
{
	Id = 0;
}

#if HAVE_THREAD_pthreads == 1
void * CThread::ThreadEntry_pthread(void * arg)
{
	CThread * t = (CThread*)arg;
	t->Running = true;
	t->ExecuteThread();
	t->Running = false;
	return NULL;
}
#endif

bool CThread::Start()
{
#if HAVE_THREAD_pthreads == 1
	pthread_attr_t attr;
	pthread_attr_init(&attr);
	pthread_attr_setdetachstate(&attr, PTHREAD_CREATE_DETACHED);
	int ret = pthread_create(&Id, &attr, &ThreadEntry_pthread, this);
	pthread_attr_destroy(&attr);
	return (ret == RET_OK);
#else
	return false;
#endif
}


bool CThread::IsRunning()
{
#if HAVE_THREAD_pthreads == 1
	/*
	int policy;
	struct sched_param param;
	int ret = pthread_getschedparam(Id, &policy, &param);
	return (ret == RET_OK); */

	/*
	StdOut.print("[pthread_kill]\n");
	int ret = pthread_kill(Id, 0);
	return (ret == RET_OK); */
#endif

	return Running;
}


void CThread::Join()
{
	while(IsRunning()) {
		delay(1);
	}
}


bool CThread::TryJoin(uint32_t timeoutMillis)
{
	uint64_t timeout = millis() + timeoutMillis;
	while(millis() < timeout)
	{
		if(!IsRunning())
			return true;
		delay(1);
	}
	return false;
}
