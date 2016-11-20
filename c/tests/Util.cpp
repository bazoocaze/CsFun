/*
 * Arquivo....: Util.cpp
 * Autor......: José Ferreira - olvebra
 * Data.......: 06/08/2015 - 15:41
 * Objetivo...: Utilitários diversos.
 */


#include <stdarg.h>
#include <string.h>
#include <time.h>
#include <stdlib.h>

#include "Util.h"
#include "Config.h"
#include "Print.h"
#include "Logger.h"


#define PRINTF_SUPPORTS_FLOAT      0
#define PRINTF_SUPPORTS_PRINTABLE  1


#define PRINTF_STATE_NORMAL   0
#define PRINTF_STATE_TYPE     1
#define PRINTF_STATE_DECS     2


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


unsigned long int millis() {
struct timespec spec;
	clock_gettime(CLOCK_REALTIME, &spec);
	return (spec.tv_nsec/1000000) + (spec.tv_sec * 1000);
}


void delay(int ms) {
struct timespec t;
	t.tv_sec = ms / 1000;
	t.tv_nsec = (ms % 1000) * 1000000;
	nanosleep(&t, NULL);
}


int get_uptime() {
	return millis()/1000;
}


// ----------------------------------------


int util_itoa(Print *dest, unsigned long valor, int basen, int padsize, int zeropad, int lalign, int comSinal)
{
char stack[64 + padsize];
char * sp = stack;
int negative = (valor < 0 ? 1 : 0);
int size = 0;
	if(lalign) zeropad=0;

	if(comSinal && (valor & 0x80000000)) {
		negative = 1;
		valor = (-((int)valor));
	} else {
		if(negative) valor = -valor;
	}

	char fillchar = (zeropad ? '0' : ' ');

	do {
		int rem = valor % basen;
		if(rem < 10)
			rem += '0';
		else
			rem += 'a' - 10;
		*sp++ = rem;
		valor = valor / basen;
		size++;
	} while (valor);

	if(negative && !zeropad) *sp++ = '-';

	if(!lalign) {
		for(int n=0; n < (padsize - size - negative); n++)
			*sp++ = fillchar;
	}

	if(negative && zeropad) *sp++ = '-';

	do {
		dest->Write(*--sp);
	} while (sp != stack);

	if(lalign)
		for(int n=0; n < (padsize - size - negative); n++)
			dest->Write(fillchar);

	return 0;
}


int util_printf(Print *print, const char* fmt, ...) {
va_list ap;
int ret;
	va_start(ap, fmt);
	ret = util_printf(print, fmt, ap);
	va_end(ap);
	return ret;
}


int util_printfln(Print *print, const char* fmt, ...) {
va_list ap;
int ret;
	va_start(ap, fmt);
	ret = util_printf(print, fmt, ap);
	ret += print->println();
	va_end(ap);
	return ret;
}


int util_printfln(Print *dest, const char* fmt, va_list ap) {
	return util_printf(dest, fmt, ap) + dest->println();
}


int util_printf(Print *dest, const char* fmt, va_list ap) {
int size = 0;
int state = 0;
int argZero;
int argSize;
int argLAlign;
int valor;
char *str;
char charVal;
int argDecs;
#if PRINTF_SUPPORTS_PRINTABLE
Printable *pz;
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
				continue;
			}
			size += dest->Write(c);
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
			case 'c':
				// HACK: va_arg nao funcionando para (char)
				charVal = (char)va_arg(ap, int);
				size += dest->Write((char)charVal);
				break;
			case 'd':
				valor = va_arg(ap, int);
				size += util_itoa(dest, valor, 10, argSize, argZero, argLAlign, 1);
				break;
			case 'o':
				valor = va_arg(ap, int);
				size += util_itoa(dest, valor, 8, argSize, argZero, argLAlign, 0);
				break;
			case 'b':
				valor = va_arg(ap, int);
				size += util_itoa(dest, valor, 2, argSize, argZero, argLAlign, 0);
				break;
			case 'u':
				// TODO: printf: tratar unsigned
				valor = va_arg(ap, int);
				size += util_itoa(dest, valor, 10, argSize, argZero, argLAlign, 0);
				break;
			case 'p':
			case 'x':
				valor = va_arg(ap, int);
				size += util_itoa(dest, valor, 16, argSize, argZero, argLAlign, 0);
				break;
#if PRINTF_SUPPORTS_FLOAT
			case 'f':
				valorf = va_arg(ap, double);
				size += dest->print(valorf, argDecs);
				break;
#endif
			case 's':
				str = va_arg(ap, char *);
				valor = strlen(str);

				if(!argLAlign)
					for(int n=0; n < (argSize - valor); n++)
						size += dest->Write(' ');

				size += dest->Write((uint8_t*)str, valor);

				if(argLAlign)
					for(int n=0; n < (argSize - valor); n++)
						size += dest->Write(' ');

				break;
#if PRINTF_SUPPORTS_PRINTABLE
			case 'z':
				pz = va_arg(ap, Printable *);
				pz->printTo(*dest);
				break;
#endif
			default:
				size += dest->Write(c);
			}
			state = PRINTF_STATE_NORMAL;
		}
	}
	return size;
}


int __attribute__((weak)) OutOffMemoryHandler(const char *module, const char *subject, int size)
{
   Logger::LogMsg(
      LEVEL_FATAL,
      "Memory allocation failure in %s/%s for %d bytes",
      module, subject, size);
	exit(100);
}

