/*
 * Fil.....: Util.h
 * Author..: Jose Ferreira
 * Date....: 2015-08-06 - 15:42
 * Purpose.: Tools.
 */


#pragma once


#include <unistd.h>
#include <stdint.h>
#include <stdarg.h>


#define rotl16(a) ((uint16_t)(a << 1)|(uint16_t)(a >> 15))

#define CALL_MEMBER_FN(object, ptrToMember) ((object).*(ptrToMember))

#define RET_ENUM_STR(var, val)        if(var == val) return #val
#define RET_ENUM_DESC(var, val, desc) if(var == val) return desc

#ifndef MAX
#define MAX(a,b) ((a)>=(b)?(a):(b))
#endif /* !MAX */


// Constants

// Return OK or NO ERROR
#define RET_OK     (0)

// Error return
#define RET_ERR   (-1)

// Error return
#define RET_ERROR (-1)

// Represents a closed file descriptor
#define CLOSED_FD (-1)

#define TRUE      (-1)
#define FALSE     (0)

#define INT_EOF   (-1)
#define INT_ERR   (-2)




typedef const char * cstr;
typedef char *       vstr;


class TextWriter;


unsigned long int millis();
void delay(int ms);

int get_uptime();

void do_events();

uint16_t simple_hash(cstr text);

uint16_t simple_data_hash(void * data, int size);

int   util_printf(TextWriter   *writer, const char* fmt, ...);
int   util_printf(TextWriter   *writer, const char* fmt, va_list ap);
int   util_printfln(TextWriter *writer, const char* fmt, ...);
int   util_printfln(TextWriter *writer, const char* fmt, va_list ap);

#pragma weak OutOffMemoryHandler
#pragma weak EndProgramHandler 

extern  int  OutOffMemoryHandler(const char *module, const char *subject, int size);
extern  void EndProgramHandler(const char *module, const char *subject);
