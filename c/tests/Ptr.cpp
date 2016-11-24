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



MemPtr::MemPtr() {
	this->ref = NULL;
}


MemPtr::MemPtr(void * data) {
	this->ref = NULL;
	Set(data);
}


MemPtr::MemPtr(const MemPtr& other) {
	this->ref  = other.ref;
	AddRef();
}


MemPtr& MemPtr::operator=(const MemPtr& other)
{
	if(other.ref == this->ref) 
		return *this;
	ReleaseRef();
	this->ref = other.ref;
	AddRef();
	return *this;
}


MemPtr::~MemPtr()
{
	Clear();
}


void MemPtr::Clear()
{
	Set(NULL);
}


void MemPtr::Set(void * data)
{
	if(data == Get()) return;
	ReleaseRef();
	if(data == NULL) {
		this->ref = NULL;
	} else {
		ref = (MemPtr_t*)UTIL_MEM_MALLOC(sizeof(MemPtr_t));
		ref->count = 1;
		ref->data  = data;
	}
}


void * MemPtr::Get() const
{
	if(ref) return ref->data;
	return NULL;
}


bool MemPtr::Resize(int newSize)
{
	void * curPtr = Get();
	void * newPtr = UTIL_MEM_REALLOC(curPtr, newSize);
	if(newPtr == NULL) return false;
	ChangeTo(newPtr);
	return true;
}


void MemPtr::Memset(uint8_t val, int size)
{
	if(ref == NULL || ref->data == NULL || size < 0) return;
	memset(ref->data, val, size);
}


void MemPtr::ChangeTo(void * newPtr)
{
	if(ref == NULL)
		Set(newPtr);
	else
		ref->data = newPtr;
}


void MemPtr::AddRef()
{
	if(ref != NULL) ref->count++;
}


void MemPtr::ReleaseRef()
{
	if(ref == NULL) return;
	ref->count--;
	if(ref->count > 0) return;

	if(ref->data) UTIL_MEM_FREE(ref->data);
	ref->data = NULL;
	UTIL_MEM_FREE(ref);
	ref = NULL;
}



//////////////////////////////////////////////////////////////////////
// FdPtr
//////////////////////////////////////////////////////////////////////



FdPtr::FdPtr() {
   this->ref = NULL;
   this->Fd  = CLOSED_FD;
}

FdPtr::FdPtr(int fd) {
   this->ref = NULL;
   Set(fd);
}

FdPtr::FdPtr(const FdPtr& other) {
   this->ref = other.ref;
   this->Fd  = other.Fd;
   AddRef();
}

FdPtr& FdPtr::operator=(const FdPtr& other)
{
   if(other.ref == this->ref)
      return *this;
   ReleaseRef();
   this->ref = other.ref;
   this->Fd  = other.Fd;
   AddRef();
   return *this;
}

FdPtr::~FdPtr()
{
   Close();
}

void FdPtr::Close()
{
   Set(CLOSED_FD);
}

void FdPtr::Set(int fd)
{
	if(Fd == fd) return;
	ReleaseRef();
	this->Fd = fd;
	if(fd == CLOSED_FD) {
		this->ref = NULL;
	} else {
		ref = (FdPtr_t*)UTIL_MEM_MALLOC(sizeof(FdPtr_t));
		ref->count = 1;
	}
}

void FdPtr::AddRef()
{
   if(ref != NULL) ref->count++;
}

void FdPtr::ReleaseRef()
{
   if(ref == NULL) return;
   ref->count--;
   if(ref->count > 0) return;

   StdErr.printf("[FdPtr:close(%d)]", Fd);

   if(Fd != CLOSED_FD) close(Fd);
   if(ref)             UTIL_MEM_FREE(ref);
   ref = NULL;
   Fd  = CLOSED_FD;
}

void FdPtr::Debug()
{
	StdErr.printf("[FdPtr:fd=%d,c=%d,ref=%p]", Fd, (ref != NULL) ? ref->count : 0, ref);
}
