/*
 * File....: Stream.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 09:35
 * Purpose.: Byte stream abstraction
 */


#include <fcntl.h>
#include <errno.h>

#include "IO.h"
#include "Config.h"


#if HAVE_CONSOLE == 1

	// CConsole Console;
	CFdReader DefaultStdIn(0);
	CFdWriter DefaultStdOut(1);
	CFdWriter DefaultStdErr(2);

#else

	// NullText Console;
	CNullText DefaultStdIn;
	CNullText DefaultStdOut;
	CNullText DefaultStdErr;

#endif



DelegateReader StdIn(&DefaultStdIn);
DelegateWriter StdOut(&DefaultStdOut);
DelegateWriter StdErr(&DefaultStdErr);



//////////////////////////////////////////////////////////////////////
// FileStream
//////////////////////////////////////////////////////////////////////



bool CFileStream::Create(const char * fileName, CFileStream &ret)
{
int fd;
	fd = creat(fileName, S_IRUSR | S_IWUSR | S_IRGRP | S_IWGRP);
	if(fd == RET_ERR) {
		ret.m_lastErr = errno;
		return false;
	}
	ret.SetFd(fd);
	return true;
}

CFileStream::CFileStream() : CFdStream()
{
}

CFileStream::CFileStream(int fd) : CFdStream(fd)
{
	m_fdPtr.Set(fd);
}

void CFileStream::SetFd(int fd)
{
	CFdStream::SetFd(fd);
	m_fdPtr.Set(fd);
}

void CFileStream::Close()
{
	m_fdPtr.Set(CLOSED_FD);
	m_fd = CLOSED_FD;
}


//////////////////////////////////////////////////////////////////////
// DelegateReader
//////////////////////////////////////////////////////////////////////


DelegateReader::DelegateReader(CTextReader* reader)
	: m_reader(reader)
{
}

void DelegateReader::Set(CTextReader* reader)
{
	this->m_reader = (reader != NULL) ? reader : &CTextReader::Null;
}

bool DelegateReader::IsEof()
{
	return m_reader->IsEof();
}

bool DelegateReader::IsError()
{
	return m_reader->IsError();
}

int DelegateReader::GetLastErr()
{
	return m_reader->GetLastErr();
}

const char * DelegateReader::GetLastErrMsg()
{
	return m_reader->GetLastErrMsg();
}

int DelegateReader::Read()
{
	return m_reader->Read();
}

int DelegateReader::Read(void * buffer, int size)
{
	return m_reader->Read(buffer, size);
}


//////////////////////////////////////////////////////////////////////
// DelegateWriter
//////////////////////////////////////////////////////////////////////


DelegateWriter::DelegateWriter(CTextWriter* writer)
		: m_writer(writer)
{
}

void DelegateWriter::Set(CTextWriter* writer)
{
	m_writer = (writer != NULL) ? writer : &CTextWriter::Null;
}


bool DelegateWriter::IsError()
{
	return m_writer->IsError();
}

int DelegateWriter::Write(const uint8_t c)
{
	return m_writer->Write(c);
}

int DelegateWriter::Write(const void * buffer, int size)
{
	return m_writer->Write(buffer, size);
}

int DelegateWriter::GetLastErr()
{
	return m_writer->GetLastErr();
}

const char * DelegateWriter::GetLastErrMsg()
{
	return m_writer->GetLastErrMsg();
}
