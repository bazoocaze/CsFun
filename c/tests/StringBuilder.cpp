/*
 * File....: StringBuilder.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 22:30
 * Purpose.: String builder
 * */


#include "StringBuilder.h"
#include "Logger.h"


int CStringBuilder::Length()
{
	return m_buffer.Length();
}


int CStringBuilder::Write(uint8_t c)
{
	m_buffer.Write(c);
	return 1;
}

int CStringBuilder::Write(uint8_t * buffer, int size)
{
	m_buffer.Write(buffer, 0, size);
	return size;
}

CString CStringBuilder::GetString()
{
CString ret = m_buffer.GetString();
	m_buffer.Clear();
	return ret;
}
