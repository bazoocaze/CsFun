/*
 * File....: Stream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 09:35
 * Purpose.: Byte stream abstraction
 */


#pragma once


// #include <stdio.h>
// #include <stdlib.h>
// #include <string.h>
// #include <inttypes.h>
// #include <unistd.h>


#include "Util.h"


#define DEC 10
#define HEX 16
#define OCT 8
#define BIN 2



class NullStream;
class Printable;
class ByteBuffer;



// Byte stream abstraction
class Stream
{
public:
	// Close the stream
	virtual void Close() { }

	/* Write a byte to the stream.
	 * Return the number of bytes written, or RET_ERR on error. */
	virtual int Write(const uint8_t c) { return RET_ERR; }
	
	/* Write bytes to the stream.
	 * Return the number of bytes written, or RET_ERR on error. */
	virtual int Write(const uint8_t * c, int size) { return RET_ERR; } 

	/* Read a byte from stream.
	 * Returns the byte read, or INT_EOF on EOF or INT_ERR on error. */
	virtual int ReadByte() { return INT_ERR; }

	/* Read bytes from stream.
	 * Returns the number of read bytes, including 0 on EOF.
	 * Returns RET_ERR on error. */
	virtual int Read(uint8_t * buffer, int size) { return RET_ERR; }

	/* Returns the last error code, or RET_OK in case of no error. */
	virtual int GetLastErr() = 0;
	
	/* Returns the string message for the last error code. */
	virtual const char * GetLastErrMsg() = 0;

	/* Null stream (EOF on read and null store on write). */
	static NullStream Null;
};


// Null stream abstraction
class NullStream : public Stream
{
public:
	void Close()                             { }
	int  Write(const uint8_t c)              { return 1; }
	int  Write(const uint8_t * c, int size)  { return size; }
	int  ReadByte()                          { return INT_EOF; }
	int  Read(uint8_t buffer, int size)      { return 0; }
	int  GetLastErr()                        { return RET_OK; }
	const char * GetLastErrMsg()             { return ""; }
	
};


// File descriptor stream abstraction
class FdStream : public Stream
{
protected:
	// file descriptor, or CLOSED_FD if closed
	int m_fd;
	// last error code
	int m_lastErr;

public:
	// Constructs a closed fd stream
	FdStream();

	// Constructs a stream on fd
	FdStream(int fd);

	// Sets file descriptor to fd
	virtual void SetFd(int fd);

	// Returns the current file descriptor, or CLOSED_FD if closed
	int  GetFd();

	// Closes the current file descriptor, and set fd to CLOSED_FD
	virtual void Close();

	// Write the byte to the fd. Return the number of bytes written, or RET_ERR if error.
	int  Write(const uint8_t c);

	// Write byte buffer to the fd. Return the number of bytes written, or RET_ERR if error.
	int  Write(const uint8_t * c, int size);

	// Read a byte from the fd. Returns INT_EOF on EOF or INT_ERR on error.
	int  ReadByte();

	// Read a block of bytes from the fd. Returns the number of bytes read, 0 on EOF, or RET_ERR on error.
	int  Read(uint8_t * buffer, int size);

	// Returns the last error code, or RET_OK in case of no error found
	int  GetLastErr();
	
	// Returns the string message for the last error code
	const char * GetLastErrMsg();
};

