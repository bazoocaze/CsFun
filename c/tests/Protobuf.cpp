#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "Protobuf.h"
#include "ByteBuffer.h"
#include "Debug.h"
#include "Util.h"



//////////////////////////////////////////////////////////////////////
// CodedOutputStream
//////////////////////////////////////////////////////////////////////



CodedOutputStream::CodedOutputStream()
{
	m_fd = CLOSED_FD;
	m_buffer = NULL;
}


CodedOutputStream::CodedOutputStream(int fd)
{
	m_fd = fd;
	m_buffer = NULL;
}


CodedOutputStream::CodedOutputStream(ByteBuffer* buffer)
{
	m_fd = CLOSED_FD;
	m_buffer = buffer;
}


void CodedOutputStream::WriteByte(char c)
{
	if(m_buffer)
		m_buffer->Write(c);
	else
		write(m_fd, &c, 1);
}


void CodedOutputStream::WriteBytes(const char * data, int offset, int size)
{
	if(m_buffer)
		m_buffer->Write((const void *)data, offset, size);
	else
		write(m_fd, &data[offset], size);
}


int CodedOutputStream::CalculateVarint32Size(int val)
{
unsigned int v = val;
int ret = 0;
	do
	{
		ret++;
		v = v >> 7;
	} while(v > 0);
	return ret;
}


int CodedOutputStream::CalculateInt32Size(int fieldNumber, int val)
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
	len = strlen(val);
	ret += CalculateVarint32Size(strlen(val));
	ret += len;
	return ret;
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


void CodedOutputStream::WriteVarint32(int val)
{
unsigned int v = val;
int c;
	do
	{
		c = (v & 0x7F) | (v > 127 ? 0x80 : 0x00);
		v = v >> 7;
		WriteByte(c);
	} while(v > 0);
}


void CodedOutputStream::WriteInt32(int fieldNumber, int val)
{
int tag = (fieldNumber << 3) | P_INT32_WIRETYPE;
	if(val == 0) return;
	WriteVarint32(tag);
	WriteVarint32(val);
}


void CodedOutputStream::WriteString(int fieldNumber, const char * val)
{
int len;
int tag = (fieldNumber << 3) | P_STRING_WIRETYPE;
	if(val == NULL || val[0] == 0) return;
	WriteVarint32(tag);
	len = strlen(val);
	WriteVarint32(len);
	WriteBytes(val, 0, len);
}


void CodedOutputStream::WriteBool(int fieldNumber, bool val)
{
int tag = (fieldNumber << 3) | P_BOOL_WIRETYPE;
	if(!val) return;
	WriteVarint32(tag);
	WriteVarint32(1);
}



//////////////////////////////////////////////////////////////////////
// CodedInputStream
//////////////////////////////////////////////////////////////////////



CodedInputStream::CodedInputStream()
{
}


CodedInputStream::CodedInputStream(ByteBuffer* buffer)
{
	m_buffer = buffer;
}


int CodedInputStream::ReadByte()
{
int ret = m_buffer->ReadByte();
	// printf("[ReadByte=%d]\n", ret);
	return ret;
}


bool CodedInputStream::ReadBytes(char * buffer, int offset, int size)
{
	return m_buffer->Read((void *)buffer, offset, size) == size;
}


void CodedInputStream::DiscardBytes(int size)
{
	return m_buffer->DiscardBytes(size);
}


int CodedInputStream::ReadVarint32()
{
int v;
int rot = 0;
unsigned int ret = 0;
	// printf("[ReadVaring32:begin]\n");
	do
	{
		v = ReadByte();
		// printf("[v=%d]\n", v);
		if(v == -1) return 0;
		ret = ret | ((v & 0x7F) << rot);
		rot = rot + 7;
	} while(v > 127);
	// printf("[ReadVaring32:end ret=%d]\n", ret);
	return ret;
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
		DiscardBytes(size);
		return;
	}

}


bool CodedInputStream::ReadInt32(int fieldNumber, int * val)
{
	if(	CurrentFieldNumber != fieldNumber ||
			CurrentWireType != CodedOutputStream::P_WIRETYPE_VARINT) return false;
	*val = ReadVarint32();
	return true;
}


bool CodedInputStream::ReadBool(int fieldNumber, bool * val)
{
	if(	CurrentFieldNumber != fieldNumber ||
			CurrentWireType != CodedOutputStream::P_WIRETYPE_VARINT) return false;
	*val = (ReadVarint32() != 0);
	return true;
}


bool CodedInputStream::ReadString(int fieldNumber, char ** val)
{
	if(	CurrentFieldNumber != fieldNumber ||
			CurrentWireType != CodedOutputStream::P_WIRETYPE_DELIMITED) return false;
	int size = ReadVarint32();
	printf("[ReadString: size=%d]\n", size);
	if(size == 0)
	{
		*val = (char *)"";
	}
	else
	{
		char * ptr = (char*)malloc(size+1);
		memset(ptr, 0, size+1);
		ReadBytes(ptr, 0, size);
		*val = ptr;
	}
	return true;
}



//////////////////////////////////////////////////////////////////////
// ByteString
//////////////////////////////////////////////////////////////////////



ByteString::ByteString()
{
	m_ptr = NULL;
	Offset = 0;
	Size = 0;
	ReadPos = 0;
}


ByteString::ByteString(const char * ptr, int offset, int size)
{
	m_ptr = ptr;
	Offset = offset;
	Size = size;
	ReadPos = 0;
}


ByteString::ByteString(MemPtr memPtr, const char * ptr, int offset, int size)
{
	m_mem = memPtr;
	m_ptr = ptr;
	Offset = offset;
	Size = size;
	ReadPos = 0;
}


int ByteString::ReadByte()
{
	if(Size == 0) return -1;
	Size--;
	return m_ptr[Offset + ReadPos++];	
}


ByteString ByteString::ReadBlock(int size)
{
ByteString ret;
	if(size < 0) size = 0;
	if(size > Size) size = Size;
	ret = ByteString(m_mem, m_ptr, Offset + ReadPos, size);
	Size    -= size;
	ReadPos += size;
	return ret;
}


const char * ByteString::ReadString(int size)
{
char * ret;
	if(size < 0) size = 0;
	if(size > Size) size = Size;
	if(size == 0) return strdup("");
	ret = (char *)malloc(size + 1);
	memset(ret, 0, size + 1);
	memcpy(ret, &m_ptr[Offset + ReadPos], size);
	Size    -= size;
	ReadPos += size;
	return ret;
}


// void ByteString::Write(char c) { }
// void ByteString::Write(const char *, int offset, int size) { }

