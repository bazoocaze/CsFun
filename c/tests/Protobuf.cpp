/*
 * File.....: Protobuf.cpp
 * Author...: Jose Ferreira
 * Date.....: 2016-11-23 20:13
 * Purpose..: Simple protobuf implementation
 * */


#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "Protobuf.h"
#include "ByteBuffer.h"
#include "Debug.h"
#include "Util.h"
#include "Text.h"
#include "Config.h"



//////////////////////////////////////////////////////////////////////
// CodedOutputStream
//////////////////////////////////////////////////////////////////////



CodedOutputStream::CodedOutputStream()
{
	m_output = &Stream::Null;
}


CodedOutputStream::CodedOutputStream(Stream* stream)
{
	if(stream != NULL)
		m_output = stream;
	else
		m_output = &Stream::Null;
}


void CodedOutputStream::WriteByte(uint8_t c)
{
	m_output->Write(c);
}


void CodedOutputStream::WriteBytes(const void * data, int offset, int size)
{
	uint8_t * ptr = (uint8_t *)data;
	m_output->Write(&ptr[offset], size);
}


int CodedOutputStream::CalculateVarint32Size(int32_t val)
{
unsigned int v = val;
int ret = 0;
	do {
		ret++;
		v = v >> 7;
	} while(v > 0);
	return ret;
}


int CodedOutputStream::CalculateInt32Size(int fieldNumber, int32_t val)
{
int tag = fieldNumber << 3;
	if(val == 0) return 0;
	return CalculateVarint32Size(tag) + CalculateVarint32Size(val);
}


int CodedOutputStream::CalculateStringSize(int fieldNumber, const char * val)
{
int tag = fieldNumber << 3;
int len = 0;
int ret = 0;
	if(val == NULL || val[0] == 0) return 0;
	ret += CalculateVarint32Size(tag);
	len  = strlen(val);
	ret += CalculateVarint32Size(strlen(val));
	ret += len;
	return ret;
}


int CodedOutputStream::CalculateStringSize(int fieldNumber, const String& val)
{
	return CalculateStringSize(fieldNumber, val.c_str);
}


int CodedOutputStream::CalculateBoolSize(int fieldNumber, bool val)
{
int tag = fieldNumber << 3;
int ret = 0;
	if(!val) return 0;
	ret += CalculateVarint32Size(tag);
	ret += CalculateVarint32Size(1);
	return ret;
}


int CodedOutputStream::CalculateMessageSize(int fieldNumber, const IMessage& val)
{
	int msgSize = val.CalculateSize();
	if(msgSize == 0) return 0;
	int tag = fieldNumber << 3;
	int ret = 0;
	ret += CalculateVarint32Size(tag);
	ret += CalculateVarint32Size(msgSize);
	ret += msgSize;
	return ret;
}


void CodedOutputStream::WriteVarint32(int32_t val)
{
uint32_t v = val;
int c;
	do
	{
		c = (v & 0x7F) | (v > 127 ? 0x80 : 0x00);
		v = v >> 7;
		WriteByte(c);
	} while(v > 0);
}


void CodedOutputStream::WriteInt32(int fieldNumber, int32_t val)
{
int tag = (fieldNumber << 3) | CodedOutputStream::P_INT32_WIRETYPE;
	if(val == 0) return;
	WriteVarint32(tag);
	WriteVarint32(val);
}


void CodedOutputStream::WriteString(int fieldNumber, const char * val)
{
int len;
int tag = (fieldNumber << 3) | CodedOutputStream::P_STRING_WIRETYPE;
	if(val == NULL || val[0] == 0) return;
	WriteVarint32(tag);
	len = strlen(val);
	WriteVarint32(len);
	WriteBytes(val, 0, len);
}


void CodedOutputStream::WriteString(int fieldNumber, const String& val)
{
	WriteString(fieldNumber, val.c_str);
}


void CodedOutputStream::WriteBool(int fieldNumber, bool val)
{
int tag = (fieldNumber << 3) | CodedOutputStream::P_BOOL_WIRETYPE;
	if(!val) return;
	WriteVarint32(tag);
	WriteVarint32(1);
}


void CodedOutputStream::WriteMessage(int fieldNumber, IMessage& val)
{
	int msgSize = val.CalculateSize();
	if(msgSize == 0) return;
	
	int tag = (fieldNumber << 3) | CodedOutputStream::P_MESSAGE_WIRETYPE;
	WriteVarint32(tag);
	WriteVarint32(msgSize);
	val.WriteTo(this);
}


//////////////////////////////////////////////////////////////////////
// CodedInputStream
//////////////////////////////////////////////////////////////////////



CodedInputStream::CodedInputStream()
{
	m_input = ByteString();
}


CodedInputStream::CodedInputStream(Stream* input)
{
	m_input = ByteString(input);
}


CodedInputStream::CodedInputStream(ByteString& input)
{
	m_input = input;
}


int CodedInputStream::SourceReadByte()
{
	return m_input.ReadByte();
}


bool CodedInputStream::SourceCopyBytes(void * buffer, int offset, int size)
{
	uint8_t* dest = ((uint8_t*)buffer) + offset;
	
	ByteString block = m_input.ReadBlock(size);
	if(block.Size > 0)
		memcpy(dest, block.GetPtr(), block.Size);
	return block.Size == size;
}


bool CodedInputStream::SourceDiscardBytes(int size)
{
	ByteString ret = m_input.ReadBlock(size);
	return ret.Size == size;
}


int32_t CodedInputStream::ReadVarint32()
{
	int ret;
	if(!ReadVarint32(ret)) return 0;
	return ret;
}


bool CodedInputStream::ReadVarint32(int32_t & retVal)
{
int v;
int rot = 0;
uint32_t ret = 0;
	do
	{
		v = SourceReadByte();
		if(v < 0) { retVal = 0; return false; }
		ret = ret | ((v & 0x7F) << rot);
		rot = rot + 7;
	} while(v > 127);
	retVal = ret;
	return true;
}


bool CodedInputStream::ReadTag()
{
	CurrentTag = ReadVarint32();
	CurrentFieldNumber = CurrentTag >> 3;
	CurrentWireType = CurrentTag & 0x07;
	return (CurrentTag != 0);
}


void CodedInputStream::SkipLastField()
{
	if(CurrentTag == 0) return;

	if(CurrentWireType == CodedOutputStream::P_WIRETYPE_VARINT)
	{
		ReadVarint32();
		return;
	}

	if(CurrentWireType == CodedOutputStream::P_WIRETYPE_DELIMITED)
	{
		int size = ReadVarint32();
		SourceDiscardBytes(size);
		return;
	}
}


bool CodedInputStream::ReadInt32(int fieldNumber, int32_t & val)
{
	if(	CurrentFieldNumber != fieldNumber ||
		CurrentWireType != CodedOutputStream::P_WIRETYPE_VARINT) return false;
	ReadVarint32(val);
	return true;
}


bool CodedInputStream::ReadBool(int fieldNumber, bool & val)
{
	if(	CurrentFieldNumber != fieldNumber ||
		CurrentWireType != CodedOutputStream::P_WIRETYPE_VARINT) return false;
	val = (ReadVarint32() != 0);
	return true;
}


bool CodedInputStream::ReadString(int fieldNumber, char * strRet, int maxSize)
{
	if(	CurrentFieldNumber != fieldNumber ||
		CurrentWireType != CodedOutputStream::P_WIRETYPE_DELIMITED) return false;

	if(strRet == NULL) return false;

	int blockSize = ReadVarint32();

	if(blockSize <= 0 || maxSize < 2)
	{
		if(maxSize > 0)
			strRet[0] = 0;
	}
	else
	{
		int strSize = MIN(blockSize, maxSize - 1);
		memset(strRet, 0, strSize + 1);
		SourceCopyBytes(strRet, 0, strSize);
		if(strSize < blockSize)
			SourceDiscardBytes(blockSize - strSize);
	}
	return true;
}


bool CodedInputStream::ReadString(int fieldNumber, String & val)
{
	return ReadString(fieldNumber, val, P_PROTOBUF_DEF_MAX_STR_SIZE);
}



bool CodedInputStream::ReadString(int fieldNumber, String & strRet, int maxSize)
{
	if(	CurrentFieldNumber != fieldNumber ||
		CurrentWireType != CodedOutputStream::P_WIRETYPE_DELIMITED) return false;
	
	int blockSize = ReadVarint32();
	
	if(blockSize <= 0 || maxSize < 2)
	{
		strRet = "";
	}
	else
	{
		int strSize = MIN(blockSize, maxSize - 1);
		MemPtr strPtr;
		strPtr.Resize(strSize+1);
		strPtr.Memset(0, strSize+1);
		SourceCopyBytes(strPtr.Get(), 0, strSize);
		strRet.Set(strPtr);
		if(strSize < blockSize)
			SourceDiscardBytes(blockSize - strSize);
	}
	return true;
}


bool CodedInputStream::ReadMessage(int fieldNumber, IMessage& val)
{
	if(	CurrentFieldNumber != fieldNumber ||
		CurrentWireType != CodedOutputStream::P_WIRETYPE_DELIMITED) return false;
	
	int msgSize = ReadVarint32();
	
	printf("[ReadMessage: msgSize=%d]\n", msgSize);
	
	if(msgSize == 0) return true;
	
	ByteString block = m_input.ReadBlock(msgSize);
	if(block.Size != msgSize) return true;
	
	CodedInputStream cis = CodedInputStream(block);
	val.MergeFrom(&cis);
	
	return true;
}



//////////////////////////////////////////////////////////////////////
// ByteString
//////////////////////////////////////////////////////////////////////



ByteString::ByteString()
{
	m_ptr    = NULL;
	m_stream = &Stream::Null;
	Offset   = 0;
	Size     = 0;
	ReadPos  = 0;
}


ByteString::ByteString(Stream* input)
{
	m_ptr    = NULL;
	m_stream = input;
	Offset   = 0;
	Size     = 0;
	ReadPos  = 0;
}


ByteString::ByteString(const void * ptr, int offset, int size)
{
	m_stream = NULL;
	m_ptr    = (uint8_t*) ptr;
	Offset   = offset;
	Size     = size;
	ReadPos  = 0;
}


ByteString::ByteString(MemPtr memPtr, const void * ptr, int offset, int size)
{
	m_stream = NULL;
	m_mem    = memPtr;
	m_ptr    = (uint8_t*) ptr;
	Offset   = offset;
	Size     = size;
	ReadPos  = 0;
}


const uint8_t * ByteString::GetPtr()
{
	if(m_ptr == NULL) return NULL;
	return &m_ptr[Offset + ReadPos];
}


int ByteString::ReadByte()
{
	if(m_stream != NULL)
	{
		return m_stream->ReadByte();
	}
	
	if(m_ptr != NULL)
	{
		if(Size == 0) return INT_EOF;
		Size--;
		return m_ptr[Offset + ReadPos++];
	}
	
	return INT_ERR;
}


ByteString ByteString::ReadBlock(int size)
{
	ByteString block;
	if(size <= 0) return block;
	
	if(m_stream != NULL)
	{
		MemPtr mem = MemPtr();
		if(!mem.Resize(size))
		{
			OutOffMemoryHandler("ByteString", "ReadBlock", size, false);
			return block;
		}
		int ret = m_stream->Read(mem.Get(), size);
		block = ByteString(mem, mem.Get(), 0, ret);
		return block;
	}
	
	if(m_ptr != NULL)
	{
		if(Size == 0) return block;
		if(size > Size) size = Size;
		block = ByteString(m_mem, m_ptr, Offset + ReadPos, size);
		Size    -= size;
		ReadPos += size;
	}
	
	return block;
}
