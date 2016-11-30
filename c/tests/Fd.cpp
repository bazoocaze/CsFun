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
#include <fcntl.h>
#include <errno.h>
#include <sys/ioctl.h>
#include <sys/select.h>

#include "Fd.h"
#include "Util.h"
#include "IO.h"



//////////////////////////////////////////////////////////////////////
// Fd
//////////////////////////////////////////////////////////////////////



CFd::CFd()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


CFd::CFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


void CFd::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


int CFd::GetFd() const
{
	return m_fd;
}


void CFd::Close()
{
	m_fd = CLOSED_FD;
}


void CFd::Clear()
{
	Close();
	LastErr = RET_OK;
}


bool CFd::IsReadable() const
{
	return CFd::IsReadable(m_fd);
}


bool CFd::IsWritable() const
{
	return CFd::IsWritable(m_fd);
}


bool CFd::IsClosed() const
{
	return (m_fd == CLOSED_FD);
}


bool CFd::SetNonBlock(int enabled)
{
	return CFd::SetNonBlock(m_fd, enabled);
}


int CFd::BytesAvailable() const
{
	return CFd::BytesAvailable(m_fd);
}


bool CFd::IsReadable(int fd)
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


bool CFd::IsWritable(int fd)
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


bool CFd::SetNonBlock(int fd, int enable)
{
	int fdfl = fcntl(fd, F_GETFL);
	if(fdfl != RET_ERROR)	{
		if(enable)
			fdfl |= O_NONBLOCK;
		else
			fdfl &= (~O_NONBLOCK);
		int ret = fcntl(fd, F_SETFL, fdfl);
		return ret != RET_ERROR;
	}
	return false;
}


// Get the number of bytes that are immediately available for reading.
int CFd::BytesAvailable(int fd) {
int ret;
int bytesAv;
	ret = ioctl(fd, FIONREAD, &bytesAv);
	if(ret != RET_OK)
		return ret;
	return bytesAv;
}


const char * CFd::GetLastErrMsg() const
{
	return strerror(LastErr);
}


//////////////////////////////////////////////////////////////////////
// FdWriter
//////////////////////////////////////////////////////////////////////


CFdWriter::CFdWriter()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
}


CFdWriter::CFdWriter(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


void CFdWriter::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
}


int CFdWriter::Write(uint8_t c)
{
int ret;
	if(m_fd == CLOSED_FD)
		return 0;

	ret = write(m_fd, &c, 1);

	if(ret == RET_ERR)
		LastErr = errno;

	return ret;
}


int CFdWriter::Write(const void * buffer, int size)
{
int ret;
	if(m_fd == CLOSED_FD)
		return 0;

	ret = write(m_fd, buffer, size);

	if(ret == RET_ERR)
		LastErr = errno;

	return ret;
}


int CFdWriter::GetLastErr()
{
	return LastErr;
}


const char * CFdWriter::GetLastErrMsg()
{
	return strerror(LastErr);
}


bool CFdWriter::IsError()
{
	return (LastErr != RET_OK);
}


//////////////////////////////////////////////////////////////////////
// FdWriter
//////////////////////////////////////////////////////////////////////


CFdReader::CFdReader()
{
	m_fd = CLOSED_FD;
	LastErr = RET_OK;
	Eof = false;
}


CFdReader::CFdReader(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
	Eof = false;
}


void CFdReader::SetFd(int fd)
{
	m_fd = fd;
	LastErr = RET_OK;
	Eof = false;
}


int CFdReader::Read()
{
int n;
uint8_t c;
	if(m_fd == CLOSED_FD)
		return INT_EOF;

	n = read(m_fd, &c, 1);

	if(n == RET_ERR) {
		LastErr = errno;
		return INT_ERR;
	}

	if(n == 0) {
		Eof = true;
		return INT_EOF;
	}

	return c;
}


int CFdReader::Read(void * buffer, int size)
{
int ret;
	if(m_fd == CLOSED_FD)
		return 0;

	ret = read(m_fd, buffer, size);

	if(ret == RET_ERR)
		LastErr = errno;

	if(ret == 0)
		Eof = true;

	return ret;
}


int CFdReader::GetLastErr()
{
	return LastErr;
}


const char * CFdReader::GetLastErrMsg()
{
	return strerror(LastErr);
}
