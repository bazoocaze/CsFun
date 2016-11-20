

#pragma once


#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "ByteBuffer.h"
#include "Debug.h"


class ByteString {
protected:
	MemPtr m_mem;
	const char * m_ptr;
public:
	int ReadPos;
	int Offset;
	int Size;

	ByteString();
	ByteString(const char * ptr, int offset, int size);
	ByteString(MemPtr memPtr, const char * ptr, int offset, int size);

	int          ReadByte();
	ByteString   ReadBlock(int size);
	const char * ReadString(int size);

	void Write(char c);
	void Write(const char *, int offset, int size);
};


class CodedOutputStream {
protected:
	int         m_fd;
	ByteBuffer *m_buffer;

	static int CalculateVarint32Size(int val);

	void WriteVarint32(int val);
	void WriteByte(char c);
	void WriteBytes(const char * buffer, int index, int size);
public:
	static const int P_WIRETYPE_VARINT    = 0;
	static const int P_WIRETYPE_64BIT     = 1;
	static const int P_WIRETYPE_DELIMITED = 2;
	static const int P_WIRETYPE_32BIT     = 5;

	static const int P_INT32_WIRETYPE  = P_WIRETYPE_VARINT;
	static const int P_BOOL_WIRETYPE   = P_WIRETYPE_VARINT;
	static const int P_STRING_WIRETYPE = P_WIRETYPE_DELIMITED;

	static int CalculateInt32Size(int fieldNumber, int val);
	static int CalculateBoolSize(int fieldNumber, bool val);
	static int CalculateStringSize(int fieldNumber, const char * val);
	static int CalculateBytesSize(int fieldNumber, int size);

	CodedOutputStream();
	CodedOutputStream(int fd);
	CodedOutputStream(ByteBuffer* buffer);

	void WriteInt32(int fieldNumber, int val);
	void WriteBool(int fieldNumber, bool val);
	void WriteString(int fieldNumber, const char * val);
	void WriteBytes(int fieldNumber, const void * val, int size);
};


class CodedInputStream {
protected:
	ByteBuffer *m_buffer;

	int  CurrentTag;
	int  CurrentFieldNumber;
	int  CurrentWireType;
	int  ReadByte();
	int  ReadVarint32();
	bool ReadBytes(char * buffer, int index, int size);
	void DiscardBytes(int size);

public:
	CodedInputStream();
	CodedInputStream(ByteBuffer* buffer);

	bool ReadTag();
	void SkipLastField();
	bool ReadInt32(int fieldNumber, int * val);
	bool ReadBool(int fieldNumber, bool * val);
	bool ReadString(int fieldNumber, char ** val);
	bool ReadBytes(int fieldNumber, void ** val);
	bool ReadBytes(int fieldNumber, void ** val, int size);
};


class IMessage
{
public:
	virtual int  CalculateSize() = 0;
	virtual void MergeFrom(CodedInputStream * input) = 0;
	virtual void WriteTo(CodedOutputStream * output) = 0;
};

