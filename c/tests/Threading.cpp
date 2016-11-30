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


#include "Threading.h"
#include "Util.h"
#include "IO.h"



//////////////////////////////////////////////////////////////////////
// Monitor
//////////////////////////////////////////////////////////////////////



CMonitor::CMonitor()
{
	lockVal = 0;
	current = 0;
	counter = 0;
}


void CMonitor::Enter()
{
#if defined HAVE_THREAD_pthreads

pthread_t id = pthread_self();

	if(id == current) {
		counter++;
		return;
	}

	while(!__sync_bool_compare_and_swap(&lockVal, 0, 1))
	{
		delay(1);
	}

	current = id;
	counter = 1;

#endif
}


void CMonitor::Exit()
{
#if defined HAVE_THREAD_pthreads
	if(--counter == 0)
		lockVal = 0;
#endif
}



//////////////////////////////////////////////////////////////////////
// Thread
//////////////////////////////////////////////////////////////////////



CThread::CThread()
{
	Id = 0;
}

#if defined HAVE_THREAD_pthreads
void * CThread::ThreadEntry_pthread(void * arg)
{
	CThread * t = (CThread*)arg;
	t->ExecuteThread();
	return NULL;
}
#endif

bool CThread::Start()
{
#if defined HAVE_THREAD_pthreads
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
#if defined HAVE_THREAD_pthreads
	int policy;
	struct sched_param param;
	int ret = pthread_getschedparam(Id, &policy, &param);
	return (ret == RET_OK);
#endif
	return false;
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
