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



//////////////////////////////////////////////////////////////////////
// Monitor
//////////////////////////////////////////////////////////////////////



Monitor::Monitor()
{
	lockVal = 0;
	current = 0;
}


void Monitor::Enter()
{
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
}


void Monitor::Exit() { if(--counter == 0) lockVal = 0; }



//////////////////////////////////////////////////////////////////////
// Thread
//////////////////////////////////////////////////////////////////////



Thread::Thread()
{
	Id      = 0;
	m_entry = NULL;
	m_data  = NULL;
}

void * Thread::ThreadEntry(void * arg)
{
	Thread * t = (Thread*)arg;
	if(t->m_entry != NULL) return (void *)t->m_entry(t->m_data);
	return NULL;
}

bool Thread::Run(ThreadStart_t entry)
{
	m_entry = entry;
	m_data  = NULL;
	return Run();
}

bool Thread::Run(ThreadStart_t entry, void * data)
{
	m_entry = entry;
	m_data = data;
	return Run();
}

bool Thread::Run()
{
	int s = pthread_create(&Id, NULL, &ThreadEntry, this);
	return (s == RET_OK);
}

void Thread::Join()
{
	pthread_join(Id, NULL);
}

