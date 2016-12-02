/*
 * File....: Ptr.cpp
 * Author..: Jose Ferreira
 * Date....: 2016-11-22 22:18
 * Purpose.: Memory and fd automatic management.
 * */


#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "Ptr.h"
#include "Util.h"
#include "IO.h"
#include "Logger.h"


//////////////////////////////////////////////////////////////////////
// RefPtr
//////////////////////////////////////////////////////////////////////


void CRefPtr::AddRef()
{
	if(m_ref != NULL)
		m_ref->count++;
}

void CRefPtr::ReleaseRef()
{
	if(m_ref == NULL || m_ref->count <= 0)
		return;

	m_ref->count--;

	if(m_ref->count == 0)
	{
		if(m_ref->data != NULL)
		{
			ReleaseData(m_ref->data);
			m_ref->data = NULL;
		}
		UTIL_MEM_FREE(m_ref);
	}
	m_ref = NULL;
}


void CRefPtr::ChangeDataPtrTo(void * data)
{
	if(m_ref == NULL)
	{
		m_ref = (ref_ptr_t*)UTIL_MEM_MALLOC(sizeof(ref_ptr_t));
		m_ref->count = 1;
	}
	m_ref->data = data;
}


void CRefPtr::SetDataPtr(void * data)
{
	if(m_ref != NULL)
	{
		if(m_ref->data == data)
			return;
		ReleaseRef();
	}

	if(data == NULL)
		return;

	ChangeDataPtrTo(data);
}

void * CRefPtr::GetDataPtr() const
{
	return (m_ref == NULL ? NULL : m_ref->data);
}

CRefPtr::CRefPtr()
{
	this->m_ref = NULL;
}

// Copy constructor
CRefPtr::CRefPtr(const CRefPtr& other)
{
	this->m_ref = other.m_ref;
	AddRef();
}

// Copy assignment.
CRefPtr& CRefPtr::operator=(const CRefPtr& other)
{
	if(other.m_ref == this->m_ref)
		return *this;

	ReleaseRef();
	this->m_ref = other.m_ref;
	AddRef();

	return *this;
}

CRefPtr::~CRefPtr()
{
}

void CRefPtr::Debug()
{
	int count  = (this->m_ref != NULL ? this->m_ref->count : 0);
	void *data = (this->m_ref != NULL ? this->m_ref->data : NULL);
	StdErr.printf("[CRefPtr:count=%d, data=%p]\n", count, data);
}


//////////////////////////////////////////////////////////////////////
// MemPtr
//////////////////////////////////////////////////////////////////////


CMemPtr::CMemPtr()
{
}


CMemPtr::CMemPtr(void * data)
{
	SetDataPtr(data);
}


CMemPtr::~CMemPtr()
{
	CRefPtr::ReleaseRef();
}


void CMemPtr::Clear()
{
	CRefPtr::ReleaseRef();
}


void CMemPtr::Set(void * data)
{
	SetDataPtr(data);
}


void * CMemPtr::Get() const
{
	return GetDataPtr();
}


bool CMemPtr::Resize(int newSize)
{
	void * curPtr = Get();
	void * newPtr = UTIL_MEM_REALLOC(curPtr, newSize);

	if(newPtr == NULL)
		return false;

	if(curPtr != newPtr)
		ChangeDataPtrTo(newPtr);

	return true;
}


void CMemPtr::Memset(uint8_t val, int size)
{
	void * data = Get();
	if(data == NULL || size < 0)
		return;

	memset(data, val, size);
}


void CMemPtr::ReleaseData(void * data)
{
	if(data != NULL)
	{
		// CLogger::LogMsg(LEVEL_VERBOSE, "CMemPtr: free(%p)", data);
		UTIL_MEM_FREE(data);
	}
}
