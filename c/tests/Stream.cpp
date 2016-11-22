#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unistd.h>
#include <errno.h>

#include "Stream.h"
#include "MemoryStream.h"
#include "Util.h"


// Null stream implemented as NullStream
NullStream Stream::Null = NullStream();



//////////////////////////////////////////////////////////////////////
// FdStream
//////////////////////////////////////////////////////////////////////



FdStream::FdStream()
{
	m_fd = CLOSED_FD;
	m_lastErr = RET_OK;
}


FdStream::FdStream(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
}


void FdStream::Close()
{
	if(m_fd != CLOSED_FD) {
		close(m_fd);
		m_fd = CLOSED_FD;
		m_lastErr = RET_OK;
	}
}


void FdStream::SetFd(int fd)
{
	m_fd = fd;
	m_lastErr = RET_OK;
}


int FdStream::GetFd()
{ return m_fd; }


int FdStream::GetLastErr()
{ return m_lastErr; }


const char * FdStream::GetLastErrMsg()
{ return strerror(m_lastErr); }


int FdStream::Write(const uint8_t c)
{
int ret;
	if(m_fd == CLOSED_FD) return RET_ERR;
	ret = write(m_fd, &c, 1);
	if(ret == RET_ERR) m_lastErr = ret;
	return ret;
}


int FdStream::Write(const uint8_t * c, int size)
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
	if(ret==0) { return INT_EOF; }
	if(ret<0)  { m_lastErr = errno; return INT_ERR; }
	return (int)c;
}


int  FdStream::Read(uint8_t * buffer, int size)
{
int ret = read(m_fd, buffer, size);
	if(ret < 0) m_lastErr = errno;
	return ret;
}



//////////////////////////////////////////////////////////////////////
// MemoryStream
//////////////////////////////////////////////////////////////////////



MemoryStream::MemoryStream()
{
}


int MemoryStream::GetLastErr()
{
	return RET_OK;
}


const char * MemoryStream::GetLastErrMsg()
{
	return "";
}


int MemoryStream::ReadByte()
{
	if(m_buffer.Length()==0) return INT_EOF;
	return m_buffer.ReadByte();
}


int MemoryStream::Read(uint8_t * buffer, int size)
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


int MemoryStream::Write(const uint8_t * c, int size)
{
	return m_buffer.Write(c, 0, size);
}
