
#include "StringBuilder.h"
#include "Logger.h"


int StringBuilder::Length()
{
	return m_buffer.Length();
}


int StringBuilder::Write(uint8_t c)
{
	m_buffer.Write(c);
	return 1;
}

int StringBuilder::Write(uint8_t * buffer, int size)
{
	m_buffer.Write(buffer, 0, size);
	return size;
}

// const char * StringBuilder::GetStr()
// {
// 	return m_buffer.GetStr();
// }

String StringBuilder::GetString()
{
String ret = m_buffer.GetString();
	m_buffer.Clear();
	return ret;
}
