#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unistd.h>
#include <stdarg.h>


#include "Text.h"
#include "Stream.h"
#include "Binary.h"
#include "Ptr.h"
#include "Util.h"
#include "StringBuilder.h"


//////////////////////////////////////////////////////////////////////
// Null Reader and Writer
//////////////////////////////////////////////////////////////////////


CNullText CTextReader::Null = CNullText();
CNullText CTextWriter::Null = CNullText();


//////////////////////////////////////////////////////////////////////
// Printable
//////////////////////////////////////////////////////////////////////



CString CPrintable::ToString() const
{
	CStringBuilder sb;
	printTo(sb);
	return sb.GetString();
}



//////////////////////////////////////////////////////////////////////
// String
//////////////////////////////////////////////////////////////////////



CString::CString()
{
	c_str = NULL;
	m_aloc.Set(NULL);
}


CString::CString(const char * c)
{
	c_str = c;
	m_aloc.Set(NULL);
}


CString::CString(char * c, bool dynamic)
{
	c_str = c;
	if(dynamic)
		m_aloc.Set(c);
	else
		m_aloc.Set(NULL);
}


CString::CString(CMemPtr mem)
{
	m_aloc = mem;
	c_str  = (cstr)mem.Get();
}


CString::CString(CMemPtr mem, char * c)
{
	m_aloc = mem;
	c_str = c;
}


void CString::Set(const char * c)
{
	m_aloc.Clear();
	c_str = c;
}


void CString::Set(char * c, bool dynamic)
{
	c_str = c;
	if(dynamic)
		m_aloc.Set(c);
	else
		m_aloc.Clear();
}


void CString::Set(const CString& c)
{
	this->m_aloc = c.m_aloc;
	this->c_str  = c.c_str;
}


void CString::Set(const CMemPtr& mem)
{
	this->m_aloc = mem;
	this->c_str  = (cstr)mem.Get();
}


void CString::Set(const CMemPtr& mem, vstr c)
{
	this->m_aloc = mem;
	this->c_str  = c;
}


void CString::Clear()
{
	m_aloc.Set(NULL);
	c_str = NULL;
}


int CString::printTo(CTextWriter& p) const
{
	if(c_str == NULL || c_str[0] == 0)
		return 0;
	return p.print(c_str);
}


void CString::CDebug()
{
	printf("[String: c_str=[%p:%s] aloc=[%p]]\n", c_str, c_str, m_aloc.Get());
}



//////////////////////////////////////////////////////////////////////
// TextWriter
//////////////////////////////////////////////////////////////////////



int CTextWriter::Write(const void* c, int size)
{
	uint8_t* ptr = (uint8_t*)c;
	for(int n = 0; n < size; n++)
		Write(ptr[n]);
	return size;
}


int CTextWriter::println()
{
	Write(10);
	return 1;
}


int CTextWriter::printf(const char * fmt, ...)
{
	va_list ap;
	int ret;
	va_start(ap, fmt);
	ret = printf(fmt, ap);
	va_end(ap);
	return ret;
}


int CTextWriter::printf(const char * fmt, va_list va)
{
	return util_printf(this, fmt, va);
}


int CTextWriter::print(const char str[])
{
	int len = strlen(str);
	return Write((const uint8_t*)str, len);
}


int CTextWriter::print(long n, int base)
{
	int t=0;

	if(base < 2)
		base=10;

	if (base == 10 && n < 0) {
		t = print('-');
		n = -n;
	}

	return printNumber(n, base) + t;
}

int CTextWriter::print(char c)
{
	return Write(c);
}

int CTextWriter::print(unsigned char b, int base)
{
	return print((unsigned long) b, base);
}

int CTextWriter::print(int n, int base)
{
	return print((long) n, base);
}

int CTextWriter::print(unsigned int n, int base)
{
	return print((unsigned long) n, base);
}

int CTextWriter::print(unsigned long n, int base)
{
	return printNumber(n, base);
}

int CTextWriter::print(double n, int digits)
{
	return printFloat(n, digits);
}

int CTextWriter::print(float n, int digits)
{
	return printFloat(n, digits);
}

int CTextWriter::print(const CPrintable& x)
{
	return x.printTo(*this);
}

int CTextWriter::println(const char c[])
{
	return print(c) + println();
}

int CTextWriter::println(char c)
{
	return print(c) + println();
}

int CTextWriter::println(unsigned char b, int base)
{
	return print(b, base) + println();
}

int CTextWriter::println(int num, int base)
{
	return print(num, base) + println();
}

int CTextWriter::println(unsigned int num, int base)
{
	return print(num, base) + println();
}

int CTextWriter::println(long num, int base)
{
	return print(num, base) + println();
}

int CTextWriter::println(unsigned long num, int base)
{
	return print(num, base) + println();
}

int CTextWriter::println(double num, int digits)
{
	return print(num, digits) + println();
}

int CTextWriter::println(float num, int digits)
{
	return print(num, digits) + println();
}

int CTextWriter::println(const CPrintable& x)
{
	return print(x) + println();
}

// Private Methods /////////////////////////////////////////////////////////////

int CTextWriter::printNumber(unsigned long n, int base)
{
	char buf[8 * sizeof(long) + 1]; // Assumes 8-bit chars plus zero byte.
	char *str = &buf[sizeof(buf) - 1];

	*str = '\0';

	// prevent crash if called with base == 1
	if (base < 2) base = 10;

	do {
		unsigned long m = n;
		n /= base;
		char c = m - base * n;
		*--str = c < 10 ? c + '0' : c + 'A' - 10;
	} while(n);

	return print(str);
}

int CTextWriter::printFloat(double number, int digits)
{
	int n = 0;

	// Handle negative numbers
	if (number < 0.0) {
		n += print('-');
		number = -number;
	}

	// Round correctly so that print(1.999, 2) prints as "2.00"
	double rounding = 0.5;
	for (uint8_t i=0; i<digits; ++i)
		rounding /= 10.0;

	number += rounding;

	// Extract the integer part of the number and print it
	unsigned long int_part = (unsigned long)number;
	double remainder = number - (double)int_part;
	n += print(int_part);

	// Print the decimal point, but only if there are digits beyond
	if (digits > 0) {
		n += print(".");
	}

	// Extract digits from the remainder one at a time
	while (digits-- > 0) {
		remainder *= 10.0;
		int toPrint = int(remainder);
		n += print(toPrint);
		remainder -= toPrint;
	}

	return n;
}

int CTextWriter::printFloat(float number, int digits)
{
	int n = 0;

	// Handle negative numbers
	if (number < 0.0) {
		n += print('-');
		number = -number;
	}

	// Round correctly so that print(1.999, 2) prints as "2.00"
	float rounding = 0.5;
	for (uint8_t i=0; i<digits; ++i)
		rounding /= 10.0;

	number += rounding;

	// Extract the integer part of the number and print it
	unsigned long int_part = (unsigned long)number;
	float remainder = number - (float)int_part;
	n += print(int_part);

	// Print the decimal point, but only if there are digits beyond
	if (digits > 0) {
		n += print(".");
	}

	// Extract digits from the remainder one at a time
	while (digits-- > 0) {
		remainder *= 10.0;
		int toPrint = int(remainder);
		n += print(toPrint);
		remainder -= toPrint;
	}

	return n;
}



//////////////////////////////////////////////////////////////////////
// TextReader
//////////////////////////////////////////////////////////////////////



int CTextReader::Read(void * buffer, int size)
{
	uint8_t* ptr = (uint8_t*)buffer;
	int pos = 0;
	while(pos < size) {
		int c = Read();
		if(c < 0)
			return (pos > 0) ? pos : c;
		ptr[pos] = (uint8_t)c;
		pos++;
	}
	return pos;
}


int CTextReader::ReadLine(char * buffer, int size)
{
	int pos = 0;
	while(pos < (size - 1)) {
		int c = Read();
		if(c < 0) {
			buffer[pos] = 10;
			return (pos > 0) ? pos : c;
		}
		if(c == 13)
			continue;
		if(c == 10)
			break;
		buffer[pos] = c;
		pos++;
	}
	buffer[pos] = 0;
	return pos;
}


bool CTextReader::ReadLine(CString& ret, int maxSize)
{
	CByteBuffer buffer;
	if(maxSize <= 1)
		return false;
	while(buffer.Length() < maxSize) {
		int c = Read();
		if(c < 0) {
			if(buffer.Length() == 0)
				return false;
			break;
		}
		if(c == 13)
			continue;
		if(c == 10)
			break;
		buffer.Write((char)c);
	}
	buffer.Write((char)0);
	ret.Set(buffer.GetString());
	return true;
}



//////////////////////////////////////////////////////////////////////
// StreamWriter
//////////////////////////////////////////////////////////////////////



CStreamWriter::CStreamWriter()
{
	m_stream = &CStream::Null;
}

CStreamWriter::CStreamWriter(CStream * stream)
{
	m_stream = stream;
}

void CStreamWriter::Close()
{
	m_stream->Close();
}

int CStreamWriter::GetLastErr()
{
	return m_stream->GetLastErr();
}

const char * CStreamWriter::GetLastErrMsg()
{
	return m_stream->GetLastErrMsg();
}

bool CStreamWriter::IsError()
{
	return m_stream->IsError();
}

int CStreamWriter::Write(const uint8_t c)
{
	return m_stream->Write(c);
}

int CStreamWriter::Write(const void * c, int size)
{
	return m_stream->Write(c, size);
}



//////////////////////////////////////////////////////////////////////
// StreamReader
//////////////////////////////////////////////////////////////////////



CStreamReader::CStreamReader()
{
	m_stream = &CStream::Null;
}

CStreamReader::CStreamReader(CStream * stream)
{
	m_stream = stream;
}

void CStreamReader::Close()
{
	m_stream->Close();
}

bool CStreamReader::IsEof()
{
	return m_stream->IsEof();
}

bool CStreamReader::IsError()
{
	return m_stream->IsError();
}

int CStreamReader::GetLastErr()
{
	return m_stream->GetLastErr();
}

const char * CStreamReader::GetLastErrMsg()
{
	return m_stream->GetLastErrMsg();
}

int CStreamReader::Read()
{
	return m_stream->ReadByte();
}

int CStreamReader::Read(void * c, int size)
{
	return m_stream->Read(c, size);
}
