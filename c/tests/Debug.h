/* File....: Debug.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 16:10
 * Purpose.: Debug tools
 * */


#pragma once


class TextWriter;


// Debug tools.
class Debug
{
public:
	// Hexdump the block of data to StdOut.
	static int HexDump(const void * ptr, int size);
	
	// Hexdump the block of data to the destination writer.
	static int HexDump(TextWriter& dest, const void * ptr, int size);
};
