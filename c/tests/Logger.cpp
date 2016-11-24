/*
 * Arquivo....: Logger.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 10/08/2015 - 15:58
 * Objetivo...: Logging de mensagens.
 */


#include <stdio.h>
#include <stdarg.h>


#include "Logger.h"
#include "Config.h"
#include "Util.h"
#include "Text.h"
#include "IO.h"


const char* LogLevelStr(LogLevelEnum level) {
	RET_ENUM_STR(level, LEVEL_VERBOSE);
	RET_ENUM_STR(level, LEVEL_DEBUG);
	RET_ENUM_STR(level, LEVEL_INFO);
	RET_ENUM_STR(level, LEVEL_WARN);
	RET_ENUM_STR(level, LEVEL_ERROR);
	RET_ENUM_STR(level, LEVEL_FATAL);
	RET_ENUM_STR(level, LEVEL_DISABLED);
	return "<unknow>";
}


const char* LogLevelDesc(LogLevelEnum level) {
	RET_ENUM_DESC(level, LEVEL_VERBOSE,  "verbose");
	RET_ENUM_DESC(level, LEVEL_DEBUG,    "debug");
	RET_ENUM_DESC(level, LEVEL_INFO,     "info");
	RET_ENUM_DESC(level, LEVEL_WARN,     "warn");
	RET_ENUM_DESC(level, LEVEL_ERROR,    "error");
	RET_ENUM_DESC(level, LEVEL_FATAL,    "fatal");
	RET_ENUM_DESC(level, LEVEL_DISABLED, "disabled");
	return "<unknow>";
}


TextWriter*  Logger::Default  = &StdErr;
LogLevelEnum Logger::LogLevel = P_DEFAULT_LOG_LEVEL;


void Logger::LogMsg(LogLevelEnum level, const char * fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);

	if(Default == NULL) return;

	if(level < LEVEL_VERBOSE) level = LEVEL_INFO;
	if(level > LEVEL_FATAL) level = LEVEL_INFO;

	if(level >= LogLevel) {
		Default->printf("[%s] ", LogLevelDesc(level));
		Default->printf(fmt, ap);
		Default->println();
	}

	va_end(ap);
}

