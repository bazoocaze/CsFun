/*
 * File....: Threading.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 08:57
 * Purpose.: Threading support
 */


#pragma once


#include <inttypes.h>
#include "Config.h"


#if defined HAVE_THREAD_pthreads
	#include <pthread.h>
#else
	typedef int pthread_t;
#endif


// Represents a monitor sincronization primitive.
class CMonitor
{
private:
	pthread_t current;
	int       lockVal;
	int       counter;

public:
	// Default constructor.
	CMonitor();
	
	/* Enter/lock the context for the current thread.
	 * If another thread has the context locked, then
	 * the current thread waits for the lock release
	 * and try to acquire it. */
	void Enter();
	
	/* Exit/unlock the context for the current thread. */
	void Exit();
};


/* Represents a thread.
 * Inherit from this class and implement the ExecuteThread method
 * to make a custom thread class.
 * Call Start() to start a new thread. */
class CThread
{
protected:
#if defined HAVE_THREAD_pthreads
	/* Internal entry point for the new thread. */
	static void * ThreadEntry_pthread(void * arg);
#endif

	/* This member is called as the entry point for the new created thread.
	 * Implement this member to customize the thread execution. */
	virtual void ExecuteThread() = 0;

public:
	/* Thread Id of the created thread.
	 * This value is updated after a call to Start(). */
	pthread_t Id;

	/* Default constructor for a standard thread. */
	CThread();

	/* Starts a new thread and run the *ExecuteThread()* entry point.
	 * Updates the field *Id* with the thread identifier. */
	bool Start();

	/* Join the running thread, waiting for it's termination.
	 * Can be called multiple times if needed. */
	void Join();

	/* Try to join the running thread, waiting for it's termination.
	 * Wait *timeoutMillis* milliseconds for the thread termination.
	 * Return true if the thread terminated during the timeout periodo,
	 * of false in case of timeout. */
	bool TryJoin(uint32_t timeoutMillis);

	/* Returns true/false if the thread is running. */
	bool IsRunning();
};
