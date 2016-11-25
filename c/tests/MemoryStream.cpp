/*
 * File....: MemoryStream.cpp
 * Author..: Jose Ferreira
 * Updated.: 2016-11-24 22:07
 * Purpose.: Dynamic memory stream of bytes
 * */


#include "MemoryStream.h"



//////////////////////////////////////////////////////////////////////
// MemoryStream
//////////////////////////////////////////////////////////////////////



MemoryStream::MemoryStream()
{
}


int MemoryStream::Length() const
{
	return m_buffer.Length();
}


const uint8_t * MemoryStream::GetReadPtr()
{
void * ret;
	m_buffer.LockRead(0, &ret);
	return (uint8_t *)ret;
}


int MemoryStream::GetLastErr()
{
	return RET_OK;
}


const char * MemoryStream::GetLastErrMsg()
{
	return "";
}


bool MemoryStream::IsEof()
{
	return (m_buffer.Length() == 0);
}


bool MemoryStream::IsError()
{
	return false;
}


int MemoryStream::ReadByte()
{
	if(m_buffer.Length()==0) return INT_EOF;
	return m_buffer.ReadByte();
}


int MemoryStream::Read(void * buffer, int size)
{
int len = size;
	if(len > m_buffer.Length()) len = m_buffer.Length();
	if(len > 0)
		return m_buffer.Read(buffer, 0, len);
	return 0;
}


int MemoryStream::Write(const uint8_t c)
{
	return m_buffer.Write(c);
}


int MemoryStream::Write(const void * c, int size)
{
	return m_buffer.Write(c, 0, size);
}
