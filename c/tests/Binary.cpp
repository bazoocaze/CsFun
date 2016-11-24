/*
 * File....: Binary.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 21:29
 * Purpose.: Binary data reader/writer
 * */



#include <stdlib.h>
#include <string.h>
#include <inttypes.h>

#include "Binary.h"
#include "Stream.h"
#include "Util.h"



//////////////////////////////////////////////////////////////////////
// BinaryWriter
//////////////////////////////////////////////////////////////////////



BinaryWriter::BinaryWriter()
{
	m_stream = &Stream::Null;
}


BinaryWriter::BinaryWriter(Stream * stream)
{
	m_stream = stream;
}


void BinaryWriter::Close()
{
	m_stream->Close();
}


int BinaryWriter::WriteUInt8(uint8_t val)
{
	m_stream->Write(val); return 1;
}


int BinaryWriter::WriteInt8(int8_t val)
{
	m_stream->Write((uint8_t)val); return 1;
}


int BinaryWriter::WriteInt16(int16_t val)
{
uint16_t v = val;
	m_stream->Write(v >> 0 & 0xFF);
	m_stream->Write(v >> 8 & 0xFF);
	return 2;
}


int BinaryWriter::WriteInt32(int32_t val)
{
uint32_t v = val;
	m_stream->Write(v >> 0  & 0xFF);
	m_stream->Write(v >> 8  & 0xFF);
	m_stream->Write(v >> 16 & 0xFF);
	m_stream->Write(v >> 24 & 0xFF);
	return 4;
}


int BinaryWriter::WriteInt64(int64_t val)
{
uint64_t v = val;
	for(int n = 0; n < 8; n++)
	{
		m_stream->Write(v & 0xFF);
		v = v >> 8;
	}
	return 8;
}


int BinaryWriter::WriteFloat(float val)
{
	return WriteBytes((uint8_t*)&val, 4);
}


int BinaryWriter::WriteDouble(double val)
{
	return WriteBytes((uint8_t*)&val, 8);
}


int BinaryWriter::WriteString(const char * str, int maxSize)
{
int len = 0;
int ret;
	if(str != NULL)   len=strlen(str);
	if(len > maxSize) len=maxSize;
	if(len < 0)       len=0;
	ret = WriteInt32(len);
	if(len > 0) m_stream->Write((uint8_t*)str, len);
	return ret + len;
}


int BinaryWriter::WriteString(const String& c, int maxSize)
{
	return WriteString(c.c_str, maxSize);
}


int BinaryWriter::WriteBytes(const uint8_t * ptr, int size)
{
	m_stream->Write(ptr, size);
	return size;
}



//////////////////////////////////////////////////////////////////////
// BinaryReader
//////////////////////////////////////////////////////////////////////



BinaryReader::BinaryReader()
{
	m_stream = &Stream::Null;
	Error = false;
	Eof = false;
}


BinaryReader::BinaryReader(Stream * stream)
{
	m_stream = stream;
	Error = false;
	Eof = false;
}


void BinaryReader::SetStream(Stream * stream)
{
	m_stream = stream;
	Error = false;
	Eof = false;
}


bool BinaryReader::TryReadBytes(uint8_t * buffer, int size)
{
int ret;
	if(Error || Eof) return false;
	ret=m_stream->Read(buffer, size);
	if(ret != size)
	{
		Eof = (ret != RET_ERR);
		Error = (ret == RET_ERR);
		return false;
	}
	return true;
} 


bool BinaryReader::TryDiscardBytes(int size)
{
int len;
uint8_t buffer[128];
	if(Error || Eof) return false;
	if(size == 0) return true;
	if(size <  0) { Error = true; return false; }
	while(size > 0)
	{
		if(size > sizeof(buffer))
			len = sizeof(buffer);
		else
			len = size;
		if(!TryReadBytes(buffer, len)) return false;
		size -= len;
	}
	return true;
}


bool BinaryReader::TryReadInt8(int8_t *val)
{
int ret;
	if(Error || Eof) return false;
	ret=m_stream->ReadByte();
	if(ret < 0)
	{
		Eof = (ret == INT_EOF);
		Error = (ret != INT_EOF);
		return false;
	}
	*val = (int8_t)ret;
	return true;
}


bool BinaryReader::TryReadInt32(int32_t *val)
{
uint8_t buffer[4];
	if(!TryReadBytes(buffer, 4)) return false;
	*val = (buffer[0] << 0) |
          (buffer[1] << 8) |
          (buffer[2] << 16) |
          (buffer[3] << 24); 
	return true;
}


bool BinaryReader::TryReadString(String& c, int maxSize)
{
int blockSize;
int strSize;
	if(!TryReadInt32(&blockSize)) return false;
	if(blockSize < 0)             return false;

	if(blockSize > maxSize)
		strSize = maxSize;
	else
		strSize = blockSize;

	if(strSize > 0) {
		uint8_t * strBuffer = (uint8_t*)UTIL_MEM_MALLOC(strSize+1);
		if(!TryReadBytes(strBuffer, strSize))
		{
			UTIL_MEM_FREE(strBuffer);
			return false;
		}
		strBuffer[strSize] = 0;
		c.Set((char*)strBuffer, true);
		if(strSize != blockSize)
			return TryDiscardBytes(blockSize - strSize);
	} else {
		c.Clear();
	}
	return true;
}


int32_t BinaryReader::ReadInt32()
{
int ret = 0;
	TryReadInt32(&ret);
	return ret;
}


String BinaryReader::ReadString(int maxSize)
{
String ret;
	TryReadString(ret, maxSize);
	return ret;
}
