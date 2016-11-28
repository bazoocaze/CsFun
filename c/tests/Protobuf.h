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


/* Represents a read-only buffer.
 * The data can come from a stream or a
 * memory block. */
class ByteString {
protected:

	/* Data memory pointer */
	MemPtr m_mem;
	const uint8_t * m_ptr;

	/* Base stream */
	Stream * m_stream;
	
public:
	int ReadPos;
	int Offset;
	int Size;

	/* Default constructor for an empty buffer. */
	ByteString();

	/* Constructor for a stream based buffer. */
	explicit ByteString(Stream *stream);

	/* Constructor for a memory block based buffer. */
	ByteString(const void * ptr, int offset, int size);

	/* Constructor for a memory block based buffer.
	 * The pointer permits auto-management of the memory block. */
	ByteString(MemPtr memPtr, const void * ptr, int offset, int size);

	/* Returns a pointer to the read position. */
	const uint8_t* GetPtr();

	/* Read a byte from the buffer.
	 * Returns the byte read, INT_EOF on EOF or INT_ERR on error. */
	int ReadByte();

	/* Read a block of data from the buffer.
	 * Remember to verify the returned block size. */
	ByteString ReadBlock(int size);

	/* Discards *size* bytes from the buffer.
	 * Returns the number of bytes sucessfully discarded. */
	int DiscardBytes(int size);
};


/* Represents a Protobuf encoder that writes data to
 * a destination stream. */
class CodedOutputStream {
protected:
	/* Base stream */
	Stream * m_output;

	/* Calculates the size in bytes of the varint32 value. */
	static int CalculateVarint32Size(int32_t val);

	/* Writes a varint32 value to the destination. */
	void WriteVarint32(int32_t val);
	
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

	static int CalculateInt8Size   (int fieldNumber, int8_t val);
	static int CalculateInt16Size  (int fieldNumber, int16_t val);
	static int CalculateInt32Size  (int fieldNumber, int32_t val);
	static int CalculateInt64Size  (int fieldNumber, int64_t val);

	static int CalculateUInt8Size  (int fieldNumber, uint8_t val);
	static int CalculateUInt16Size (int fieldNumber, uint16_t val);
	static int CalculateUInt32Size (int fieldNumber, uint32_t val);
	static int CalculateUInt64Size (int fieldNumber, uint64_t val);

	static int CalculateFloatSize  (int fieldNumber, float val);
	static int CalculateDoubleSize (int fieldNumber, double val);
	static int CalculateBoolSize   (int fieldNumber, bool val);

	static int CalculateStringSize (int fieldNumber, const char * val);
	static int CalculateStringSize (int fieldNumber, const String& val);
	static int CalculateBytesSize  (int fieldNumber, int size);
	static int CalculateMessageSize(int fieldNumber, const IMessage& val);

	/* Default constructor for a Null encoder that
	 * discards the encoded data. */
	CodedOutputStream();

	/* Constructor for a encoder that writes the
	 * encoded data to the output Stream. */
	explicit CodedOutputStream(Stream* output);

	void WriteInt8   (int fieldNumber, int8_t val);
	void WriteInt16  (int fieldNumber, int16_t val);
	void WriteInt32  (int fieldNumber, int32_t val);
	void WriteInt64  (int fieldNumber, int64_t val);

	void WriteUInt8  (int fieldNumber, uint8_t val);
	void WriteUInt16 (int fieldNumber, uint16_t val);
	void WriteUInt32 (int fieldNumber, uint32_t val);
	void WriteUInt64 (int fieldNumber, uint64_t val);

	void WriteFloat  (int fieldNumber, float val);
	void WriteDouble (int fieldNumber, double val);
	void WriteBool   (int fieldNumber, bool val);

	void WriteString (int fieldNumber, const char * val);
	void WriteString (int fieldNumber, const String& val);
	void WriteBytes  (int fieldNumber, const void * val, int size);
	void WriteMessage(int fieldNumber, IMessage& val);
};


/* Represents a Protobuf decoder that reads the
 * encoded data from a ByteString. */
class CodedInputStream {
protected:
	ByteString m_input;

	int  CurrentTag;
	int  CurrentFieldNumber;
	int  CurrentWireType;
	
	/* Read a varint32 from source. Return the value read or 0 on error. */ 
	int32_t ReadVarint32();

	/* Read a varint64 from source. Return the value read or 0 on error. */ 
	int64_t ReadVarint64();
	
	/* Read a varint32 from source. Return true/false on success. *Val* is set to zero on error. */ 
	bool ReadVarint32(int32_t & val);

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
	explicit CodedInputStream(const ByteString& input);
	explicit CodedInputStream(Stream*     input);

	/* Read the tag for the next field.
	 * Returns true/false on success. */
	bool   ReadTag();
	
	/* Skip/discard the data for unknow/unprocessed fields. */
	void   SkipLastField();
	
	bool   ReadInt8    (int fieldNumber, int8_t & val);
	bool   ReadInt16   (int fieldNumber, int16_t & val);
	bool   ReadInt32   (int fieldNumber, int32_t & val);
	bool   ReadInt64   (int fieldNumber, int64_t & val);

	bool   ReadUInt8   (int fieldNumber, uint8_t & val);
	bool   ReadUInt16  (int fieldNumber, uint16_t & val);
	bool   ReadUInt32  (int fieldNumber, uint32_t & val);
	bool   ReadUInt64  (int fieldNumber, uint64_t & val);

	bool   ReadBool    (int fieldNumber, bool & val);
	bool   ReadFloat   (int fieldNumber, float & val);
	bool   ReadDouble  (int fieldNumber, double & val);

	bool   ReadString  (int fieldNumber, char * val, int maxSize);
	bool   ReadString  (int fieldNumber, String & val);
	bool   ReadString  (int fieldNumber, String & val, int maxSize);
	bool   ReadMessage (int fieldNumber, IMessage & val);
	
	bool   ReadBytes   (int fieldNumber, void ** val);
	bool   ReadBytes   (int fieldNumber, void ** val, int size);
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
