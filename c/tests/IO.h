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


class DelegateReader : public CTextReader
{
private:
	CTextReader * m_reader = NULL;

public:
	DelegateReader(CTextReader* reader);
	void Set(CTextReader * reader);

	bool IsEof() override;
	bool IsError() override;
	int Read() override;
	int Read(void * buffer, int size) override;
	int GetLastErr() override;
	const char * GetLastErrMsg() override;
};


class DelegateWriter : public CTextWriter
{
private:
	CTextWriter * m_writer;

public:
	DelegateWriter(CTextWriter * writer);
	void Set(CTextWriter * writer);

	bool IsError() override;
	int Write(const uint8_t c) override;
	int Write(const void * buffer, int size) override;
	int GetLastErr() override;
	const char * GetLastErrMsg() override;
};


extern DelegateReader StdIn;
extern DelegateWriter StdOut;
extern DelegateWriter StdErr;
