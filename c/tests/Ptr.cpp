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
	add();
}


MemPtr& MemPtr::operator=(const MemPtr& other)
{
	if(other.ref == this->ref) 
		return *this;
	release();
	this->ref = other.ref;
	add();
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
	release();
	if(data == NULL) {
		this->ref = NULL;
	} else {
		ref = (MemPtr_t*)malloc(sizeof(MemPtr_t));
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
	void * newPtr = realloc(curPtr, newSize);
	if(newPtr == NULL) return false;
	ChangeTo(newPtr);
	return true;
}


void MemPtr::ChangeTo(void * newPtr)
{
	if(ref == NULL)
		Set(newPtr);
	else
		ref->data = newPtr;
}


void MemPtr::add()
{
	if(ref != NULL) ref->count++;
}


void MemPtr::release()
{
	if(ref == NULL) return;
	ref->count--;
	if(ref->count > 0) return;

	if(ref->data) free(ref->data);
	ref->data = NULL;
	free(ref);
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
   add();
}

FdPtr& FdPtr::operator=(const FdPtr& other)
{
   if(other.ref == this->ref)
      return *this;
   release();
   this->ref = other.ref;
   this->Fd  = other.Fd;
   add();
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
	release();
	this->Fd = fd;
	if(fd == CLOSED_FD) {
		this->ref = NULL;
	} else {
		ref = (FdPtr_t*)malloc(sizeof(FdPtr_t));
		ref->count = 1;
	}
}

void FdPtr::add()
{
   if(ref != NULL) ref->count++;
}

void FdPtr::release()
{
   if(ref == NULL) return;
   ref->count--;
   if(ref->count > 0) return;

   printf("[fd.close(%d)]", Fd);

   if(Fd != CLOSED_FD) close(Fd);
   if(ref)             free(ref);
   ref = NULL;
   Fd  = CLOSED_FD;
}

