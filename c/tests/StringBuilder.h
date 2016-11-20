

#pragma once


#include "Print.h"
#include "ByteBuffer.h"


class StringBuilder : public Print
{
private:
	ByteBuffer m_buffer;

public:

	size_t Write(uint8_t c);
	size_t Write(uint8_t * buffer, size_t size);

	const char * GetStr();
};

