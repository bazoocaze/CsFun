/*
 * File....: Util.h
 * Author..: Jose Ferreira
 * Date....: 2015-08-06 - 15:42
 * Purpose.: Various tools and utils.
 */


#pragma once




#include <unistd.h>
#include <stdint.h>
#include <stdarg.h>


#define rotl16(a) ((uint16_t)(a << 1)|(uint16_t)(a >> 15))

#define CALL_MEMBER_FN(object, ptrToMember) ((object).*(ptrToMember))

#define RET_ENUM_STR(var, val)        if(var == val) return #val
#define RET_ENUM_DESC(var, val, desc) if(var == val) return desc

#define HAS_FLAG(flags, bit) ((flags & bit) == bit)

#define MILLIS_TO_TIMEVAL(msecs, tv) do { uint64_t m = (msecs); tv.tv_usec = (m % 1000) * 1000; tv.tv_sec = (m / 1000); }while(false)


#ifndef MAX
#define MAX(a,b) ((a)>=(b)?(a):(b))
#endif /* !MAX */

#ifndef MIN
#define MIN(a,b) ((a)<=(b)?(a):(b))
#endif /* !MAX */

#ifndef CLAMP
#define CLAMP(a, min, max) (((a) < (min)) ? (min) : (((a) > (max)) ? (max) : (a)))
#endif /* !CLAMP */


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


class CTextWriter;


uint64_t millis();
uint32_t uptime();

void delay(uint32_t milliseconds);
void doevents();
void doevents(uint32_t milliseconds);

uint16_t simple_hash(cstr text);

uint16_t simple_data_hash(void * data, int size);


#define UTIL_MEM_STRDUP(v)     util_mem_strdup(v, __FILE__, __LINE__)
#define UTIL_MEM_MALLOC(s)     util_mem_malloc(s, __FILE__, __LINE__)
#define UTIL_MEM_REALLOC(p, s) util_mem_realloc(p, s, __FILE__, __LINE__)
#define UTIL_MEM_FREE(p)       util_mem_free(p, __FILE__, __LINE__)

char * util_mem_strdup(const char * source, cstr srcName, int srcLine);
void * util_mem_malloc(size_t size, cstr srcName, int srcLine);
void * util_mem_realloc(void * ptr, size_t size, cstr srcName, int srcLine);
void   util_mem_free(void * ptr, cstr srcName, int srcLine);
void   util_mem_debug();

int util_printf(CTextWriter *writer, const char* fmt, ...);
int util_printf(CTextWriter *writer, const char* fmt, va_list ap);

#define WEAK_ATTR __attribute__((weak))

extern void WEAK_ATTR OutOffMemoryHandler(const char *module, const char *subject, int size, bool isFatal);
extern void WEAK_ATTR EndProgramHandler(int exitCode);
extern void WEAK_ATTR EndProgramHandler(int exitCode, const char *reason, const char *module, const char *subject);
extern void WEAK_ATTR IdleHandler();
