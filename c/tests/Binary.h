/*
 * File....: Binary.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 15:32
 * Purpose.: Binary data reading and writing
 * */


#pragma once


#include "Stream.h"
#include "Text.h"


// Writes binary data to a stream.
class CBinaryWriter
{
protected:
	CStream * m_stream;

public:
	// Default constructor that writes the a NULL stream.
	CBinaryWriter();
	
	// Constructor that writes data to the target stream.
	explicit CBinaryWriter(CStream * stream);
	
	// Closes the writer. Does not close the base stream.
	void Close();
	
	// Sets the base stream to the *stream*.
	void SetStream(CStream * stream);

	int  WriteInt8 (int8_t val);
	int  WriteInt16(int16_t val);
	int  WriteInt32(int32_t val);
	int  WriteInt64(int64_t val);

	int  WriteUInt8 (uint8_t val);
	int  WriteUInt16(uint16_t val);
	int  WriteUInt32(uint32_t val);
	int  WriteUInt64(uint64_t val);

	int  WriteBool  (bool val);
	int  WriteFloat (float val);
	int  WriteDouble(double val);

	int  WriteBytes(const void * data, int size);

	int  WriteString(const char * c, int maxSize);
	int  WriteString(const CString& c, int maxSize);
};


// Reads binary data from a stream.
class CBinaryReader
{
protected:
	CStream * m_stream;

public:
	// True/false indicating an EndOfStream condition
	bool Eof;
	
	// True/false indicating an Error condition
	bool Error;

	// Default constructor that reads from an empty stream
	CBinaryReader();
	
	// Constructor that reads from the source streeam.
	explicit CBinaryReader(CStream * stream);
	
	// Return the last error code, or RET_OK if no error found.
	int GetLastErr();
	
	// Return the string for the last error code.
	const char * GetLastErrMsg();
	
	// Close the reade. Does not close the stream.
	void Close();
	
	// Sets the base stream to the *stream*.
	void SetStream(CStream * stream);

	bool TryReadInt8 (int8_t    *val);
	bool TryReadInt16(int16_t   *val);
	bool TryReadInt32(int32_t   *val);
	bool TryReadInt64(int64_t   *val);

	bool TryReadUInt8 (uint8_t   *val);
	bool TryReadUInt16(uint16_t  *val);
	bool TryReadUInt32(uint32_t  *val);
	bool TryReadUInt64(uint64_t  *val);

	bool TryReadBool  (bool *val);
	bool TryReadFloat (float   *val);
	bool TryReadDouble(double *val);

	bool TryReadString(char * c, int maxSize);
	bool TryReadString(CString& c, int maxSize);
	bool TryReadBytes(void * buffer, int size);
	bool TryDiscardBytes(int size);

	int8_t  ReadInt8();
	int16_t ReadInt16();
	int32_t ReadInt32();
	int64_t ReadInt64();

	uint8_t  ReadUInt8();
	uint16_t ReadUInt16();
	uint32_t ReadUInt32();
	uint64_t ReadUInt64();

	bool   ReadBool();
	float  ReadFloat();
	double ReadDouble();

	CString ReadString(int maxSize);
	void ReadString(char * str, int maxSize);
	int  ReadBytes(void * buffer, int size);
	void DiscardBytes(int size);
};
