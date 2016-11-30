#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "ByteBuffer.h"
#include "Debug.h"
#include "Protobuf.h"
#include "IO.h"
#include "MemoryStream.h"


class TesteSubMensagem : public IMessage
{
public:
	int    SubValor;
	CString SubStr1;
	char   SubStr2[128];

	int CalculateSize() const
	{
		int ret = 0;
		ret += CCodedOutputStream::CalculateInt32Size(1,  SubValor);
		ret += CCodedOutputStream::CalculateStringSize(2, SubStr1);
		ret += CCodedOutputStream::CalculateStringSize(3, SubStr2);
		return ret;
	}
	
	void MergeFrom(CCodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadInt32(1, SubValor)) continue;
			if(input->ReadString(2, SubStr1, 16)) continue;
			if(input->ReadString(3, SubStr2, 16)) continue;
			input->SkipLastField();
		}
	}
	
	void WriteTo(CCodedOutputStream * output)
	{
		output->WriteString(2, SubStr1);
		output->WriteString(3, SubStr2);
		output->WriteInt32(1, SubValor);
	}
	
	// TesteSubMensagem()  { printf("[SubMensagem:ctor]"); }
	// ~TesteSubMensagem() { printf("[SubMensagem:dtor]"); }
};


class TesteMensagem : public IMessage
{
public:
	int    Valor;
	CString Nome;
	bool   Flag;
	int    Valor1;
	TesteSubMensagem Sub;

	int CalculateSize() const
	{
		int ret = 0;
		ret += CCodedOutputStream::CalculateInt32Size(1,   Valor);
		ret += CCodedOutputStream::CalculateStringSize(2,  Nome);
		ret += CCodedOutputStream::CalculateBoolSize(3,    Flag);
		ret += CCodedOutputStream::CalculateInt32Size(4,   Valor1);
		ret += CCodedOutputStream::CalculateMessageSize(4, Sub);
		return ret;
	}

	void MergeFrom(CCodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadInt32(1,   Valor))  continue;
			if(input->ReadString(2,  Nome, 16))   continue;
			if(input->ReadBool(3,    Flag))   continue;
			if(input->ReadInt32(4,   Valor1)) continue;
			if(input->ReadMessage(5, Sub))    continue;
			input->SkipLastField();
		}
	}

	void WriteTo(CCodedOutputStream * output)
	{
		output->WriteMessage(5, Sub);
		output->WriteInt32(1,   Valor);
		output->WriteString(2,  Nome);
		output->WriteBool(3,    Flag);
		output->WriteInt32(4,   Valor1);
	}
};


TesteMensagem teste3;


int gpb_main()
{
	CMemoryStream ms;
	CCodedOutputStream cos = CCodedOutputStream(&ms);
	CCodedInputStream cis = CCodedInputStream(&ms);
	
	TesteMensagem teste1;
	teste1.Valor = 123456789;
	teste1.Nome  = "Porto Alegre Rio Grande do Sul Brasil";
	teste1.Flag  = true;
	teste1.Valor1 = 1024;
	
	teste1.Sub.SubValor = 234;
	teste1.Sub.SubStr1 = "The quick brown fox jumps over the lazy dog.";
	strcpy(teste1.Sub.SubStr2, "alfa bravo charlie");
	
	teste1.WriteTo(&cos);
	
	CDebug::HexDump(StdOut, ms.GetReadPtr(), ms.Length());
	
	// int bufferSize = ms.Length();
	// uint8_t buffer[bufferSize];
	// ms.Read(buffer, bufferSize);
	// Debug::HexDump(StdOut, buffer, bufferSize);

	TesteMensagem teste2 = TesteMensagem();
	teste2.MergeFrom(&cis);
	
	printf("\n");
	printf("[teste1: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS1=%s, SubS1=%s]\n", teste1.Valor, teste1.Nome.c_str, teste1.Flag, teste1.Valor1, teste1.Sub.SubValor, teste1.Sub.SubStr1.c_str, teste1.Sub.SubStr2);
	printf("[teste2: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS1=%s, SubS1=%s]\n", teste2.Valor, teste2.Nome.c_str, teste2.Flag, teste2.Valor1, teste2.Sub.SubValor, teste2.Sub.SubStr1.c_str, teste2.Sub.SubStr2);
	printf("[teste3: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS1=%s, SubS1=%s]\n", teste3.Valor, teste3.Nome.c_str, teste3.Flag, teste3.Valor1, teste3.Sub.SubValor, teste3.Sub.SubStr1.c_str, teste3.Sub.SubStr2);

	return 0;
}
