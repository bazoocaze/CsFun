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



CMemoryStream::CMemoryStream()
{
}


int CMemoryStream::Length() const
{
	return m_buffer.Length();
}


const uint8_t * CMemoryStream::GetReadPtr()
{
void * ret;
	m_buffer.LockRead(0, &ret);
	return (uint8_t *)ret;
}


int CMemoryStream::GetLastErr()
{
	return RET_OK;
}


const char * CMemoryStream::GetLastErrMsg()
{
	return "";
}


bool CMemoryStream::IsEof()
{
	return (m_buffer.Length() == 0);
}


bool CMemoryStream::IsError()
{
	return false;
}


int CMemoryStream::ReadByte()
{
	if(m_buffer.Length()==0)
		return INT_EOF;

	return m_buffer.ReadByte();
}


int CMemoryStream::Read(void * buffer, int size)
{
int len = size;
	if(len > m_buffer.Length())
		len = m_buffer.Length();

	if(len > 0)
		return m_buffer.Read(buffer, 0, len);

	return 0;
}


int CMemoryStream::Write(const uint8_t c)
{
	return m_buffer.Write(c);
}


int CMemoryStream::Write(const void * c, int size)
{
	return m_buffer.Write(c, 0, size);
}
