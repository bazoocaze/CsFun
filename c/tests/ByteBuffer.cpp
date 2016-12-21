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



#define RESIZE_SMALL_MIN_SIZE  16
#define RESIZE_SMALL_SIZE_MULT 4
#define RESIZE_BIG_THRESHOULD  (4 * 1024 * 1024)
#define RESIZE_BIG_STEP        (4 * 1024 * 1024)



CByteBuffer::CByteBuffer()
{
	Init();
}


void CByteBuffer::Clear()
{
	Init();
}


int CByteBuffer::Write(char c)
{
	if(!Resize(m_WritePos + 1))
		return RET_ERR;

	vstr ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = c;
	m_WritePos++;
	m_Length++;
	return 1;
}


int CByteBuffer::Write(const void * data, int index, int size)
{
	if(size <= 0)
		return 0;

	if(!Resize(m_WritePos + size))
		return RET_ERR;

	vstr ptr = (vstr)m_ptr.Get();
	memcpy(ptr + m_WritePos, ((char*)data) + index, size);
	m_WritePos += size;
	m_Length += size;
	return size;
}


int CByteBuffer::ReadByte()
{
	if(m_Length == 0)
		return INT_EOF;

	vstr ptr = (vstr)m_ptr.Get();
	int ret = (unsigned char)ptr[m_ReadPos];
	m_ReadPos++;
	m_Length--;
	return ret;
}


int CByteBuffer::Read(void * dest, int index, int size)
{
	if(size < 0)
		size = 0;

	if(size > m_Length)
		size = m_Length;

	if(size > 0) {
		vstr ptr = (vstr)m_ptr.Get();
		memcpy(((char*)dest) + index, ptr + m_ReadPos, size);
	}

	m_ReadPos += size;
	m_Length -= size;
	return size;
}


CBytePtr CByteBuffer::ReadBlock(int size)
{
	if(size < 0)
		size = 0;

	if(size > m_Length)
		size = m_Length;

	CBytePtr ret = CBytePtr(this->m_ptr, m_ReadPos, size);
	m_ReadPos += size;
	m_Length -= size;
	return ret;
}


int CByteBuffer::ConfirmRead(int size)
{
	if(size <= 0)
		return 0;

	if(size > m_Length)
		size = m_Length;

	m_ReadPos += size;
	m_Length -= size;
	return size;
}


bool CByteBuffer::Resize(int targetSize)
{
	if(targetSize < 0) {
		OutOffMemoryHandler("ByteBuffer", "resize: size overflow", targetSize, true);
		return false;
	}

	if(targetSize <= m_Capacity)
		return true;

	int newCapacity = RESIZE_SMALL_MIN_SIZE;

	while(newCapacity <= targetSize)
	{
		if(newCapacity < RESIZE_BIG_THRESHOULD) {
			newCapacity = newCapacity * RESIZE_SMALL_SIZE_MULT;
		} else {
			int newtarget = newCapacity + RESIZE_BIG_STEP;
			if(newtarget < newCapacity) {
				OutOffMemoryHandler("ByteBuffer", "resize: size overflow", targetSize, true);
				return false;
			}
			newCapacity = newtarget;
		}
	}

	if(!m_ptr.Resize(newCapacity))
	{
		OutOffMemoryHandler("ByteBuffer", "resize", newCapacity, true);
		return false;
	}

	m_Capacity = newCapacity;
	return true;
}


int CByteBuffer::LockRead(int size, void ** dest)
{	
	if(size <= 0)
		size = 0;

	if(size > m_Length)
		size = m_Length;

	vstr ptr = (vstr)m_ptr.Get();
	*dest = (ptr + m_ReadPos);
	return size;
}


CBytePtr CByteBuffer::LockRead(int size)
{
	if(size < 0)
		size = 0;

	if(size > m_Length)
		size = m_Length;

	return CBytePtr(this->m_ptr, m_ReadPos, size);
}


int CByteBuffer::DiscardBytes(int discardBytes)
{
	if(discardBytes <= 0)
		return 0;

	if(discardBytes > m_Length)
		discardBytes = m_Length;

	m_ReadPos += discardBytes;
	m_Length -= discardBytes;
	return m_Length;
}


void CByteBuffer::Compress()
{
	if(m_ReadPos > 1024 && m_Length == 0)
	{
		CLogger::LogMsg(LEVEL_VERBOSE, "ByteBuffer/Compress");
		m_ptr.Clear();
		Init();
	}
}


void CByteBuffer::Init()
{
	m_ptr.Clear();
	m_Capacity = 0;
	m_Length   = 0;
	m_ReadPos  = 0;
	m_WritePos = 0;
}


int CByteBuffer::Length() const
{
	return m_Length;
}


int  CByteBuffer::LockWrite(int size, void ** dest)
{
	if(size < 0)
		size = 0;

	Resize(m_WritePos + size);

	if(m_Capacity - m_WritePos < size)
		size = m_Capacity - m_WritePos;

	vstr ptr = (vstr)m_ptr.Get();
	*dest = (ptr + m_WritePos);
	return size;
}


CBytePtr CByteBuffer::LockWrite(int size)
{
	if(size < 0)
		size = 0;

	Resize(m_WritePos + size);

	if(m_Capacity - m_WritePos < size)
		size = m_Capacity - m_WritePos;

	return CBytePtr(m_ptr, m_WritePos, size);
}


void CByteBuffer::ConfirmWrite(int confirmBytes)
{
	if(confirmBytes <= 0)
		return;

	if(confirmBytes > (m_Capacity - m_WritePos))
	{
		CLogger::LogMsg(
			LEVEL_ERROR,
			"ByteBuffer/UnlockWrite: confirmBytes invalido: %d",
			confirmBytes);
		confirmBytes = (m_Capacity - m_WritePos);
	}

	m_WritePos += confirmBytes;
	m_Length += confirmBytes;
}


int CByteBuffer::Capacity() const {
	return m_Capacity;
}


CString CByteBuffer::GetString()
{
vstr ptr;
	Resize(m_WritePos + 1);
	ptr = (vstr)m_ptr.Get();
	ptr[m_WritePos] = 0;	
	return CString(m_ptr, &ptr[m_ReadPos]);
}


int CByteBuffer::ReadPos() const
{
	return m_ReadPos;
}


int CByteBuffer::WritePos() const
{
	return m_WritePos;
}



//////////////////////////////////////////////////////////////////////
// BytePtr
//////////////////////////////////////////////////////////////////////



CBytePtr::CBytePtr()
{
	Ptr   = NULL;
	Size  = 0;
}


CBytePtr::CBytePtr(uint8_t * ptr, int size)
{
	Ptr   = ptr;
	Size  = size;
}


CBytePtr::CBytePtr(uint8_t * ptr, int offset, int size)
{
	Ptr   = &ptr[offset];
	Size  = size;
}


CBytePtr::CBytePtr(CMemPtr memPtr, int offset, int size)
{
	m_ptr = memPtr;
	Ptr   = (uint8_t*)m_ptr.Get();
	Ptr   = &Ptr[offset];
	Size  = size;
}


CBytePtr::CBytePtr(CMemPtr memPtr, uint8_t * ptr, int size)
{
	m_ptr = memPtr;
	Ptr   = ptr;
	Size  = size;
}
