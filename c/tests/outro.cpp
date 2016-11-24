#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unistd.h>
#include <errno.h>


#include "Stream.h"
#include "Binary.h"
#include "Ptr.h"
#include "Text.h"
#include "StringBuilder.h"
#include "Fd.h"
#include "IO.h"
#include "FdSelect.h"



void outro_multi()
{
	FdSelect sel;
	sel.Add(0, SEL_READ);
	StdErr.println("Begin waiting");
	int ret = sel.WaitAll(5000);
	StdErr.printf("End waiting. ret=%d  fd.status=%d", ret, sel.GetStatus(0));
}


void outro_single()
{
	int ret = FdSelect::Wait(0, SEL_READ, 2100);
}



void outro_main()
{
	outro_multi();
	// outro_single();
	delay(6000);
}
