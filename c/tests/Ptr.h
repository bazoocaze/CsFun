/*
 * File....: Ptr.h
 * Author..: Jose Ferreira
 * Date....: 2016/11/18 - 13:39
 * Purpose.: Automatic managment of dynamic memory and fd pointers.
 */


#pragma once


#include <inttypes.h>


// struct MemPtr_s;
// struct FdPtr_s;
// typedef struct MemPtr_s MemPtr_t;
// typedef struct FdPtr_s FdPtr_t;



#define FD_TYPE_NORMAL 0
#define FD_TYPE_SOCKET 1



typedef struct {
	uint32_t count;
	void *   data;
} ref_ptr_t;


/* This abstract class provides reference count management of a data pointer.
 * You must implement the ReleaseData() member to release the resources
 * associated with the data pointer when the referenc count reaches zero. */
class CRefPtr
{
private:
	ref_ptr_t * m_ref;

protected:
	void AddRef();
	void ReleaseRef();
	void SetDataPtr(void * data);
	void * GetDataPtr() const;
	virtual void ReleaseData(void *) = 0;

public:

	// Default empty reference constructor.
	CRefPtr();

	// Copy constructor.
	CRefPtr(const CRefPtr& other);

	// Copy assignment.
	CRefPtr& operator=(const CRefPtr& other);

	// Default destructor.
	virtual ~CRefPtr();

	// Release the managed fd, closing the fd if the reference count reaches 0.
	void Close();

	void Debug();
};



/* Automatic management of dynamic memory (malloc/realloc) pointers using reference counting.
 * The data is automatic freed via free() call when needed. */ 
class CMemPtr : CRefPtr
{
private:
	void ReleaseData(void *);

	/* Changes the current data pointer reference to the new reference,
	 * without altering the reference count. (DANGER)*/
	void ChangeTo(void * data);

public:
	// Default empty constructor
	CMemPtr();

	// Constructor for automatic management of the dynamic data pointer *data*.
	explicit CMemPtr(void * data);

	// Default destructor. Release the reference.
	virtual ~CMemPtr();

	/* Clear/release the managed data.
	 * Free the buffer if the reference count reaches 0 */
	void Clear();

	// Release the current managed data and begin management for the new data.
	void Set(void * data);

	// Resize the managed data pointer.
	bool Resize(int newSize);

	// Fill the data buffer.
	void Memset(uint8_t val, int size);

	/* Return the managed data pointer.
	 * The pointer is valid until freed or the next resize (Resize). */
	void * Get() const;
};



/* Automatic management of integer file descriptor (FD) by reference counting.
   The descriptor is automatic closed (via close() call) when needed. */ 
class CFdPtr : CRefPtr
{
private:
	virtual void ReleaseData(void *);

public:
	// Cached read-only copy of the managed file descriptor.
	int  Fd;

	// Default empty constructor.
	CFdPtr();
	
	// Constructor for automatic management of the fd descriptor.
	CFdPtr(int fd);

	// Automatic return Fd on conversion to int.
	operator int() const { return Fd; }

	// Default destructor. Releases the fd.
	~CFdPtr();

	// Release the managed fd, closing the fd if the reference count reaches 0.
	void Close();

	// Release the current fd and begin management for the new fd.
	void Set(int fd);

	void Debug();
};
