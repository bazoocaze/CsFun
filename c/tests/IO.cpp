/*
 * File....: Stream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 09:35
 * Purpose.: Byte stream abstraction
 */


#include <fcntl.h>
#include <errno.h>

#include "IO.h"
#include "Config.h"


#if defined HAVE_CONSOLE

	// CConsole Console;
	CFdReader StdIn(0);
	CFdWriter StdOut(1);
	CFdWriter StdErr(2);

#else

	// NullText Console;
	CNullText StdIn;
	CNullText StdOut;
	CNullText StdErr;

#endif



//////////////////////////////////////////////////////////////////////
// FileStream
//////////////////////////////////////////////////////////////////////



bool CFileStream::Create(const char * fileName, CFileStream &ret)
{
int fd;
	fd = creat(fileName, S_IRUSR | S_IWUSR | S_IRGRP | S_IWGRP);
	if(fd == RET_ERR) {
		ret.m_lastErr = errno;
		return false;
	}
	ret.SetFd(fd);
	return true;
}

CFileStream::CFileStream() : CFdStream()
{
}

CFileStream::CFileStream(int fd) : CFdStream(fd)
{
	m_fdPtr.Set(fd);
}

void CFileStream::SetFd(int fd)
{
	CFdStream::SetFd(fd);
	m_fdPtr.Set(fd);
}

void CFileStream::Close()
{
	m_fdPtr.Set(CLOSED_FD);
	m_fd = CLOSED_FD;
}
