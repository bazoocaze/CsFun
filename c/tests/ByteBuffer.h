/*
 * Arquivo....: ByteBuffer.h
 * Autor......: José Ferreira - olvebra
 * Data.......: 10/08/2015 - 15:53
 * Objetivo...: Buffer de memoria flexivel para leitura/escrita
 */


#pragma once

#include "Ptr.h"


class BytePtr
{
private:
	MemPtr m_ptr;

public:
	char * Ptr;
	int    Size;

	BytePtr();
	BytePtr(char * ptr, int size);
	BytePtr(MemPtr memPtr, char * ptr, int size);
};


class ByteBuffer
{
private:
	/* Area de memoria interna do buffer. */
	MemPtr m_ptr;
	int    m_Length;           /* Tamanho dos data no buffer. */
	int    m_Capacity;           /* Tamanho da memoria pre-alocada. */
	int    m_ReadPos;           /* Posicao de leitura dos data. */
	int    m_WritePos;          /* Posicao de escrita dos data. */

	void Init();            /* Prepara as estruturas internas para uso. */
	int  Resize(int size);   /* Redimensiona o tamanho alocado do buffer. Retorna RET_OK/RET_ERRO se conseguiu. */

public:

	ByteBuffer();
	ByteBuffer(int initialCapacity);

	/* Retorna a quantidade de data no buffer. */
	int  Length() const;
	/* Retorna a quantidade de memoria alocada para o buffer. */
	int  Capacity() const;

	char * Ptr() const;

	const char * GetStr();

	/* Limpa o buffer descartando os data armazenados. */
	void Clear();

	/* Grava data no buffer. */
	int  Write(char c);
	int  Write(const void * source, int index, int size);
	
	/* Bloqueia pra escrita. Permite evitar a copia temporaria de Write(). */
	int     LockWrite(int size, void ** dest);
	BytePtr LockWrite(int size);
	void ConfirmWrite(int confirmBytes);

	/* Read data from buffer */
	int  ReadByte();
	int  Read(void * dest, int index, int size);
	int  LockRead(void ** dest);
	int  LockRead(int size, void ** dest);

	/* descarta os bytes lidos. */
	void DiscardBytes(int discardBytes);

	/* Comprime/otimiza a memoria alocada pelo buffer. */
	void Compress();
};


