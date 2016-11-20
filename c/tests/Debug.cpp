/* 
 * File....: Debug.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-19 23:25
 * Purpose.: Debug functions
 */


#include "Debug.h"
#include "Print.h"
#include "Fd.h"


int Debug::HexDump(const void * ptr, int size)
{
	FdPrint p = FdPrint(1);
	HexDump(p, ptr, size);
}


int Debug::HexDump(Print& dest, const void * ptr, int size)
{
int desloc = 0;
int tamLinha = 16;
int i;
const unsigned char *dados = (const unsigned char *)ptr;
	while(size > 0)
	{
		dest.printf("%04x|", desloc);
		for(i=0; i<tamLinha; i++)
		{
			if(i >= size)
			{
				if(desloc == 0) break;
				dest.print("  ");
			}
			else
				dest.printf("%02x", dados[desloc + i]);
			if(i < (tamLinha-1)) dest.print(' ');
		}
		dest.print("|");
		for(i=0; i<tamLinha; i++)
		{
			if(i >= size) break;
			char c = dados[desloc+i];
			if(c < 32 || c >= 127) c = '.';
			dest.printf("%c", c);
		}
		dest.print("|\n");
		size -= tamLinha;
		desloc += tamLinha;
	}
}

