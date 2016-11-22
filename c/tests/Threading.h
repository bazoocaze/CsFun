/*
 * File....: Threading.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 08:57
 * Purpose.: Threading support
 */


#pragma once


#include <pthread.h>


// Represents a monitor sincronization primitive.
class Monitor
{
private:
	pthread_t current;
	int       lockVal;
	int       counter;

public:
	// Default constructor.
	Monitor();
	
	/* Enter/lock the context for the current thread.
	 * If another thread has the context locked, then
	 * the current thread waits for the lock release
	 * and try to acquire it. */
	void Enter();
	
	/* Exit/unlock the context for the current thread. */
	void Exit();
};


typedef void * (*ThreadStart_t)(void *);


// Represents a thread.
class Thread
{
protected:
	// User entry point for the thread.
	ThreadStart_t  m_entry;
	
	// User data for the thread.
	void *         m_data;

	// Internal entry point for the thread.
	static void * ThreadEntry(void * arg);

	// Start a new thread.
	bool Run();

public:
	// Thread Id of the created thread.
	pthread_t Id;

	// Default constructor for a standard thread.
	Thread();

	// Starts a new thread running the entry point informed.
	bool Run(ThreadStart_t entry);
	
	/* Start a new thread running the entry point informed
	 * and passing the data as argument. */
	bool Run(ThreadStart_t entry, void * data);

	// Join the running thread / waits for it's finalization.
	void Join();
};
