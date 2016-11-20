/*
  Stream.h - base class for character-based streams.
  Copyright (c) 2010 David A. Mellis.  All right reserved.

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

  parsing functions based on TextFinder library by Michael Margolis
*/


#pragma once


#include <inttypes.h>
#include "Print.h"


class Stream : public Print
{
  protected:
    unsigned long _timeout;      // number of milliseconds to wait for the next char before aborting timed read
    unsigned long _startMillis;  // used for timeout measurement
    int timedRead();    // private method to read stream with timeout
    int timedPeek();    // private method to peek stream with timeout
    int peekNextDigit(); // returns the next numeric digit in the stream or -1 if timeout

  public:
    virtual int Available() = 0;
    virtual int Read() = 0;
    virtual int Peek() = 0;
    virtual void Flush() = 0;

    Stream() {_timeout=1000;}

	// sets maximum milliseconds to wait for stream data, default is 1 second
	void SetTimeout(unsigned long timeout);  

	// reads data from the stream until the target string is found
	// returns true if target string is found, false if timed out (see setTimeout)
	bool Find(const char *target);

	// reads data from the stream until the target string of given length is found
	// returns true if target string is found, false if timed out
	bool Find(const char *target, size_t length);

	// as find but search ends if the terminator string is found
	bool FindUntil(const char *target, const char *terminator);

	// as above but search ends if the terminate string is found
	bool FindUntil(const char *target, size_t targetLen, const char *terminate, size_t termLen);

	// returns the first valid (long) integer value from the current position.
	// initial characters that are not digits (or the minus sign) are skipped
	// integer is terminated by the first character that is not a digit.
	long ParseInt();

	// float version of parseInt
	float ParseFloat();      

	// read chars from stream into buffer
	// terminates if length characters have been read or timeout (see setTimeout)
	// returns the number of characters placed in the buffer (0 means no valid data found)
	size_t ReadBytes(char *buffer, size_t length);

	// as readBytes with terminator character
	// terminates if length characters have been read, timeout, or if the terminator character  detected
	// returns the number of characters placed in the buffer (0 means no valid data found)
	size_t ReadBytesUntil(char terminator, char *buffer, size_t length);

protected:
	// as above but the given skipChar is ignored
	// as above but the given skipChar is ignored
	// this allows format characters (typically commas) in values to be ignored
	long ParseInt(char skipChar);  

	// as above but the given skipChar is ignored
	float ParseFloat(char skipChar);
};
