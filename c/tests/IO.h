/*
 * File....: Stream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 09:35
 * Purpose.: Byte stream abstraction
 */


#pragma once


#include "Fd.h"
#include "Config.h"


/* Represents a stream for a file on disk.
 * Auto-manage the fd via FdPtr. */
class FileStream : public FdStream
{
protected:
	FdPtr m_fdPtr;

public:
	/* Create a file on disk with the name *fileName*.
	 * Returns true/false if created, and the FileStream on *ret*.
	 * Returns false in case of error.
	 * Consult the LastErr field on *ret* for the error reason. */ 
	static bool Create(const char * fileName, FileStream &ret);

	// Default constructor for a closed file.
	FileStream();
	
	// Constructor for the file on the informed descriptor.
	FileStream(int fd);

	// Sets the current descriptor for the *fd*.
	void SetFd(int fd);

	// Close the current open fd.
	void Close();
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
