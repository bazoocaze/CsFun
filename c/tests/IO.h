/*
 * File....: Stream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 09:35
 * Purpose.: Byte stream abstraction
 */


#pragma once


#include "Fd.h"
#include "Text.h"
#include "Config.h"


/* Represents a stream for a file on disk.
 * Auto-manage the fd via FdPtr. */
class CFileStream : public CFdStream
{
protected:
	CFdHandle m_fdPtr;

public:
	/* Create a file on disk with the name *fileName*.
	 * Returns true/false if created, and the FileStream on *ret*.
	 * Returns false in case of error.
	 * Consult the LastErr field on *ret* for the error reason. */ 
	static bool Create(const char * fileName, CFileStream &ret);

	// Default constructor for a closed file.
	CFileStream();
	
	// Constructor for the file on the informed descriptor.
	explicit CFileStream(int fd);

	// Sets the current descriptor for the *fd*.
	void SetFd(int fd);

	// Close the current open fd.
	void Close();
};


#if defined HAVE_CONSOLE

	// Standard input reader
	extern CFdReader StdIn;

	// Standard output writer
	extern CFdWriter StdOut;

	// Standard error writer
	extern CFdWriter StdErr;

	// extern CConsole bConsole;

#else

	// Dummy stdin.
	extern CNullText StdIn;

	// Dummy stdout.
	extern CNullText StdOut;

	// Dummy stderr.
	extern CNullText StdErr;

#endif
