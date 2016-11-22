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
// Printable
//////////////////////////////////////////////////////////////////////



String Printable::ToString() const
{
	StringBuilder sb;
	printTo(sb);
	return sb.GetString();
}



//////////////////////////////////////////////////////////////////////
// String
//////////////////////////////////////////////////////////////////////



String::String()
{
	c_str = NULL;
}


String::String(void * c)
{
	c_str = (char *)c;
	m_aloc.Set(c);
}


String::String(char * c)
{
	c_str = c;
}


String::String(const char * c)
{
	c_str = c;
}


String::String(char * c, bool dynamic)
{
	c_str = c;
	if(dynamic) m_aloc.Set(c);
}


String::String(MemPtr mem, char * c)
{
	c_str = c;
	m_aloc = mem;
}


void String::Set(void * c)
{
	c_str = (char *)c;
	m_aloc.Set(c);
}


void String::Set(const char * c)
{
	c_str = c;
	m_aloc = NULL;
}


void String::Set(char * c, bool dynamic)
{
	c_str = c;
	if(dynamic)
		m_aloc.Set(c);
	else
		m_aloc.Set(NULL);
}


void String::Set(const String& c)
{
	this->c_str  = c.c_str;
	this->m_aloc = c.m_aloc;
}


void String::Clear()
{
	m_aloc.Set(NULL);
	c_str = NULL;
}


int String::printTo(TextWriter& p) const
{
	if(c_str == NULL) return 0;
	return p.print(c_str);
}


void String::Debug()
{
	printf("[String: c_str=[%p:%s] aloc=[%p]]\n", c_str, c_str, m_aloc.Get());
}



//////////////////////////////////////////////////////////////////////
// TextWriter
//////////////////////////////////////////////////////////////////////



int TextWriter::Write(const uint8_t* c, int size)
{
	for(int n = 0; n < size; n++) Write(c[n]);
	return size;
}


int TextWriter::println()
{
	Write(10);
	return 1;
}


int TextWriter::printf(const char * fmt, ...)
{
va_list ap;
int ret;
   va_start(ap, fmt);
   ret = printf(fmt, ap);
   va_end(ap);
   return ret;
}


int TextWriter::printf(const char * fmt, va_list va)
{
	return util_printf(this, fmt, va);
}


int TextWriter::print(const char str[])
{
int len = strlen(str);
    return Write((const uint8_t*)str, len);
}


int TextWriter::print(long n, int base)
{
int t=0;

	if(base < 2) base=10;

	if (base == 10 && n < 0) {
		t = print('-');
		n = -n;
	}

	return printNumber(n, base) + t;
}

int TextWriter::print(char c)                        { return Write(c); }
int TextWriter::print(unsigned char b, int base)     { return print((unsigned long) b, base); }
int TextWriter::print(int n, int base)               { return print((long) n, base); }
int TextWriter::print(unsigned int n, int base)      { return print((unsigned long) n, base); }
int TextWriter::print(unsigned long n, int base)     { return printNumber(n, base); }
int TextWriter::print(double n, int digits)          { return printFloat(n, digits); }
int TextWriter::print(float n, int digits)           { return printFloat(n, digits); }
int TextWriter::print(const Printable& x)            { return x.printTo(*this); }
int TextWriter::println(const char c[])              { return print(c) + println(); }
int TextWriter::println(char c)                      { return print(c) + println(); }
int TextWriter::println(unsigned char b, int base)   { return print(b, base) + println(); }
int TextWriter::println(int num, int base)           { return print(num, base) + println(); }
int TextWriter::println(unsigned int num, int base)  { return print(num, base) + println(); }
int TextWriter::println(long num, int base)          { return print(num, base) + println(); }
int TextWriter::println(unsigned long num, int base) { return print(num, base) + println(); }
int TextWriter::println(double num, int digits)      { return print(num, digits) + println(); }
int TextWriter::println(float num, int digits)       { return print(num, digits) + println(); }
int TextWriter::println(const Printable& x)          { return print(x) + println(); }

// Private Methods /////////////////////////////////////////////////////////////

int TextWriter::printNumber(unsigned long n, int base) {
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

int TextWriter::printFloat(double number, int digits) 
{ 
    int n = 0;

    // Handle negative numbers
    if (number < 0.0)
    {
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
    while (digits-- > 0)
    {
        remainder *= 10.0;
        int toPrint = int(remainder);
        n += print(toPrint);
        remainder -= toPrint;
    }

    return n;
}

int TextWriter::printFloat(float number, int digits)
{
    int n = 0;

    // Handle negative numbers
    if (number < 0.0)
    {
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
    while (digits-- > 0)
    {
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



int TextReader::Read(uint8_t * buffer, int size)
{
int pos = 0;
	while(pos < size)
	{
		int c = Read();
		if(c < 0) return (pos > 0) ? pos : c;
		buffer[pos] = (uint8_t)c;
		pos++;
	}
	return pos;
}


int TextReader::ReadLine(char * buffer, int size)
{
int c;
int pos = 0;
	while(pos < (size - 1))
	{
		c = Read();
		if(c < 0) {
			// printf("[TextReader:c==%d]", c);
			buffer[pos] = 10;
			return (pos > 0) ? pos : c;
		}
		if(c == 13) {
			// printf("[TR:CR]");
			continue;
		}
		if(c == 10) { 
			// printf("[TR:LF]");
			break;
		}
		// printf("[%c]", c);
		buffer[pos] = c;
		pos++;
	}
	buffer[pos] = 0;
	return pos;
}


bool TextReader::ReadLine(String& ret, int maxSize)
{
int c;
ByteBuffer buffer;
	if(maxSize <= 1) return false;
	while(buffer.Length() < maxSize)
	{
		c = Read();
		// printf("[C=%d,%c]", c, c);
		if(c < 0) {
			if(buffer.Length() == 0) { return false; }
			break;
		}
		if(c == 13) { continue; }
		if(c == 10) { break; }
		buffer.Write((char)c);
	}
	buffer.Write((char)0);
	ret.Set(buffer.GetString());
	return true;
}



//////////////////////////////////////////////////////////////////////
// StreamWriter
//////////////////////////////////////////////////////////////////////



StreamWriter::StreamWriter()
{ m_stream = &Stream::Null; }

StreamWriter::StreamWriter(Stream * stream)
{ m_stream = stream; }

void StreamWriter::Close()
{ m_stream->Close(); }

int StreamWriter::GetLastErr()
{ return m_stream->GetLastErr(); }

const char * StreamWriter::GetLastErrMsg()
{ return m_stream->GetLastErrMsg(); }

int StreamWriter::Write(const uint8_t c)
{ return m_stream->Write(c); }

int StreamWriter::Write(const uint8_t * c, int size)
{ return m_stream->Write(c, size); }



//////////////////////////////////////////////////////////////////////
// StreamReader
//////////////////////////////////////////////////////////////////////



StreamReader::StreamReader()
{ m_stream = &Stream::Null; }

StreamReader::StreamReader(Stream * stream)
{ m_stream = stream; }

void StreamReader::Close()
{ m_stream->Close(); }

int StreamReader::GetLastErr()
{ return m_stream->GetLastErr(); }

const char * StreamReader::GetLastErrMsg()
{ return m_stream->GetLastErrMsg(); }

int StreamReader::Read()
{ return m_stream->ReadByte(); }

int StreamReader::Read(uint8_t * c, int size)
{ return m_stream->Read(c, size); }
