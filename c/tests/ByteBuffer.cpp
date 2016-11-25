/*
 * File.....: ByteBuffer.cpp
 * Author...: Jose Ferreira
 * Date.....: 2015-08-10 - 15:54
 * Purpose..: Byte buffer reader/writer
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
	Init();
}


int ByteBuffer::Write(char c)
{
	if(!Resize(m_WritePos + 1)) return RET_ERR;
	vstr ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = c;
	m_WritePos++;
	m_Length++;
	return 1;
}


int ByteBuffer::Write(const void * data, int index, int size)
{
	if(size <= 0) return 0;
	if(!Resize(m_WritePos + size)) return RET_ERR;
	vstr ptr = (vstr)m_ptr.Get();
	memcpy(ptr + m_WritePos, ((char*)data) + index, size);
	m_WritePos += size;
	m_Length += size;
	return size;
}


int ByteBuffer::ReadByte()
{
	if(m_Length == 0) return INT_EOF;
	vstr ptr = (vstr)m_ptr.Get();
	int ret = (unsigned char)ptr[m_ReadPos];
	m_ReadPos++;
	m_Length--;
	return ret;
}


int ByteBuffer::Read(void * dest, int index, int size)
{
	if(size < 0) size = 0;
	if(size > m_Length) size = m_Length;
	if(size > 0) {
		vstr ptr = (vstr)m_ptr.Get();
		memcpy(((char*)dest) + index, ptr + m_ReadPos, size);
	}
	m_ReadPos += size;
	m_Length -= size;
	return size;
}


BytePtr ByteBuffer::ReadBlock(int size)
{
	if(size < 0) size = 0;
	if(size > m_Length) size = m_Length;
	BytePtr ret = BytePtr(this->m_ptr, m_ReadPos, size);
	m_ReadPos += size;
	m_Length -= size;
	return ret;
}


int ByteBuffer::ConfirmRead(int size)
{
	if(size <= 0) return 0;
	if(size > m_Length) size = m_Length;
	m_ReadPos += size;
	m_Length -= size;
	return size;
}


bool ByteBuffer::Resize(int size)
{
int newSize;

	if(size < 0) return false;
	if(size > P_BYTEBUFFER_MAX_SIZE) return false;
	if(size < m_Capacity) return true;
	
	newSize = m_Capacity;

	/* validate minimum size */
	if(newSize < 16)
		newSize = 16;

	/* double the size */
	while(newSize < size)
		newSize = newSize * 2;

	if(!m_ptr.Resize(newSize))
	{
		OutOffMemoryHandler("ByteBuffer", "resize", newSize, true);
		return false;
	}

	m_Capacity = newSize;
	return true;
}


int ByteBuffer::LockRead(int size, void ** dest)
{	
	if(size <= 0) size = 0;
	if(size > m_Length) size = m_Length;
	vstr ptr = (vstr)m_ptr.Get();
	*dest = (ptr + m_ReadPos);
	return size;
}


BytePtr ByteBuffer::LockRead(int size)
{
	if(size < 0) size = 0;
	if(size > m_Length) size = m_Length;
	return BytePtr(this->m_ptr, m_ReadPos, size);
}


int ByteBuffer::DiscardBytes(int discardBytes)
{
	if(discardBytes <= 0)       return 0;
	if(discardBytes > m_Length) discardBytes = m_Length;
	m_ReadPos += discardBytes;
	m_Length -= discardBytes;
	return m_Length;
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
	if(size < 0) size = 0;
	Resize(m_WritePos + size);
	if(m_Capacity - m_WritePos < size)
		size = m_Capacity - m_WritePos;

	vstr ptr = (vstr)m_ptr.Get();
	*dest = (ptr + m_WritePos);
	return size;
}


BytePtr ByteBuffer::LockWrite(int size)
{
	if(size < 0) size = 0;
	Resize(m_WritePos + size);
	if(m_Capacity - m_WritePos < size)
		size = m_Capacity - m_WritePos;

	return BytePtr(m_ptr, m_WritePos, size);
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


String ByteBuffer::GetString()
{
vstr ptr;
	Resize(m_WritePos + 1);
	ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = 0;	
	return String(m_ptr, &ptr[m_ReadPos]);
}


int ByteBuffer::ReadPos() const
{
	return m_ReadPos;
}


int ByteBuffer::WritePos() const
{
	return m_WritePos;
}



//////////////////////////////////////////////////////////////////////
// BytePtr
//////////////////////////////////////////////////////////////////////



BytePtr::BytePtr()
{
	Ptr   = NULL;
	Size  = 0;
}


BytePtr::BytePtr(uint8_t * ptr, int size)
{
	Ptr   = ptr;
	Size  = size;
}


BytePtr::BytePtr(uint8_t * ptr, int offset, int size)
{
	Ptr   = &ptr[offset];
	Size  = size;
}


BytePtr::BytePtr(MemPtr memPtr, int offset, int size)
{
	m_ptr = memPtr;
	Ptr = (uint8_t*)m_ptr.Get();
	Ptr   = &Ptr[offset];
	Size = size;
}


BytePtr::BytePtr(MemPtr memPtr, uint8_t * ptr, int size)
{
	m_ptr = memPtr;
	Ptr   = ptr;
	Size  = size;
}

