#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unistd.h>


#include "Stream.h"
#include "Binary.h"
#include "Ptr.h"
#include "Text.h"
#include "StringBuilder.h"
#include "Fd.h"


String ReadLine()
{
StringBuilder sb;
	sb.print("Alfa ");
	sb.print("Bravo ");
	return sb.GetString();
}


FdReader reader  = 0;
FdWriter writer = 1;


int main()
{
String a;
	while(reader.ReadLine(a, 32))
	{
		writer.printf("Linha:[%z]\n", &a);	
		// a.debug();
	}
	
	return 0;
}

