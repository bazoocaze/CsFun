/*
 * File.....: ByteBuffer.h
 * Author...: José Ferreira
 * Date.....: 10/08/2015 - 15:53
 * Purpose..: Read/write byte buffer
 */


#pragma once

#include <inttypes.h>

#include "Text.h"
#include "Ptr.h"


class String;


// Represents a pointer reference to data or memory buffer.
class BytePtr
{
private:
	// Internal memory pointer for dynamic memory.
	MemPtr m_ptr;

public:
	// Cached read-only data pointer.
	uint8_t * Ptr;
	
	// Current size of the data on the buffer.
	int       Size;

	// Default constructor to and empty data buffer.
	BytePtr();
	
	// Constructor to reference the *ptr* and *size* buffer.
	BytePtr(uint8_t * ptr, int size);
	
	// Constructor to reference the *ptr*, *offset* and *size* buffer.
	BytePtr(uint8_t * ptr, int offset, int size);
	
	// Constructor to reference dynamic memory.
	BytePtr(MemPtr memPtr, int offset, int size);
	
	// Constructor to reference dynamic memory using *ptr* and *size* for the buffer data.
	BytePtr(MemPtr memPtr, uint8_t * ptr, int size);
	
	// Constructor to reference dynamic memory using *ptr*, *offset* and *size* for the buffer data.
	BytePtr(MemPtr memPtr, uint8_t * ptr, int offset, int size);
};


// Represents a dynamic growing writable/readable byte buffer.
class ByteBuffer
{
private:
	/* Internal memory pointer for the dynamic buffer memory */
	MemPtr m_ptr;
	
	// Size of the data on the buffer
	int    m_Length;
	
	// Dynamic memory size of the whole buffer
	int    m_Capacity;
	
	// Read cursor position on the buffer
	int    m_ReadPos;
	
	// Write cursor position on the buffer
	int    m_WritePos;

	// Clear and initialize internal structures
	void Init();
	
	// Resize internal buffer dynamic memory
	int  Resize(int size);

public:

	// Default constructor for an empty buffer.
	ByteBuffer();
	
	// Constructor for a buffer with *initialCapacity* initial size.
	ByteBuffer(int initialCapacity);

	/* Returns the size of the readable data on the buffer. */
	int  Length() const;
	
	/* Returns the total size of the dynamic allocated memory. */
	int  Capacity() const;

	char * Ptr() const;

	const char * GetStr();
	String       GetString();
	MemPtr       GetMemPtr();

	/* Clear the buffer, discarding the stored data. */
	void Clear();

	/* Write data to the buffer. */
	int  Write(char c);
	
	/* Write data to the buffer. */
	int  Write(const void * source, int index, int size);
	
	/* Lock an area of the buffer for writting.
	 * Returns the number of bytes locked and the pointer to the area.
	 * The area is valid until the next change on the buffer. */
	int     LockWrite(int size, void ** dest);
	
	/* Lock an area of the buffer for writting.
	 * Returns pointer to the locked area.
	 * The area is valid until the next change on the buffer. */
	BytePtr LockWrite(int size);
	
	// Updates the write cursor to reflect a lock write of *confirmBytes* bytes.
	void    ConfirmWrite(int confirmBytes);

	/* Read a byte from the buffer. Return the byte read, or INT_EOF if the buffer is empty. */
	int  ReadByte();
	
	/* Read a block of maximum *size* bytes of data from the buffer.
	 * Returns the number of bytes read. */
	int  Read(void * dest, int index, int size);
	
	/* Lock an area of the buffer for reading.
	 * Returns the number of bytes locked for reading.
	 * The area is valid until the next change on the buffer (read or write). */
	int  LockRead(void ** dest);
	
	/* Lock an area of the buffer for reading, with maximum of *size* bytes.
	 * Returns the number of bytes locked for reading.
	 * The area is valid until the next change on the buffer (read or write). */	
	int  LockRead(int size, void ** dest);

	/* Discards bytes from the read buffer.
	 * Returns the number of bytes discarded. */
	int  DiscardBytes(int discardBytes);

	/* Compress/optmize the dynamic alocated memory. */
	void Compress();
};
