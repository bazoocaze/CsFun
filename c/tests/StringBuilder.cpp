
#include "StringBuilder.h"
#include "Print.h"
#include "Logger.h"


size_t StringBuilder::Write(uint8_t c)
{
	m_buffer.Write(c);
	return 1;
}

size_t StringBuilder::Write(uint8_t * buffer, size_t size)
{
	m_buffer.Write(buffer, 0, size);
	return size;
}


const char * StringBuilder::GetStr()
{
	return m_buffer.GetStr();
}
