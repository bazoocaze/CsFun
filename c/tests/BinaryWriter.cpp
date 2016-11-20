
#include "BinaryWriter.h"
#include "Util.h"


BinaryWriter::BinaryWriter()
{
	m_fd = CLOSED_FD;
}


BinaryWriter::BinaryWriter(int fd)
{
	m_fd = fd;
}


void BinaryWriter::Write(char c)
{
	write(m_fd, &c, 1);
}


void BinaryWriter::Write(const void * buffer, int size)
{
	write(m_fd, buffer, size);
}


void BinaryWriter::WriteInt32(int val)
{
char buffer[4];
	buffer[0] = (val >> 0) & 0xFF;
	buffer[1] = (val >> 1) & 0xFF;
	buffer[2] = (val >> 2) & 0xFF;
	buffer[3] = (val >> 3) & 0xFF;
	Write(buffer, 4);
}


void BinaryWriter::WriteString(const char * val)
{
}

