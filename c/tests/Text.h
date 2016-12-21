/*
 * File.....: Text.h
 * Author...: Jose Ferreira
 * Date.....: 2016-11-22 10:43
 * Purpose..: Base text and string support
 */


#pragma once


#include <inttypes.h>

#include "Stream.h"
#include "Ptr.h"


#define DEC 10
#define HEX 16
#define OCT 8
#define BIN 2


class CPrintable;
class CTextWriter;
class CString;
class CNullText;


/** The Printable class provides a way for new classes to allow themselves to be printed.
    By deriving from Printable and implementing the printTo method, it will then be possible
    for users to print out instances of this class by passing them into the usual
    Print::print and Print::println methods.
*/
class CPrintable
{
public:
	// Print the current instance to the writer
	virtual int printTo(CTextWriter& p) const {
		static_cast<void>(p);
		return 0;
	}

	// Print the current instance to the writer
	virtual int printTo(CTextWriter* p) const {
		return printTo(*p);
	}

	// Returns the string representation of the current instance
	CString ToString() const;
};


// Represents an immutable null terminated string
// with optional automatic dynamic memory management using reference counting.
class CString : public CPrintable
{
protected:
	// Pointer for dynamic allocation (not used if ro.section or stack memory)
	CMemPtr m_aloc;

public:
	// Read-only pointer for ther null terminated string.
	const char * c_str;

	// Default constructor for a NULL string.
	CString();

	// Constructor for readonly or stack string.
	CString(const char * c);

	// Constructor for readonly, stack or dynamic string.
	// Automatic memory management selectable via *dynamic* flag.
	CString(char * c, bool dynamic);

	// Constructor for dynamic string with automatic memory management using pointer *mem*.
	explicit CString(CMemPtr mem);

	// Constructor for dynamic string with automatic memory management using pointer *mem*.
	CString(CMemPtr mem, char * c);

	// // Free the current string resources and sets the instance to the dynamic memory string c.
	// void Set(void * c);

	// Free the current string resources and sets the instance to the readonly or stack string c.
	void Set(const char * c);

	// Free the current string resources and sets the instance to string c.
	// Automatic memory management selectable via *dynamic* flag.
	void Set(char * c, bool dynamic);

	/* Free the current string resources and set the instance to *mem*
	 * with automatic memory management. */
	void Set(const CMemPtr& mem);

	/* Free the current string resources and set the instance to string c
	 * with automatic memory management using pointer *mem*. */
	void Set(const CMemPtr& mem, char * c);

	// Free the current string resources and set the instance to be the same string of c
	void Set(const CString& c);

	// Free the current string and set the instance to the NULL string
	void Clear();

	// Print the string to ther target writer.
	int printTo(CTextWriter& p) const;

	// Print string debug info to stdout
	void CDebug();

	// Create a formated string using printf semantics.
	static CString Format(const char* fmt, ...);
};


// Represents an abstract text/string writer
class CTextWriter
{
protected:
	int printNumber(unsigned long n, int base);
	int printFloat(double number, int digits);
	int printFloat(float  number, int digits);

public:

	virtual bool IsError() = 0;

	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr() {
		return RET_OK;
	}

	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg() {
		return "";
	}

	virtual int Write(const uint8_t c) = 0;
	virtual int Write(const void  * c, int size);

	int print(const char[]);
	int print(char);
	int print(unsigned char, int = DEC);
	int print(int,           int = DEC);
	int print(unsigned int,  int = DEC);
	int print(long,          int = DEC);
	int print(unsigned long, int = DEC);
	int print(double,        int = 2);
	int print(float,         int = 2);
	int print(const CPrintable&);

	int println(const char[]);
	int println(char);
	int println(unsigned char, int = DEC);
	int println(int,           int = DEC);
	int println(unsigned int,  int = DEC);
	int println(long,          int = DEC);
	int println(unsigned long, int = DEC);
	int println(double,        int = 2);
	int println(float,         int = 2);
	int println(const CPrintable&);
	int println(void);

	// Print the data using printf semantics. You can also
	// use %z to print *Printable* objects.
	int printf(const char* fmt, ...);

	// Print the data using printf semantics. You can also
	// use %z to print *Printable* objects.
	int printf(const char* fmt, va_list ap);

	// Represents a null writer.
	static CNullText Null;
};


// Represents an abstract text/string reader
class CTextReader
{
public:
	/* True/false on EndOfStream/File. */
	virtual bool IsEof() = 0;

	/* True/false on error. */
	virtual bool IsError() = 0; // { return GetLastErr()!=RET_OK; }

	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr() = 0;

	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg() = 0;

	virtual int Read() = 0;
	virtual int Read(void * buffer, int size);

	/* Read a line of text, terminated by LF character, and put it in the string buffer.
	 * Returns the number of bytes read, 0 if empty line, INT_EOF if eof, INT_ERR if error. */
	virtual int ReadLine(char * buffer, int size);

	/* Read a line of text, terminated by LF character, and put in the ret string.
	 * Returns true/false if success. */
	virtual bool ReadLine(CString& ret, int maxSize);

	// Represents a null writer.
	static CNullText Null;
};


// Represents a text/string writer that writes to a stream.
class CStreamWriter : public CTextWriter
{
protected:
	// Internal base stream.
	CStream * m_stream;

public:
	// Default constructor that writes to a null stream.
	CStreamWriter();

	// Constructor that writes on the target stream.
	explicit CStreamWriter(CStream * stream);

	/* Closes the writer. Does not close the stream. */
	void Close();

	/* Returns the last error code, or RET_OK if not error found. */
	virtual int GetLastErr();

	/* Returns the string message for the last error code. */
	virtual const char * GetLastErrMsg();
	
	/* Returns true/false if error found. */
	virtual bool IsError();

	int  Write(const uint8_t c);
	int  Write(const void * c, int size);
};


// Represents a text/string reader that reads from a stream.
class CStreamReader : public CTextReader
{
protected:
	// Internal base stream.
	CStream  * m_stream;

public:
	// Default constructor that reads from an empty stream.
	CStreamReader();

	// Constructor that reads from the source stream.
	explicit CStreamReader(CStream * stream);

	/* Closes the reader. Does not close the stream. */
	void Close();

	bool IsEof();
	
	bool IsError();

	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr();

	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg();

	int  Read();
	int  Read(void * c, int size);
};


// Represents a null text reader/writer (EOF on read and null store on write).
class CNullText : public CTextReader, public CTextWriter
{
public:
	bool IsEof()                 { return true; }
	bool IsError()               { return false; }
	int GetLastErr()             { return RET_OK; }
	const char* GetLastErrMsg()  { return ""; }
	int Read()                   { return INT_EOF; }
	int Write(const uint8_t c)   { static_cast<void>(c); return 1; }
};


class CNullReader : public CTextReader
{
public:
	bool IsEof()         { return true; }
	bool IsError()       { return false; }
	int  GetLastErr()    { return RET_OK; }
	cstr GetLastErrMsg() { return ""; }
	int Read()           { return INT_EOF; }
	int Read(void * c, int size) { static_cast<void>(c); static_cast<void>(size); return 0; }
};


class CNullWriter : public CTextWriter
{
public:
	// bool IsEof()         { return true; }
	bool IsError()       { return false; }
	int  GetLastErr()    { return RET_OK; }
	cstr GetLastErrMsg() { return ""; }
	int Write(const uint8_t c) { static_cast<void>(c); return 1; }
	int Write(const void * c, int size) { static_cast<void>(c); static_cast<void>(size); return size; }
};
