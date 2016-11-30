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


struct MemPtr_s {
	void * data;
	int    count;
};


struct FdPtr_s {
	int    count;
};



//////////////////////////////////////////////////////////////////////
// MemPtr
//////////////////////////////////////////////////////////////////////



CMemPtr::CMemPtr() {
	this->ref = NULL;
}


CMemPtr::CMemPtr(void * data) {
	this->ref = NULL;
	Set(data);
}


CMemPtr::CMemPtr(const CMemPtr& other) {
	this->ref  = other.ref;
	AddRef();
}


CMemPtr& CMemPtr::operator=(const CMemPtr& other)
{
	if(other.ref == this->ref) 
		return *this;

	ReleaseRef();
	this->ref = other.ref;
	AddRef();

	return *this;
}


CMemPtr::~CMemPtr()
{
	Clear();
}


void CMemPtr::Clear()
{
	Set(NULL);
}


void CMemPtr::Set(void * data)
{
	if(data == Get())
		return;

	ReleaseRef();

	if(data == NULL) {
		this->ref = NULL;
	} else {
		ref = (MemPtr_t*)UTIL_MEM_MALLOC(sizeof(MemPtr_t));
		ref->count = 1;
		ref->data  = data;
	}
}


void * CMemPtr::Get() const
{
	if(ref)
		return ref->data;

	return NULL;
}


bool CMemPtr::Resize(int newSize)
{
	void * curPtr = Get();
	void * newPtr = UTIL_MEM_REALLOC(curPtr, newSize);

	if(newPtr == NULL)
		return false;

	ChangeTo(newPtr);
	return true;
}


void CMemPtr::Memset(uint8_t val, int size)
{
	if(ref == NULL || ref->data == NULL || size < 0)
		return;

	memset(ref->data, val, size);
}


void CMemPtr::ChangeTo(void * newPtr)
{
	if(ref == NULL)
		Set(newPtr);
	else
		ref->data = newPtr;
}


void CMemPtr::AddRef()
{
	if(ref != NULL)
		ref->count++;
}


void CMemPtr::ReleaseRef()
{
	if(ref == NULL)
		return;

	ref->count--;

	if(ref->count > 0)
		return;

	if(ref->data)
		UTIL_MEM_FREE(ref->data);

	ref->data = NULL;
	UTIL_MEM_FREE(ref);
	ref = NULL;
}



//////////////////////////////////////////////////////////////////////
// FdPtr
//////////////////////////////////////////////////////////////////////



CFdPtr::CFdPtr() {
   this->ref = NULL;
   this->Fd  = CLOSED_FD;
}

CFdPtr::CFdPtr(int fd) {
   this->ref = NULL;
   Set(fd);
}

CFdPtr::CFdPtr(const CFdPtr& other) {
   this->ref = other.ref;
   this->Fd  = other.Fd;
   AddRef();
}

CFdPtr& CFdPtr::operator=(const CFdPtr& other)
{
   if(other.ref == this->ref)
      return *this;

   ReleaseRef();
   this->ref = other.ref;
   this->Fd  = other.Fd;
   AddRef();

   return *this;
}

CFdPtr::~CFdPtr()
{
   Close();
}

void CFdPtr::Close()
{
   Set(CLOSED_FD);
}

void CFdPtr::Set(int fd)
{
	if(Fd == fd)
		return;

	ReleaseRef();
	this->Fd = fd;

	if(fd == CLOSED_FD) {
		this->ref = NULL;
	} else {
		ref = (FdPtr_t*)UTIL_MEM_MALLOC(sizeof(FdPtr_t));
		ref->count = 1;
	}
}

void CFdPtr::AddRef()
{
   if(ref != NULL)
	   ref->count++;
}

void CFdPtr::ReleaseRef()
{
   if(ref == NULL)
	   return;

   ref->count--;
   if(ref->count > 0)
	   return;

   CLogger::LogMsg(LEVEL_VERBOSE, "FdPtr:close fd %d", Fd);

   if(Fd != CLOSED_FD)
	   close(Fd);

   if(ref)
	   UTIL_MEM_FREE(ref);

   ref = NULL;
   Fd  = CLOSED_FD;
}

void CFdPtr::CDebug()
{
	StdErr.printf("[FdPtr:fd=%d,c=%d,ref=%p]", Fd, (ref != NULL) ? ref->count : 0, ref);
}
