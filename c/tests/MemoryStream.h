/*
 * File....: MemoryStream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 12:07
 * Purpose.: Memory stream abstraction
 */


#pragma once


#include "Stream.h"
#include "ByteBuffer.h"


// Represents a dynamic memory stream buffer.
class CMemoryStream : public CStream
{
protected:
	// Internal buffer for the data
	CByteBuffer m_buffer;

public:
	// Default constructor for an empty read/write memory buffer.
	CMemoryStream();
	
	/* Returns the pointer for the read/input data.
	 * The pointer is valid until the next change in the stream. */
	const uint8_t * GetReadPtr();
	
	/* Read a byte from the buffer.
	 * Returns the byte read, or INT_EOF in case of EOF (empty buffer). */
	int ReadByte();
	
	/* Read a block of bytes from the buffer.
	 * Returns the number of bytes read, including 0 for and empty buffer. */
	int Read(void * buffer, int size);
	
	/* Write a byte on the buffer.
	 * Return the number of bytes written,
	 * or RET_ERR in case of error (read-only buffer). */
	int Write(const uint8_t c);
	
	/* Write a block of bytes to the buffer.
	 * Returns the number of bytes written,
	 * or RET_ERR in case of error (read-only buffer). */
	int Write(const void * c, int size);
	
	/* Returns the length of the data in the stream. */
	int Length() const;
	
	/* Returns the last error code, or RET_OK in case of no errors. */
	int GetLastErr();
	
	/* Returns the string message for the last error code. */
	const char * GetLastErrMsg();
	
	/* Returns/false if the buffer is empty. */
	bool IsEof();
	
	/* Returns trye/false if error found.
	 * Default implementation allways return false. */
	bool IsError();
};
