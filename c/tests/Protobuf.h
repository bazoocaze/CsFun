/*
 * File....: Protobuf.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-23 08:36
 * Purpose.: Simple protocol buffer implementation
 * */


#pragma once


#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "ByteBuffer.h"
#include "Debug.h"


class IMessage;


class ByteString {
protected:
	MemPtr m_mem;
	const uint8_t * m_ptr;
	Stream * m_stream;
	
public:
	int ReadPos;
	int Offset;
	int Size;

	ByteString();
	ByteString(Stream *stream);
	ByteString(const void * ptr, int offset, int size);
	ByteString(MemPtr memPtr, const void * ptr, int offset, int size);
	
	const uint8_t* GetPtr();

	int          ReadByte();
	ByteString   ReadBlock(int size);
	String       ReadString(int size);
	int          DiscardBytes(int size);

	void Write(uint8_t c);
	void Write(const void *, int offset, int size);
};


class CodedOutputStream {
protected:
	Stream * m_output;
	// int         m_fd;
	// ByteBuffer *m_buffer;

	/* Calculates the size in bytes of the varint32 value. */
	static int CalculateVarint32Size(int val);

	/* Writes a varint32 value to the destination. */
	void WriteVarint32(int val);
	
	/* Writes a byte to the destination. */
	void WriteByte(uint8_t c);
	
	/* Write bytes to the destination. */
	void WriteBytes(const void * buffer, int offset, int size);
	
public:
	static const int P_WIRETYPE_VARINT    = 0;
	static const int P_WIRETYPE_64BIT     = 1;
	static const int P_WIRETYPE_DELIMITED = 2;
	static const int P_WIRETYPE_32BIT     = 5;

	static const int P_INT32_WIRETYPE   = P_WIRETYPE_VARINT;
	static const int P_BOOL_WIRETYPE    = P_WIRETYPE_VARINT;
	static const int P_STRING_WIRETYPE  = P_WIRETYPE_DELIMITED;
	static const int P_MESSAGE_WIRETYPE = P_WIRETYPE_DELIMITED;

	static int CalculateInt32Size(int fieldNumber, int val);
	static int CalculateBoolSize(int fieldNumber, bool val);
	static int CalculateStringSize(int fieldNumber, const char * val);
	static int CalculateStringSize(int fieldNumber, const String& val);
	static int CalculateBytesSize(int fieldNumber, int size);
	static int CalculateMessageSize(int fieldNumber, const IMessage& val);

	CodedOutputStream();
	CodedOutputStream(Stream* output);

	void WriteInt8(int fieldNumber,    int8_t val);
	void WriteInt16(int fieldNumber,   int16_t val);
	void WriteInt32(int fieldNumber,   int32_t val);
	void WriteBool(int fieldNumber,    bool val);
	void WriteString(int fieldNumber,  const char * val);
	void WriteString(int fieldNumber,  const String& val);
	void WriteBytes(int fieldNumber,   const void * val, int size);
	void WriteMessage(int fieldNumber, IMessage& val);
};


class CodedInputStream {
protected:
	ByteString m_input;
	// Stream     *m_stream;

	int  CurrentTag;
	int  CurrentFieldNumber;
	int  CurrentWireType;
	
	/* Read a varint32 from source. Return the value read or 0 on error. */ 
	int ReadVarint32();

	/* Read a varint64 from source. Return the value read or 0 on error. */ 
	int64_t ReadVarint64();
	
	/* Read a varint32 from source. Return true/false on success. *Val* is set to zero on error. */ 
	bool ReadVarint32(int & val);

	/* Read a varint64 from source. Return true/false on success. *Val* is set to zero on error. */ 
	bool ReadVarint64(int64_t & val);
	
	/* Read a byte from source. Return the byte read, INT_EOF on EOF or INT_ERR on error. */ 
	int SourceReadByte();
	
	/* Copy exact *size* bytes from source and fill the buffer. Return true/false on success. */ 
	bool SourceCopyBytes(void * buffer, int index, int size);
	
	/* Discard *size* bytes from source. Return true/false on success. */
	bool SourceDiscardBytes(int size);

public:
	CodedInputStream();	
	CodedInputStream(ByteString& input);
	CodedInputStream(Stream*     input);

	/* Read the tag for the next field. Returns true/false on success. */
	bool   ReadTag();
	
	/* Skip/discard the data for unknow/unprocessed fields. */
	void   SkipLastField();
	
	bool   ReadInt32(int fieldNumber,   int & val);
	bool   ReadBool(int fieldNumber,    bool & val);
	bool   ReadString(int fieldNumber,  String & val);
	bool   ReadMessage(int fieldNumber, IMessage & val);
	
	bool   ReadBytes(int fieldNumber,   void ** val);
	bool   ReadBytes(int fieldNumber,   void ** val, int size);
};


/* Represents a serializable message. */
class IMessage
{
public:
	/* Calculates the size of the message, in bytes. */
	virtual int  CalculateSize() const = 0;
	
	/* Merge the content of the message reading new data from *input*. */
	virtual void MergeFrom(CodedInputStream * input) = 0;
	
	/* Write the contents of the message to *output*. */
	virtual void WriteTo(CodedOutputStream * output) = 0;
};
