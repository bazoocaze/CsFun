/*
 * File....: Stream.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 22:29
 * Purpose.: Stream of bytes
 * */
 
 
// #include <stdio.h>
// #include <stdlib.h>
// #include <inttypes.h>
// #include <unistd.h>

#include <string.h>
#include <errno.h>

#include "Stream.h"
#include "MemoryStream.h"
#include "Util.h"
#include "IO.h"
#include "FdSelect.h"


#define READ_BLOCK_TIMEOUT 10000


// Null stream implemented as NullStream
CNullStream CStream::Null = CNullStream();


//////////////////////////////////////////////////////////////////////
// Stream
//////////////////////////////////////////////////////////////////////

int CStream::ReadBlock(void * buffer, int size)
{
	uint8_t* ptr = (uint8_t*)buffer;
	int total = 0;

	while(total < size)
	{
		int ret = this->Read(&ptr[total], size - total);

		if(ret <= 0)
		{
			if(total > 0)
				return total;
			return ret;
		}
		total += ret;
	}

	return total;
}


/* Default discard bytes implementation.
 * Reads and discards block of bytes from input. */
int CStream::DiscardBytes(int size)
{
	uint8_t buffer[128];
	int total = 0;
	while(total < size)
	{
		int len = MIN(size - total, sizeof(buffer));
		int ret = Read(buffer, len);
		if(ret <= 0)
			return total;
		total += ret;
	}
	return total;
}



//////////////////////////////////////////////////////////////////////
// FdStream
//////////////////////////////////////////////////////////////////////



CFdStream::CFdStream()
{
	m_fd = CLOSED_FD;
	m_lastErr = RET_OK;
	m_Eof = false;
}


CFdStream::CFdStream(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
	m_Eof = false;
}


void CFdStream::Close()
{
	if(m_fd != CLOSED_FD) {
		close(m_fd);
		m_fd = CLOSED_FD;
		m_lastErr = RET_OK;
		m_Eof = false;
	}
}


void CFdStream::SetFd(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
	m_Eof = false;
}


int CFdStream::GetFd()
{ return m_fd; }


int CFdStream::GetLastErr()
{ return m_lastErr; }


const char * CFdStream::GetLastErrMsg()
{ return strerror(m_lastErr); }


bool CFdStream::IsEof()
{ return m_Eof; }


bool CFdStream::IsError()
{ return m_lastErr != RET_OK; }


int CFdStream::Write(const uint8_t c)
{
int ret;
	if(m_fd == CLOSED_FD)
		return RET_ERR;

	ret = write(m_fd, &c, 1);

	if(ret == RET_ERR)
		m_lastErr = ret;

	return ret;
}


int CFdStream::Write(const void * c, int size)
{
int ret;
	if(m_fd == CLOSED_FD)
		return RET_ERR;

	ret = write(m_fd, c, size);

	if(ret == RET_ERR)
		m_lastErr = ret;

	return ret;
}


int  CFdStream::ReadByte()
{
	uint8_t c;
	int ret = read(m_fd, &c, 1);

	if(ret==0) {
		m_Eof = true;
		return INT_EOF;
	}

	if(ret<0) {
		m_lastErr = errno;
		return INT_ERR;
	}

	return (int)c;
}


int  CFdStream::Read(void * buffer, int size)
{
	int ret = read(m_fd, buffer, size);

	if(ret < 0)
		m_lastErr = errno;

	if(ret == 0)
		m_Eof = true;

	return ret;
}


int CFdStream::ReadBlock(void * buffer, int size)
{
	uint8_t* ptr = (uint8_t*)buffer;
	int total = 0;
	uint64_t timeout = millis() + READ_BLOCK_TIMEOUT;

	while((total < size) && (millis() < timeout))
	{
		uint64_t ms = millis();
		if(ms > timeout) return total;
		int remaing = timeout - ms;

		int ret;

		ret = CFdSelect::Wait(m_fd, SEL_READ | SEL_ERROR, remaing);
		if(ret == 0)
		{
			m_lastErr = ETIMEDOUT;
			return RET_ERR;
		}

		if(ret == RET_ERR)
		{
			m_lastErr = errno;
			if(total > 0)
				return total;
			return ret;
		}

		ret = this->Read(&ptr[total], size - total);
		if(ret < 0)
			m_lastErr = errno;

		if(ret == 0)
			m_Eof = true;

		if(ret <= 0)
		{
			if(total > 0)
				return total;
			return ret;
		}

		total += ret;
	}

	return total;
}
