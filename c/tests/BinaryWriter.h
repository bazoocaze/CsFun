


#pragma once


class BinaryWriter
{
int m_fd;
public:
	BinaryWriter();
	BinaryWriter(int fd);

	virtual void Write(char c);
	virtual void Write(const void * buffer, int size);

	void WriteInt32(int val);
	void WriteString(const char * val);
};


class BinaryReader
{
public:
	BinaryReader();
};

