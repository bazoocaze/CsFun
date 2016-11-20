
#pragma once

class MyStream
{
public:
	int LastError;

	MyStream();
	
	void Close();

	bool CanRead();
	bool CanWrite();

	int Read();
	int Read(char * buffer, int offset, int size);

	int Write(char c);
	int Write(const char * buffer, int offset, int size);
};
