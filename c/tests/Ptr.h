/*
 * File....: Ptr.h
 * Author..: Jose Ferreira
 * Date....: 2016/11/18 - 13:39
 * Purpose.: Automatic managment of mem and fd pointers.
 */

#pragma once


struct MemPtr_s;
struct FdPtr_s;
typedef struct MemPtr_s MemPtr_t;
typedef struct FdPtr_s FdPtr_t;


/* Automatic management of mem (malloc) pointers by reference counting.
   The data is automatic freed via free() call when needed. */ 
class MemPtr
{
private:
	MemPtr_t * ref;

protected:
	void add();
	void release();

public:
	// // Cached read-only copy of the managed pointer
	// void * Data;
	
	// Default empty constructor
	MemPtr();

	// Constructor for automatic management of the data pointer
	MemPtr(void * data); 

	// Copy constructor
	MemPtr(const MemPtr& other);

	// Copy assignment
	MemPtr& operator=(const MemPtr& other);

	// Default destructor
	~MemPtr();

	// Clear/release the managed data, freeing the buffer if the reference count reaches 0
	void Clear();

	// Release the current managed data and begin management for the new data
	void Set(void * data);

	bool Realloc(int newSize);

	void ChangeTo(void * data);

	void * Get() const;
};



/* Automatic management of integer file descriptor (FD) by reference counting.
   The descriptor is automatic closed (via close() call) when needed. */ 
class FdPtr
{
private:
	FdPtr_t * ref;

protected:
	void add();
	void release();

public:
	// Cached read-only copy of the managed pointer
	int  Fd;

	// Default empty constructor
	FdPtr();
	// Constructor for automatic management of the fd descriptor
	FdPtr(int fd);
	// Copy constructor
	FdPtr(const FdPtr& other);

	// Copy assignment
	FdPtr& operator=(const FdPtr& other);

	// Automatic return Fd on conversion to int
	operator int() const { return Fd; }

	// Default destructor
	~FdPtr();

	// Release the managed fd, closing the fd if the reference count reaches 0
	void Close();

	// Release the current fd and begin management for the new fd
	void Set(int fd);
};

