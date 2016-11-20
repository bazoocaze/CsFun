

#include "Fd.h"
#include "Util.h"



//////////////////////////////////////////////////////////////////////
// FdPrint
//////////////////////////////////////////////////////////////////////


FdPrint::FdPrint()
{
	m_fd = CLOSED_FD;
}


FdPrint::FdPrint(int fd)
{
	m_fd = fd;
}


void FdPrint::SetFd(int fd)
{
	m_fd = fd;
}


size_t FdPrint::Write(uint8_t c)
{
	if(m_fd == CLOSED_FD) return 0;
	return write(m_fd, &c, 1);
}


size_t FdPrint::Write(const uint8_t * buffer, size_t size)
{
	if(m_fd == CLOSED_FD) return 0;
	return write(m_fd, buffer, size);
}


//////////////////////////////////////////////////////////////////////
// FdStream
//////////////////////////////////////////////////////////////////////


FdStream::FdStream()
{
	m_fd = CLOSED_FD;
}


FdStream::FdStream(int fd)
{
	m_fd = fd;
}


void FdStream::SetFd(int fd)
{
	m_fd = fd;
}

