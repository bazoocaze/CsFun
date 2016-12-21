/*
 * File......: Logger.h
 * Author....: Jose Ferreira
 * Date......: 2015/08/10 - 15:58
 * Purpose...: Message logging.
 */


#pragma once


#include "Text.h"


// Severity level of a log message.
enum LogLevelEnum {
	LEVEL_VERBOSE   = 0,
	LEVEL_DEBUG     = 1,
	LEVEL_INFO      = 2,
	LEVEL_WARN      = 3,
	LEVEL_ERROR     = 4,
	LEVEL_FATAL     = 5,
	LEVEL_DISABLED  = 6
};


// Gets a textual description for the enum value of the severity level.
const char * LogLevelStr(LogLevelEnum level);

// Gets a textual description for severity level.
const char * LogLevelDesc(LogLevelEnum level);


// Message logging facilitie
class CLogger
{
protected:
	// Default logger writer
	static CTextWriter* Default;

public:
	static CTextWriter* Get();
	static void Set(CTextWriter* logger);
	
	/* Minimum message severity for logging.
	 * Message below this severity level are discarded. */
	static LogLevelEnum  LogLevel;
	
	/* Logs a message with the severity indicated to the default logger.
	 * Message below the *Logger::LogLevel* severity are discarded without
	 * logging. */
	static void LogMsg(LogLevelEnum nivel, const char * fmt, ...);
};
