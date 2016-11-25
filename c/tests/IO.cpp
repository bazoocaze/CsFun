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
// FileStream
//////////////////////////////////////////////////////////////////////



bool FileStream::Create(const char * fileName, FileStream &ret)
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

FileStream::FileStream() : FdStream()
{
}

FileStream::FileStream(int fd) : FdStream(fd)
{
	m_fdPtr.Set(fd);
}

void FileStream::SetFd(int fd)
{
	FdStream::SetFd(fd);
	m_fdPtr.Set(fd);
}

void FileStream::Close()
{
	m_fdPtr.Set(CLOSED_FD);
	m_fd = CLOSED_FD;
}
