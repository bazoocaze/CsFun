/*
 * File.....: Util.cpp
 * Author...: Jose Ferreira
 * Date.....: 2015-08-06 - 15:41
 * Purpose..: Various tools and utils.
 */


#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include <time.h>
#include <stdlib.h>

#include "Util.h"
#include "Config.h"
#include "Logger.h"
#include "Stream.h"


#define PRINTF_SUPPORTS_FLOAT      0
#define PRINTF_SUPPORTS_PRINTABLE  1


#define PRINTF_STATE_NORMAL   0
#define PRINTF_STATE_TYPE     1
#define PRINTF_STATE_DECS     2


#define EXIT_MEM_ERROR 100


uint16_t simple_hash(const char * text) {
	uint16_t ret = 0x55AA;
	int pos = 0;
	while (text[pos] != 0) {
		uint16_t n = rotl16(ret);
		ret = n ^ text[pos];
		pos++;
	}
	return ret;
}


uint16_t simple_data_hash(void * data, int size) {
	uint8_t *ptr = static_cast<uint8_t *>(data);
	uint16_t ret = 0xAA55;
	int pos = 0;
	while (size--) {
		uint16_t n = rotl16(ret);
		ret = n ^ ptr[pos];
		pos++;
	}
	return ret;
}


uint64_t millis() {
struct timespec spec;
	clock_gettime(CLOCK_REALTIME, &spec);
	return (spec.tv_nsec/1000000) + (spec.tv_sec * 1000);
}


void delay(uint32_t ms) {
struct timespec t;
	t.tv_sec = ms / 1000;
	t.tv_nsec = (ms % 1000) * 1000000;
	nanosleep(&t, NULL);
}


uint32_t uptime() {
	return millis()/1000;
}


void doevents()
{
	IdleHandler();
}


void doevents(uint32_t milliseconds)
{
	uint64_t timeout = millis() + milliseconds;
	while(millis() < timeout)
	{
		IdleHandler();
	}
}


// ----------------------------------------


int util_itoa(CTextWriter *dest, uint64_t input, int basen, int padsize, bool zeropad, bool leftalign, bool negative)
{
char stack[(sizeof(input) * 8) + padsize + 1];
char * sp = stack;
int size = 0;
int signalSize = (negative ? 1 : 0);
int ret = 0;

	if(leftalign)
		zeropad = false;

	char fillchar = (zeropad ? '0' : ' ');

	if(basen < 2 || basen > 16)
		basen = 10;

	// push the number onto the stack
	do {
		int rem = input % basen;
		if(rem < 10)
			rem += '0';
		else
			rem += 'a' - 10;
		*sp++ = rem;
		input = input / basen;
		size++;
	} while (input);

	// push the negative signal if not zeropadded
	if(negative && !zeropad)
		*sp++ = '-';

	// left pad the number
	if(!leftalign) {
		for(int n=0; n < (padsize - size - signalSize); n++)
			*sp++ = fillchar;
	}

	// push the negative signal if zeropadded
	if(negative && zeropad)
		*sp++ = '-';

	// write stack contents to output
	do {
		ret += dest->Write(*--sp);
	} while (sp != stack);

	// right pad the number
	if(leftalign)
		for(int n=0; n < (padsize - size - signalSize); n++)
			ret += dest->Write(fillchar);

	return ret;
}


int util_printf(CTextWriter *print, const char* fmt, ...) {
va_list ap;
int ret;
	va_start(ap, fmt);
	ret = util_printf(print, fmt, ap);
	va_end(ap);
	return ret;
}


int util_printf(CTextWriter *writer, const char* fmt, va_list ap) {
int state = PRINTF_STATE_NORMAL;
int size = 0;
int argZero;
int argSize;
int argLAlign;
int argIntLen;
bool negative;
int64_t  sval;
uint64_t uval;
const char *str;
char charVal;
int argDecs;

#if PRINTF_SUPPORTS_PRINTABLE
CPrintable *pz;
#endif

#if PRINTF_SUPPORTS_FLOAT
double valorf;
#endif

	while(*fmt) {
		char c = *fmt++;

		if(state == PRINTF_STATE_NORMAL) {
			if(c == '%') {
				state = PRINTF_STATE_TYPE;
				argZero = 0;
				argSize = 0;
				argLAlign = 0;
				argDecs = 2;
				argIntLen = 32;
				continue;
			}
			size += writer->Write(c);
			continue;
		}

		if(state == PRINTF_STATE_TYPE) {
			if(c == '-' && argSize == 0) {
				argLAlign = 1;
				continue;
			}
			if(c == '0' && argSize == 0) {
				argZero = 1;
				continue;
			}
			if(c == '.') {
				argDecs = 0;
				state = PRINTF_STATE_DECS;
				continue;
			}
			if(c >= '0' && c <= '9') {
				argSize = (argSize * 10) + (c - '0');
				continue;
			}
		}

		if(state == PRINTF_STATE_DECS) {
			if(c >= '0' && c <= '9') {
				argDecs = (argDecs * 10) + (c - '0');
				continue;
			}
		}

		if(state == PRINTF_STATE_TYPE || state == PRINTF_STATE_DECS) {
			switch(c) {
			case 'l':
				argIntLen = 64;
				continue;
			case 'c':
				// HACK: va_arg does not work with (char)
				charVal = (char)va_arg(ap, int);
				size += writer->Write((char)charVal);
				break;
			case 'd':
				if(argIntLen == 64) sval = va_arg(ap, int64_t);
				else                sval = va_arg(ap, int32_t);
				negative = (sval < 0);
				if(negative) sval = -sval;
				size += util_itoa(writer, sval, 10, argSize, argZero, argLAlign, negative);
				break;
			case 'b':
				if(argIntLen == 64) uval = va_arg(ap, uint64_t);
				else                uval = va_arg(ap, uint32_t);
				size += util_itoa(writer, uval, 2, argSize, argZero, argLAlign, false);
				break;
			case 'o':
				if(argIntLen == 64) uval = va_arg(ap, uint64_t);
				else                uval = va_arg(ap, uint32_t);
				size += util_itoa(writer, uval, 8, argSize, argZero, argLAlign, false);
				break;
			case 'u':
				if(argIntLen == 64) uval = va_arg(ap, uint64_t);
				else                uval = va_arg(ap, uint32_t);
				size += util_itoa(writer, uval, 10, argSize, argZero, argLAlign, false);
				break;
			case 'x':
				if(argIntLen == 64) uval = va_arg(ap, uint64_t);
				else                uval = va_arg(ap, uint32_t);
				size += util_itoa(writer, uval, 16, argSize, argZero, argLAlign, false);
				break;
			case 'p':
				size += writer->print("0x");
				uval = (uint64_t)va_arg(ap, void*);
				size += util_itoa(writer, uval, 16, argSize, argZero, argLAlign, false);
				break;

			case 's':
				str = va_arg(ap, char *);
				sval = strlen(str);

				if(!argLAlign)
					for(int n=0; n < (argSize - sval); n++)
						size += writer->Write(' ');

				size += writer->Write((uint8_t*)str, sval);

				if(argLAlign)
					for(int n=0; n < (argSize - sval); n++)
						size += writer->Write(' ');

				break;

#if PRINTF_SUPPORTS_FLOAT
			case 'f':
				valorf = va_arg(ap, double);
				size += writer->print(valorf, argDecs);
				break;
#endif

#if PRINTF_SUPPORTS_PRINTABLE
			case 'P':
				pz = va_arg(ap, CPrintable *);
				size += pz->printTo(*writer);
				break;
#endif

			default:
				size += writer->Write(c);
			}
			state = PRINTF_STATE_NORMAL;
		}
	}
	return size;
}


void WEAK_ATTR OutOffMemoryHandler(const char *module, const char *subject, int size, bool isFatal)
{
	CLogger::LogMsg(
		LEVEL_FATAL,
		"Memory allocation failure in %s/%s for %d bytes",
		module, subject, size);
	if(isFatal)
		EndProgramHandler(EXIT_MEM_ERROR);
}


void WEAK_ATTR EndProgramHandler(int exitCode, const char *reason, const char *module, const char *subject)
{
	CLogger::LogMsg(
		LEVEL_FATAL,
		"Fatal failure: %s / %s / %s",
		reason, module, subject);
	EndProgramHandler(exitCode);
}


void WEAK_ATTR EndProgramHandler(int exitCode)
{
	exit(exitCode);
}


typedef struct {
	void * ptr;
	size_t size;
	bool   free;
	cstr   file;
	int    line;
} ptr_info_t;


#define MAX_PTR_INFO 1024


ptr_info_t ptr_info[MAX_PTR_INFO];


int util_mem_find_entry(void * ptr)
{
	for(int n = 0; n < MAX_PTR_INFO; n++)
		if(ptr_info[n].ptr == ptr)
			return n;
	return RET_ERR;
}


int util_mem_find_free()
{
	for(int n = 0; n < MAX_PTR_INFO; n++)
		if(ptr_info[n].ptr == NULL)
			return n;
	EndProgramHandler(EXIT_MEM_ERROR, "MEMORY/ERROR", "util_mem_find_free", "no free slots on ptr_info table");
	return RET_ERR;
}


char * util_mem_strdup(cstr sourceStr, cstr srcFile, int lineNumber)
{
int slot = util_mem_find_free();
	ptr_info[slot].ptr = strdup(sourceStr);
	ptr_info[slot].size = strlen(sourceStr) + 1;
	ptr_info[slot].file = srcFile;
	ptr_info[slot].line = lineNumber;
	return (vstr)ptr_info[slot].ptr;
}


void * util_mem_malloc(size_t size, cstr srcFile, int lineNumber)
{
int slot;
	slot = util_mem_find_free();
	ptr_info[slot].ptr  = malloc(size);
	ptr_info[slot].size = size;
	ptr_info[slot].file = srcFile;
	ptr_info[slot].line = lineNumber;
	return ptr_info[slot].ptr;
}


void * util_mem_realloc(void * oldPtr, size_t newSize, cstr srcFile, int lineNumber)
{
int slot;
void * newPtr;
	if(oldPtr == NULL)
		slot = util_mem_find_free();
	else
		slot = util_mem_find_entry(oldPtr);
	if(slot == RET_ERR) {
		CLogger::LogMsg(
			LEVEL_FATAL, "MEMORY/FREE: realloc of %ld bytes on unknow pointer %p (at %s:%d)\n",
			newSize, oldPtr, srcFile, lineNumber);
		EndProgramHandler(EXIT_MEM_ERROR);
	}
	newPtr = realloc(oldPtr, newSize);
	if(newSize == 0)
	{
		ptr_info[slot].ptr  = NULL;
		ptr_info[slot].size = newSize;
	}
	else if(newPtr != NULL)
	{
		ptr_info[slot].ptr  = newPtr;
		ptr_info[slot].size = newSize;
		ptr_info[slot].file = srcFile;
		ptr_info[slot].line = lineNumber;
		return newPtr;
	}
	return newPtr;
}


void util_mem_free(void * ptr, cstr srcFile, int lineNumber)
{
int slot;
	if(ptr == NULL)
		return;
	slot = util_mem_find_entry(ptr);
	if(slot == RET_ERR) {
		CLogger::LogMsg(
			LEVEL_FATAL, "MEMORY/FREE: free on unknow pointer %p (at %s:%d)\n",
			ptr, srcFile, lineNumber);
		EndProgramHandler(EXIT_MEM_ERROR);
	}
	free(ptr);
	ptr_info[slot].ptr = NULL;
}


void util_mem_debug()
{
	dprintf(2, "\n--- BEGIN MEM DEBUG ---\n");
	for(int n = 0; n < MAX_PTR_INFO; n++)
	{
		if(ptr_info[n].ptr == NULL)
			continue;
		
		const char * file;
		file = strrchr((vstr)ptr_info[n].file, '\\');
		if(file == NULL)
			file = strrchr((vstr)ptr_info[n].file, '/');
		
		if(file != NULL)
			file++;
		else
			file = ptr_info[n].file;
		
		dprintf(2, " %04d 0x%p %08ld %s:%d\n", n, ptr_info[n].ptr, ptr_info[n].size, file, ptr_info[n].line);
	}
	dprintf(2, "--- END MEM DEBUG ---\n");
}
