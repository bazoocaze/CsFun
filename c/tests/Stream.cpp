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


// Null stream implemented as NullStream
NullStream Stream::Null = NullStream();


//////////////////////////////////////////////////////////////////////
// Stream
//////////////////////////////////////////////////////////////////////


/* Default discard bytes implementation.
 * Reads and discards block of bytes from input. */
int Stream::DiscardBytes(int size)
{
	uint8_t buffer[128];
	int total = 0;
	while(total < size)
	{
		int len = MIN(size - total, sizeof(buffer));
		int ret = Read(buffer, len);
		if(ret <= 0) return total;
		total += ret;
	}
	return total;
}



//////////////////////////////////////////////////////////////////////
// FdStream
//////////////////////////////////////////////////////////////////////



FdStream::FdStream()
{
	m_fd = CLOSED_FD;
	m_lastErr = RET_OK;
	m_Eof = false;
}


FdStream::FdStream(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
	m_Eof = false;
}


void FdStream::Close()
{
	if(m_fd != CLOSED_FD) {
		close(m_fd);
		m_fd = CLOSED_FD;
		m_lastErr = RET_OK;
		m_Eof = false;
	}
}


void FdStream::SetFd(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
	m_Eof = false;
}


int FdStream::GetFd()
{ return m_fd; }


int FdStream::GetLastErr()
{ return m_lastErr; }


const char * FdStream::GetLastErrMsg()
{ return strerror(m_lastErr); }


bool FdStream::IsEof()
{ return m_Eof; }


bool FdStream::IsError()
{ return m_lastErr == RET_ERR; }


int FdStream::Write(const uint8_t c)
{
int ret;
	if(m_fd == CLOSED_FD) return RET_ERR;
	ret = write(m_fd, &c, 1);
	if(ret == RET_ERR) m_lastErr = ret;
	return ret;
}


int FdStream::Write(const void * c, int size)
{
int ret;
	if(m_fd == CLOSED_FD) return RET_ERR;
	ret = write(m_fd, c, size);
	if(ret == RET_ERR) m_lastErr = ret;
	return ret;
}


int  FdStream::ReadByte()
{
uint8_t c;
int ret = read(m_fd, &c, 1);
	if(ret==0) { m_Eof = true; return INT_EOF; }
	if(ret<0)  { m_lastErr = errno; return INT_ERR; }
	return (int)c;
}


int  FdStream::Read(void * buffer, int size)
{
int ret = read(m_fd, buffer, size);
	if(ret < 0)  m_lastErr = errno;
	if(ret == 0) m_Eof = true;
	return ret;
}
