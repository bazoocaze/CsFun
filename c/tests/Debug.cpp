/* 
 * File....: Debug.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-19 23:25
 * Purpose.: Debug functions
 */


#include "Debug.h"
#include "Fd.h"
#include "Text.h"
#include "IO.h"


int CDebug::HexDump(const void * ptr, int size)
{
	return HexDump(StdOut, ptr, size);
}


int CDebug::HexDump(CTextWriter& dest, const void * ptr, int size)
{
int ret = 0;
int desloc = 0;
int tamLinha = 16;
int i;
uint8_t * dados = (uint8_t *)ptr;
	while(size > 0)
	{
		ret += dest.printf("%04x|", desloc);
		for(i=0; i<tamLinha; i++)
		{
			if(i >= size)
			{
				if(desloc == 0)
					break;
				ret += dest.print("  ");
			}
			else
			{
				ret += dest.printf("%02x", dados[desloc + i]);
			}

			if(i < (tamLinha-1))
				ret += dest.print(' ');
		}

		ret += dest.print("|");

		for(i=0; i<tamLinha; i++)
		{
			if(i >= size)
				break;

			char c = dados[desloc+i];

			if(c < 32 || c >= 127)
				c = '.';

			ret += dest.printf("%c", c);
		}
		ret += dest.print("|\n");
		size -= tamLinha;
		desloc += tamLinha;
	}
	return ret;
}
