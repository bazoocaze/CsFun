/*
 * Arquivo....: ByteBuffer.cpp
 * Autor......: José Ferreira - olvebra
 * Data.......: 10/08/2015 - 15:54
 * Objetivo...: Buffer de memoria flexivel para leitura/escrita
 */


#include <stdio.h>
#include <stdlib.h>
#include <string.h>


#include "ByteBuffer.h"
#include "Logger.h"
#include "Config.h"



ByteBuffer::ByteBuffer()
{
	Init();
}


ByteBuffer::ByteBuffer(int initialCapacity)
{
	Init();
	Resize(initialCapacity);
}


void ByteBuffer::Clear()
{
	m_Length = 0;
	m_ReadPos = 0;
	m_WritePos = 0;
}


int ByteBuffer::Write(char c)
{
int ret;
vstr ptr;
	if(this->m_WritePos + 1 > this->m_Capacity)
	{
		ret = Resize(this->m_WritePos + 1);
		if(ret != RET_OK) return RET_ERROR;
	}
	ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = c;
	m_WritePos++;
	m_Length++;
	return 1;
}


int ByteBuffer::Write(const void * data, int index, int size)
{
int ret;
vstr ptr;
	if(this->m_WritePos + size > this->m_Capacity)
	{
		ret = Resize(this->m_WritePos + size);
		if(ret != RET_OK) return RET_ERROR;
	}
	ptr = (vstr)m_ptr.Get();
	memcpy(ptr + m_WritePos, ((char*)data) + index, size);
	m_WritePos += size;
	m_Length += size;
	return size;
}


int ByteBuffer::ReadByte()
{
int ret;
vstr ptr = (vstr)m_ptr.Get();
	if(m_Length == 0) return -1;
	ret = (unsigned char)ptr[m_ReadPos];
	m_ReadPos++;
	m_Length--;
	return ret;
}


int ByteBuffer::Read(void * dest, int index, int size)
{
vstr ptr = (vstr)m_ptr.Get();
	if(size > m_Length)
		size = m_Length;
	if(size > 0){
		memcpy(((char*)dest) + index, ptr + m_ReadPos, size);
	}
	m_ReadPos += size;
	m_Length -= size;
	return size;
}


int ByteBuffer::Resize(int size)
{
int newSize = m_Capacity;

	if(size < m_Capacity) return RET_OK;

	/* garante um size minimo valido */
	if(newSize < 16)
		newSize = 16;

	/* aumenta o size do buffer dobrando o size anterior */
	while(newSize < size)
		newSize = newSize * 2;

	Logger::LogMsg(LEVEL_VERBOSE,
		"ByteBuffer/Resize: from %d to %d",
		m_Capacity,
		newSize);

	if(!m_ptr.Realloc(newSize))
	{
		OutOffMemoryHandler("ByteBuffer", "resize", newSize);
		return RET_ERROR;
	}

	m_Capacity = newSize;
	return RET_OK;
}


int ByteBuffer::LockRead(void** dest) {
	return LockRead(m_Length, dest);
}


int ByteBuffer::LockRead(int size, void ** dest)
{
vstr ptr = (vstr)m_ptr.Get();
	if(size <= 0) size = 16;
	if(size > m_Length) size = m_Length;
	*dest = (ptr + m_ReadPos);
	return size;
}


void ByteBuffer::DiscardBytes(int discardBytes)
{
	if(discardBytes <= 0)       return;
	if(discardBytes > m_Length) discardBytes = m_Length;
	m_ReadPos += discardBytes;
	m_Length -= discardBytes;
}


void ByteBuffer::Compress()
{
	if(m_ReadPos > 1024 && m_Length == 0)
	{
		Logger::LogMsg(LEVEL_VERBOSE, "ByteBuffer/Compress");
		m_ptr = NULL;
		Init();
	}
}


void ByteBuffer::Init()
{
	m_ptr      = NULL;
	m_Capacity = 0;
	m_Length   = 0;
	m_ReadPos  = 0;
	m_WritePos = 0;
}


int ByteBuffer::Length() const
{
	return m_Length;
}


int  ByteBuffer::LockWrite(int size, void ** dest)
{
vstr ptr;

	if(m_Capacity - m_WritePos < size)
	{
		Resize(m_WritePos + size);

		if(m_Capacity - m_WritePos < size)
			size = m_Capacity - m_WritePos;
	}

	if(size < 0)
		size = m_Capacity - m_WritePos;

	ptr = (vstr)m_ptr.Get();
	*dest = (ptr + m_WritePos);

	return size;
}


BytePtr ByteBuffer::LockWrite(int size)
{
	if(m_Capacity - m_WritePos < size)
	{
		Resize(m_WritePos + size);

		if(m_Capacity - m_WritePos < size)
			size = m_Capacity - m_WritePos;
	}

	if(size < 0)
		size = m_Capacity - m_WritePos;

	BytePtr ret = BytePtr(m_ptr, m_WritePos, size);

	return ret;
}


void ByteBuffer::ConfirmWrite(int confirmBytes)
{
	if(confirmBytes <= 0)
		return;
	if(confirmBytes > (m_Capacity - m_WritePos))
	{
		Logger::LogMsg(
			LEVEL_ERROR,
			"ByteBuffer/UnlockWrite: confirmBytes invalido: %d",
			confirmBytes);
		confirmBytes = (m_Capacity - m_WritePos);
	}
	m_WritePos += confirmBytes;
	m_Length += confirmBytes;
}


int ByteBuffer::Capacity() const {
	return m_Capacity;
}


const char * ByteBuffer::GetStr()
{
vstr ptr;
	Resize(m_WritePos + 1);
	ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = 0;	
	return &ptr[m_ReadPos];
}


char * ByteBuffer::Ptr() const
{
	return (vstr)m_ptr.Get();
}


//////////////////////////////////////////////////////////////////////
// BytePtr
//////////////////////////////////////////////////////////////////////



BytePtr::BytePtr()
{
	Ptr   = NULL;
	Size  = 0;
}


BytePtr::BytePtr(char * ptr, int size)
{
	Ptr   = ptr;
	Size  = size;
}


BytePtr::BytePtr(MemPtr memPtr, char * ptr, int size)
{
	m_ptr = memPtr;
	Ptr   = ptr;
	Size  = size;
}

