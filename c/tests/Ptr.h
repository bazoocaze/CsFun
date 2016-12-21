/*
 * File....: Ptr.h
 * Author..: Jose Ferreira
 * Date....: 2016/11/18 - 13:39
 * Purpose.: Automatic managment of dynamic memory and fd pointers.
 */


#pragma once


#include <inttypes.h>


typedef struct {
	uint32_t count;
	void *   data;
} ref_ptr_t;


/* This abstract class provides reference count management of a data pointer.
 * You must implement the ReleaseData() member to release the resources
 * associated with the data pointer when the reference count reaches zero. */
class CRefPtr
{
private:
	ref_ptr_t * m_ref;
	void AddRef();

protected:

	/* Release the current reference and set a new reference to the
	 * data pointer informed. */
	void SetDataPtr(void * data);

	/* Returns the managed data pointer currently referenced.
	 * Returns NULL if there is no current reference. */
	void * GetDataPtr() const;

	/* Change the data pointer reference without affecting
	 * the reference count. (DANGER) */
	void ChangeDataPtrTo(void * data);

	/* Release the resources associated with the
	 * managed data pointer. */
	virtual void ReleaseData(void *) = 0;

	/* Release the reference to the managed data pointer.
	 * Releases the associated resources of the data poitner
	 * if the reference count reaches 0. */
	void ReleaseRef();

	void Debug();

public:

	/* Default constructor for an empty/NULL reference. */
	CRefPtr();

	/* Copy constructor.
	 * Create a new reference to the managed data pointer
	 * on the *other* reference. */
	CRefPtr(const CRefPtr& other);

	/* Copy assignment.
	 * Release the current reference and create a new
	 * reference to the managed data pointer on the
	 * *other* reference. */
	CRefPtr& operator=(const CRefPtr& other);

	/* Default destructor.
	 * You must call Close() on the devired class
	 * destructor to automatically release the
	 * managed pointer reference. */
	virtual ~CRefPtr();
};


/* Automatic management of dynamic memory (malloc/realloc)
 * pointers using reference counting.
 * The data is automatic freed via free() call when needed. */ 
class CMemPtr : CRefPtr
{
private:
	/* Deallocate the dynamic memory for the data pointer. */
	void ReleaseData(void *);

	/* Changes the current data pointer reference to the new reference,
	 * without altering the reference count. (DANGER)*/
	void ChangeTo(void * data);

public:
	/* Default constructor for an empty data pointer. */
	CMemPtr();

	/* Constructor for automatic management of the dynamic data pointer *data*. */
	explicit CMemPtr(void * data);

	/* Default destructor. Release the reference. */
	virtual ~CMemPtr();

	/* Release the reference to the managed data.
	 * Free the buffer if the reference count reaches 0. */
	void Clear();

	/* Release the reference to the current managed data
	 * and create a new managed reference to the *data*. */
	void Set(void * data);

	/* Resize the buffer.
	 * Returns true/false on success.
	 * On failure, the current buffer is not modified. */
	bool Resize(int newSize);

	/* Fill the data buffer. */
	void Memset(uint8_t val, int size);

	/* Return the managed data pointer.
	 * The pointer is valid until freed or the next resize (Resize).
	 * Return NULL if there is no current data pointer. */
	void * Get() const;
};
