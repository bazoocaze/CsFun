/*
 * Arquivo....: Logger.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 10/08/2015 - 15:58
 * Objetivo...: Logging de mensagens.
 */


#pragma once

#include "Print.h"


enum LogLevelEnum {
	LEVEL_VERBOSE   = 0,
	LEVEL_DEBUG     = 1,
	LEVEL_INFO      = 2,
	LEVEL_WARN      = 3,
	LEVEL_ERROR     = 4,
	LEVEL_FATAL     = 5,
	LEVEL_DISABLED  = 6,
};


const char * LogLevelDesc(LogLevelEnum level);


class Logger
{
public:
	static Print*        Default;
	static LogLevelEnum  LogLevel;
	static void          LogMsg(LogLevelEnum nivel, const char * fmt, ...);
};
