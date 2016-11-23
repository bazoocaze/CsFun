/*
 * File....: Ptr.h
 * Author..: Jose Ferreira
 * Date....: 2016/11/18 - 13:39
 * Purpose.: Automatic managment of dynamic memory and fd pointers.
 */


#pragma once


#include <inttypes.h>


struct MemPtr_s;
struct FdPtr_s;
typedef struct MemPtr_s MemPtr_t;
typedef struct FdPtr_s FdPtr_t;


/* Automatic management of dynamic memory (malloc/realloc) pointers using reference counting.
 * The data is automatic freed via free() call when needed. */ 
class MemPtr
{
private:
	// Pointer to the internal data pointer and reference counter.
	MemPtr_t * ref;

protected:
	// Increments the reference counter for the data pointer.
	void AddRef();
	
	/* Decrements the reference counter for the data pointer.
	 * Free the data pointer if the counter reaches 0 references. */
	void ReleaseRef();

public:
	// Default empty constructor
	MemPtr();

	// Constructor for automatic management of the dynamic data pointer *data*.
	MemPtr(void * data); 

	// Copy constructor. Copy and increments the reference.
	MemPtr(const MemPtr& other);

	// Copy assignment. Release, copy and increments the reference.
	MemPtr& operator=(const MemPtr& other);

	// Default destructor. Release the reference.
	~MemPtr();

	/* Clear/release the managed data.
	 * Free the buffer if the reference count reaches 0 */
	void Clear();

	// Release the current managed data and begin management for the new data.
	void Set(void * data);

	// Resize the managed data pointer.
	bool Resize(int newSize);

	// Fill the data buffer.
	void Memset(uint8_t val, int size);

	/* Changes the current data pointer reference to the new reference,
	 * without altering the reference count. (DANGER)*/	
	void ChangeTo(void * data);

	/* Return the managed data pointer.
	 * The pointer is valid until freed or the next resize (Resize). */
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
	// Cached read-only copy of the managed file descriptor.
	int  Fd;

	// Default empty constructor.
	FdPtr();
	
	// Constructor for automatic management of the fd descriptor.
	FdPtr(int fd);
	
	// Copy constructor.
	FdPtr(const FdPtr& other);

	// Copy assignment.
	FdPtr& operator=(const FdPtr& other);

	// Automatic return Fd on conversion to int.
	operator int() const { return Fd; }

	// Default destructor.
	~FdPtr();

	// Release the managed fd, closing the fd if the reference count reaches 0.
	void Close();

	// Release the current fd and begin management for the new fd.
	void Set(int fd);
};

