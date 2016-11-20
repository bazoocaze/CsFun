

#pragma once


#include "Ptr.h"
#include "Stream.h"
#include "Print.h"


class FdPrint : public Print
{
protected:
	int m_fd;

public:
	FdPrint();
	FdPrint(int fd);
	void SetFd(int fd);

	size_t Write(uint8_t c);
	size_t Write(const uint8_t * buffer, size_t size);
};


class FdStream : public Stream
{
protected:
	int m_fd;

public:
	FdStream();
	FdStream(int fd);
	void SetFd(int fd);
};

