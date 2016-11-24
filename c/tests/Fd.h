/*
 * File.....: Fd.h
 * Author...: Jose Ferreira
 * Date.....: 2016-11-22 16:22
 * Purpose..: File descriptor writer/reader
 * */


#pragma once


#include "Ptr.h"
#include "Text.h"


/* Represents a generic file descriptor.
 * The fd has automic reference count management,
 * closing the descriptor when the last reference
 * is released. */
class Fd
{
protected:
	// Internal file descriptor.
	FdPtr m_fd;
	
public:
	// Last error code, or RET_OK in case of no errors.
	int LastErr;
	
	// String message for the last error code.
	const char * GetLastErrMsg() const;

	// Default constructor for a closed file descriptor.
	Fd();
	
	// Constructor for the *fd* informed.
	Fd(int fd);
	
	/* Release the current fd and
	 * sets the new fd for the instance. */
	void SetFd(int fd);
	
	// Returns the current file descriptor.
	int  GetFd() const;
	
	/* Release the current open file descriptor.
	 * Closes it if it's the last reference.
	 * Keeps LastErr state. */
	void Close();

	/* Release the current open file descriptor.
	 * Closes it if it's the last reference.
	 * Clear LastErr state to RET_OK. */
	void Clear();
	
	/* Returns true/false if the fd is readable (i.e. can read data without blocking). */
	bool IsReadable() const;
	
	/* Returns true/false if the fd is writeable (i.e. can write data without blocking). */
	bool IsWritable() const;
	
	/* Returns true/false if the fd is closed. */
	bool IsClosed() const;
	
	/* Returns the number of bytes immediately available for reading. */
	int  BytesAvailable() const;
	
	/* Enable/disable non-blocking state for the fd.
	 * Returns true/false if success in changing state. */
	bool SetNonBlock(int enabled);

	// --- Static methods ---

	/* Tests if the fd is readable (i.e. can read data without blocking). */
	static bool IsReadable(int fd);

	/* Tests if the fd is writeable (i.e. can write data without blocking). */
	static bool IsWritable(int fd);

	/* Adjusts the non-blocking state of the fd. */
	static bool SetNonBlock(int fd, int enable);

	/* Get the number of bytes that are immediately available for reading. */
	static int  BytesAvailable(int fd);
};


// Representes a file descriptor text writer
class FdWriter : public TextWriter
{
protected:
	int m_fd;

public:
	// Last error code, or RET_OK in case of no errors.
	int LastErr;

	// Default constructor for a NULL writer
	FdWriter();
	
	// Constructor that writes on the *fd* informed.
	FdWriter(int fd);
	
	// Sets the target fd to the fd informed.
	void SetFd(int fd);
	
	/* Returns the error code for the last error,
	 * of RET_OK if no error found. */
	virtual int GetLastErr();
	
	/* Returns string message for the last error code. */
	virtual const char * GetLastErrMsg();
	
	/* Returns true/false if error found. */
	virtual bool IsError();

	int Write(uint8_t c);
	int Write(const uint8_t * buffer, int size);
};


// Representes a file descriptor text reader
class FdReader : public TextReader
{
protected:
	int m_fd;

public:
	// Last error code, or RET_OK in case of no errors.
	int  LastErr;

	/* True on End of Stream/File. */
	bool Eof;

	// Default constructor that reads from an empty file.
	FdReader();
	
	// Constructor for reading the fd informed.
	FdReader(int fd);
	
	// Sets the base fd to the fd informed.
	void SetFd(int fd);

	bool IsEof() { return Eof; }
	
	bool IsError() { return LastErr != RET_OK; }
	
	/* Returns the error code for the last error,
	 * of RET_OK if no error found. */
	virtual int GetLastErr();
	
	/* Returns string message for the last error code. */
	virtual const char * GetLastErrMsg();

	int Read();
	int Read(uint8_t * buffer, int size);
};
