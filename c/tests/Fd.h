/*
 * File.....: Fd.h
 * Author...: Jose Ferreira
 * Date.....: 2016-11-22 16:22
 * Purpose..: File descriptor writer/reader
 * */


#pragma once


#include "Ptr.h"
#include "Text.h"


/* Automatic management of integer file descriptor (FD) by reference counting.
   The descriptor is automatic closed (via close() call) when needed. */
class CFdHandle : CRefPtr
{
private:
	virtual void ReleaseData(void *);

public:
	// Cached read-only copy of the managed file descriptor.
	int  Fd;

	// Default empty constructor.
	CFdHandle();

	// Constructor for automatic management of the fd descriptor.
	CFdHandle(int fd);

	// Automatic return Fd on conversion to int.
	operator int() const { return Fd; }

	// Default destructor. Releases the fd.
	~CFdHandle();

	// Release the managed fd, closing the fd if the reference count reaches 0.
	void Close();

	// Release the current fd and begin management for the new fd.
	void Set(int fd);

	void Debug();
};


/* Represents a generic file descriptor.
 * The fd has automic reference count management,
 * closing the descriptor when the last reference
 * is released. */
class CFd
{
protected:
	// Internal file descriptor.
	CFdHandle m_fd;
	
public:
	// Last error code, or RET_OK in case of no errors.
	int LastErr;
	
	// String message for the last error code.
	const char * GetLastErrMsg() const;

	// Default constructor for a closed file descriptor.
	CFd();
	
	// Constructor for the *fd* informed.
	explicit CFd(int fd);
	
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
class CFdWriter : public CTextWriter
{
protected:
	int m_fd;

public:
	// Last error code, or RET_OK in case of no errors.
	int LastErr;

	// Default constructor for a NULL writer
	CFdWriter();
	
	// Constructor that writes on the *fd* informed.
	explicit CFdWriter(int fd);
	
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
	int Write(const void * buffer, int size);
};


// Representes a file descriptor text reader
class CFdReader : public CTextReader
{
protected:
	int m_fd;

public:
	// Last error code, or RET_OK in case of no errors.
	int  LastErr;

	/* True on End of Stream/File. */
	bool Eof;

	// Default constructor that reads from an empty file.
	CFdReader();
	
	// Constructor for reading the fd informed.
	explicit CFdReader(int fd);
	
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
	int Read(void * buffer, int size);
};
