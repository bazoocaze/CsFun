/*
 * File....: List.h
 * Author..: Jose Ferreira
 * Date....: 2016-11-22
 * Purpose.: Generic list using templates
 * */


#pragma once


#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <string.h>


#include "Util.h"


template <class T>
class List {
private:
	void ** m_list;
	int     m_count;

public:
	List()
	{
		m_list = NULL;
		m_count = 0;
	}

	~List() { Clear(); }

	T& operator[](const int index)
	{
	T * ptr = (T*)m_list[index];
		return *ptr;
	}
	
	int Count() { return m_count; }

	T* Add(T * item)
	{
		printf("[add %p]\n", item);
		m_list = (void **)realloc(m_list, sizeof(void*) * m_count + 1);
		m_list[m_count++] = item;
		return item;
	}

	T& Add(T & item) { Add(new T(item)); }

	void Clear()
	{
		if(m_list != NULL)
		{
			for(int n = 0; n < m_count; n++)
			{
				printf("[delete %p]\n", m_list[n]);
				delete (T*)m_list[n];
			}
			free(m_list);
			m_list = NULL;
			m_count = 0;
		}
	}
};
