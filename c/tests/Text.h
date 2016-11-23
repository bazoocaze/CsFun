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


class Printable;
class TextWriter;
class String;


/** The Printable class provides a way for new classes to allow themselves to be printed.
    By deriving from Printable and implementing the printTo method, it will then be possible
    for users to print out instances of this class by passing them into the usual
    Print::print and Print::println methods.
*/
class Printable
{
public:
	// Print the current instance to the writer
	virtual int printTo(TextWriter& p) const { return 0; }
	
	// Returns the string representation of the current instance
	String ToString() const;
};


// Represents an immutable null terminated string
// with optional automatic dynamic memory management using reference counting.
class String : public Printable
{
protected:
	// Pointer for dynamic allocation (not used if ro.section or stack memory)
	MemPtr m_aloc;

public:
	// Read-only pointer for ther null terminated string.
	const char * c_str;

	// Default constructor for a NULL string.
	String();

	// Constructor for dynamic memory string. Free the memory resource on end of use.
	String(void * c);

	// Constructor for readonly or stack string.
	String(char * c);

	// Constructor for readonly or stack string.
	String(const char * c);

	// Constructor for readonly, stack or dynamic string.
	// Automatic memory management selectable via *dynamic* flag.
	String(char * c, bool dynamic);

	// Constructor for dynamic string with automatic memory management using pointer *mem*.
	String(MemPtr mem);

	// Constructor for dynamic string with automatic memory management using pointer *mem*.
	String(MemPtr mem, char * c);

	// Free the current string resources and sets the instance to the dynamic memory string c.
	void Set(void * c);

	// Free the current string resources and sets the instance to the readonly or stack string c.
	void Set(const char * c);

	// Free the current string resources and sets the instance to string c.
	// Automatic memory management selectable via *dynamic* flag.
	void Set(char * c, bool dynamic);

	/* Free the current string resources and set the instance to *mem*
	 * with automatic memory management. */
	void Set(const MemPtr& mem);

	/* Free the current string resources and set the instance to string c
	 * with automatic memory management using pointer *mem*. */
	void Set(const MemPtr& mem, char * c);

	// Free the current string resources and set the instance to be the same string of c
	void Set(const String& c);

	// Free the current string and set the instance to the NULL string
	void Clear();

	// Print the string to ther target writer.
	int printTo(TextWriter& p) const;

	// Print string debug info to stdout
	void Debug();
};


// Represents an abstract text/string writer
class TextWriter
{
protected:
	int printNumber(unsigned long n, int base);
	int printFloat(double number, int digits);
	int printFloat(float  number, int digits);

public:
	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr() { return RET_OK; }
	
	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg() { return ""; }

	virtual int Write(const uint8_t c) = 0;
	virtual int Write(const uint8_t * c, int size);

	int print(const char[]);
	int print(char);
	int print(unsigned char, int = DEC);
	int print(int,           int = DEC);
	int print(unsigned int,  int = DEC);
	int print(long,          int = DEC);
	int print(unsigned long, int = DEC);
	int print(double,        int = 2);
	int print(float,         int = 2);
	int print(const Printable&);

	int println(const char[]);
	int println(char);
	int println(unsigned char, int = DEC);
	int println(int,           int = DEC);
	int println(unsigned int,  int = DEC);
	int println(long,          int = DEC);
	int println(unsigned long, int = DEC);
	int println(double,        int = 2);
	int println(float,         int = 2);
	int println(const Printable&);
	int println(void);

	// Print the data using printf semantics. You can also
	// use %z to print *Printable* objects.
	int printf(const char* fmt, ...);

	// Print the data using printf semantics. You can also
	// use %z to print *Printable* objects.
	int printf(const char* fmt, va_list ap);
};


// Represents an abstract text/string reader
class TextReader
{
public:
	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr() = 0;
	
	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg() = 0;

	virtual int Read() = 0;
	virtual int Read(uint8_t * buffer, int size);

	/* Read a line of text, terminated by LF character, and put it in the string buffer.
	 * Returns the number of bytes read, 0 if empty line, INT_EOF if eof, INT_ERR if error. */
	virtual int ReadLine(char * buffer, int size);

	/* Read a line of text, terminated by LF character, and put in the ret string.
	 * Returns true/false if success. */
	virtual bool ReadLine(String& ret, int maxSize);
};


// Represents a text/string writer that writes to a stream.
class StreamWriter : public TextWriter
{
protected:
	// Internal base stream.
	Stream * m_stream;
	
public:
	// Default constructor that writes to a null stream.
	StreamWriter();
	
	// Constructor that writes on the target stream.
	StreamWriter(Stream * stream);
	
	/* Closes the writer. Does not close the stream. */
	void Close();

	/* Returns the last error code, or RET_OK if not error found. */
	virtual int GetLastErr();
	
	/* Returns the string message for the last error code. */
	virtual const char * GetLastErrMsg();

	int  Write(const uint8_t c);
	int  Write(const uint8_t * c, int size);
};


// Represents a text/string reader that reads from a stream.
class StreamReader : public TextReader
{
protected:
	// Internal base stream.
	Stream  * m_stream;
	
public:
	// Default constructor that reads from an empty stream.
	StreamReader();
	
	// Constructor that reads from the source stream.
	StreamReader(Stream * stream);
	
	/* Closes the reader. Does not close the stream. */
	void Close();

	// Returns the last error code, or RET_OK if not error found.
	virtual int GetLastErr();
	
	// Returns the string message for the last error code.
	virtual const char * GetLastErrMsg();

	int  Read();
	int  Read(uint8_t * c, int size);
};
