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
#include <fcntl.h>
#include <errno.h>


#include "Stream.h"
#include "Ptr.h"
#include "Util.h"
#include "Fd.h"
#include "Config.h"


// Represents a stream for a file on disk
class FileStream : public FdStream
{
protected:
	FdPtr m_fdPtr;

public:
	/* Create a file on disk with the name *fileName*.
	 * Returns true/false if created, and the FileStream on *ret*.
	 * Returns false in case of error.
	 * Consult the LastErr field on *ret* for the error reason. */ 
	static bool Create(const char * fileName, FileStream &ret)
	{
	int fd;
		fd = creat(fileName, S_IRUSR | S_IWUSR | S_IRGRP | S_IWGRP);
		if(fd == RET_ERR) {
			ret.m_lastErr = errno;
			return false;
		}
		ret.SetFd(fd);
		return true;
	}

	// Default constructor for a closed file.
	FileStream() : FdStream()
	{
	}
	
	// Constructor for the file on the informed descriptor.
	FileStream(int fd) : FdStream(fd)
	{
		m_fdPtr.Set(fd);
	}

	// Sets the current descriptor for the *fd*.
	void SetFd(int fd)
	{
		FdStream::SetFd(fd);
		m_fdPtr.Set(fd);
	}

	// Close the current open fd.
	void Close()
	{
		m_fdPtr.Set(CLOSED_FD);
		m_fd = CLOSED_FD;
	}
};


#if defined HAVE_CONSOLE

	// Abstraction for the console terminal
	class CConsole : public FdWriter, public FdReader
	{
	public: CConsole() : FdWriter(1), FdReader(0) { }
	};

	// Console (standard input and output) for the current session.
	extern CConsole Console;

	// Standard input reader
	extern FdReader StdIn;

	// Standard output writer
	extern FdWriter StdOut;

	// Standard error writer
	extern FdWriter StdErr;

#else

	// Dummy console.
	extern NullText Console;

	// Dummy stdin.
	extern NullText StdIn;

	// Dummy stdout.
	extern NullText StdOut;

	// Dummy stderr.
	extern NullText StdErr;

#endif
