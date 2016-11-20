

#pragma once


class Print;


class Debug
{
public:
	static int HexDump(const void * ptr, int size);
	static int HexDump(Print& dest, const void * ptr, int size);
};

