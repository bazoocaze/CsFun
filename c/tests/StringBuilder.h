/*
 * File....: StringBuilder.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 18:29
 * Purpose.: String builder helper.
 * */


#pragma once


#include "ByteBuffer.h"
#include "Text.h"


/* String builder helper writer.
 * Use the GetString() method to convert the builder to a String.
 * (the content of the builder is empty after the call). */
class CStringBuilder : public CTextWriter
{
private:
	// Internal buffer
	CByteBuffer m_buffer;

public:
	// Length of the data in the builder
	int Length();

	int Write(uint8_t c);
	int Write(uint8_t * buffer, int size);
	
	bool IsError() { return false; }

	// const char * GetStr();
	
	/* Zero-terminate and return the string built.
	 * This operation drains the buffer to the
	 * empty state. */
	CString GetString();
};
