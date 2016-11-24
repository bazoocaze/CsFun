/*
 * File.....: Fd.cpp
 * Author...: Jose Ferreira
 * Date.....: 2016-11-22 16:22
 * Purpose..: File descriptor writer/reader
 * */


// #include <stdlib.h>
// #include <sys/types.h>
// #include <sys/socket.h>
// #include <netdb.h>
// #include <unistd.h>
// #include <errno.h>
// #include <sys/time.h>
// #include <fcntl.h>
// #include <errno.h>
// #include <arpa/inet.h>

#include <stdio.h>
#include <string.h>
#include <sys/ioctl.h>

#include "Fd.h"
#include "Util.h"
#include "IO.h"


#if defined HAVE_CONSOLE

	CConsole Console;
	FdReader StdIn(0);
	FdWriter StdOut(1);
	FdWriter StdErr(2);

#else

	NullText Console;
	NullText StdIn;
	NullText StdOut;
	NullText StdErr;

#endif



//////////////////////////////////////////////////////////////////////
// Fd
//////////////////////////////////////////////////////////////////////



Fd::Fd()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


Fd::Fd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


void Fd::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


int Fd::GetFd() const
{
	return m_fd;
}


void Fd::Close()
{
	m_fd = CLOSED_FD;
}


void Fd::Clear()
{
	Close();
	LastErr = RET_OK;
}


bool Fd::IsReadable() const
{
	return Fd::IsReadable(m_fd);
}


bool Fd::IsWritable() const
{
	return Fd::IsWritable(m_fd);
}


bool Fd::IsClosed() const
{
	return m_fd == CLOSED_FD;
}


bool Fd::SetNonBlock(int enabled)
{
	return Fd::SetNonBlock(m_fd, enabled);
}


int Fd::BytesAvailable() const
{
	return Fd::BytesAvailable(m_fd);
}


int Fd::IsReadable(int fd)
{
fd_set fdread;
int ret;
struct timeval t;
	FD_ZERO(&fdread);
	FD_SET(fd, &fdread);
	t.tv_sec=0;
	t.tv_usec=0;
	ret = select(fd + 1, &fdread, NULL, NULL, &t);
	return (ret == 1);
}


int Fd::IsWritable(int fd)
{
fd_set fdwrite;
int ret;
struct timeval t;
	FD_ZERO(&fdwrite);
	FD_SET(fd, &fdwrite);
	t.tv_sec=0;
	t.tv_usec=0;
	ret = select(fd + 1, NULL, &fdwrite, NULL, &t);
	return (ret == 1);
}


bool Fd::SetNonBlock(int fd, int enable)
{
	int ret;
	int fdfl = fcntl(fd, F_GETFL);
	if(fdfl != RET_ERROR)	{
		if(enable)
			fdfl |= O_NONBLOCK;
		else
			fdfl &= (~O_NONBLOCK);
		ret = fcntl(fd, F_SETFL, fdfl);
		return ret != RET_ERROR;
	}
	return false;
}


// Get the number of bytes that are immediately available for reading.
int Fd::BytesAvailable(int fd) {
int ret;
int bytesAv;
	ret = ioctl(fd, FIONREAD, &bytesAv);
	if(ret != RET_OK)
		return ret;
	return bytesAv;
}


const char * Fd::GetLastErrMsg() const
{
	return strerror(LastErr);
}



//////////////////////////////////////////////////////////////////////
// FdWriter
//////////////////////////////////////////////////////////////////////


FdWriter::FdWriter()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


FdWriter::FdWriter(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


void FdWriter::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


int FdWriter::Write(uint8_t c)
{
int ret;
	if(m_fd == CLOSED_FD) return 0;
	ret = write(m_fd, &c, 1);
	if(ret == RET_ERR) LastErr = errno;
	return ret;
}


int FdWriter::Write(const uint8_t * buffer, int size)
{
int ret;
	if(m_fd == CLOSED_FD) return 0;
	ret = write(m_fd, buffer, size);
	if(ret == RET_ERR) LastErr = errno;
	return ret;
}


int FdWriter::GetLastErr()
{
	return LastErr;
}


const char * FdWriter::GetLastErrMsg()
{
	return strerror(LastErr);
}


//////////////////////////////////////////////////////////////////////
// FdWriter
//////////////////////////////////////////////////////////////////////


FdReader::FdReader()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
	Eof = false;
}


FdReader::FdReader(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
	Eof = false;
}


void FdReader::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
	Eof = false;
}


bool FdReader::IsEof()
{
	return Eof;
}


int FdReader::Read()
{
int n;
uint8_t c;
	if(m_fd == CLOSED_FD) return INT_EOF;
	n = read(m_fd, &c, 1);
	if(n == RET_ERR) { LastErr = errno; return INT_ERR; } 
	if(n == 0) { Eof = true; return INT_EOF; }
	return c;
}


int FdReader::Read(uint8_t * buffer, int size)
{
int ret;
	if(m_fd == CLOSED_FD) return 0;
	ret = read(m_fd, buffer, size);
	if(ret == RET_ERR) LastErr = errno;
	if(ret == 0) Eof = true;
	return ret;
}


int FdReader::GetLastErr()
{
	return LastErr;
}


const char * FdReader::GetLastErrMsg()
{
	return strerror(LastErr);
}
