/*
 * Arquivo....: Util.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 06/08/2015 - 15:42
 * Objetivo...: Utilitários diversos.
 */


#pragma once


#include <unistd.h>
#include <stdint.h>
#include <stdarg.h>
// #include "Print.h"


#define rotl16(a) ((uint16_t)(a << 1)|(uint16_t)(a >> 15))

#define CALL_MEMBER_FN(object, ptrToMember) ((object).*(ptrToMember))

#define RET_ENUM_STR(var, val)        if(var == val) return #val
#define RET_ENUM_DESC(var, val, desc) if(var == val) return desc

#ifndef MAX
#define MAX(a,b) ((a)>=(b)?(a):(b))
#endif /* !MAX */


// Constantes

#define RET_ERROR (-1)
#define RET_OK     (0)
#define CLOSED_FD (-1)

#define TRUE  (-1)
#define FALSE (0)



typedef const char * cstr;
typedef char *       vstr;


class Print;


// void WDT();                /* Chamadapara reset periódico do watchdog. */

unsigned long int millis();
void delay(int ms);

int get_uptime();

void do_events();

uint16_t simple_hash(cstr text);

uint16_t simple_data_hash(void * data, int size);

int   util_printf(Print *print, const char* fmt, ...);
int   util_printf(Print *print, const char* fmt, va_list ap);
int   util_printfln(Print *print, const char* fmt, ...);
int   util_printfln(Print *print, const char* fmt, va_list ap);

#pragma weak OutOffMemoryHandler
#pragma weak EndProgramHandler 

extern  int  OutOffMemoryHandler(const char *module, const char *subject, int size);
extern  void EndProgramHandler(const char *module, const char *subject);

