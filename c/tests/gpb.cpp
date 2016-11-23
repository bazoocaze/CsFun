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
	String SubStr;

	int CalculateSize() const
	{
		int ret = 0;
		ret += CodedOutputStream::CalculateInt32Size(1, SubValor);
		ret += CodedOutputStream::CalculateStringSize(2, SubStr);
		return ret;
	}
	
	void MergeFrom(CodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadInt32(1, SubValor)) continue;
			if(input->ReadString(2, SubStr)) continue;
			input->SkipLastField();
		}
	}
	
	void WriteTo(CodedOutputStream * output)
	{
		output->WriteInt32(1, SubValor);
		output->WriteString(2, SubStr);
	}
	
	// TesteSubMensagem()  { printf("[SubMensagem:ctor]"); }
	// ~TesteSubMensagem() { printf("[SubMensagem:dtor]"); }
};


class TesteMensagem : public IMessage
{
public:
	int    Valor;
	String Nome;
	bool   Flag;
	int    Valor1;
	TesteSubMensagem Sub;

	int CalculateSize() const
	{
		int ret = 0;
		ret += CodedOutputStream::CalculateInt32Size(1,   Valor);
		ret += CodedOutputStream::CalculateStringSize(2,  Nome);
		ret += CodedOutputStream::CalculateBoolSize(3,    Flag);
		ret += CodedOutputStream::CalculateInt32Size(4,   Valor1);
		ret += CodedOutputStream::CalculateMessageSize(4, Sub);
		return ret;
	}

	void MergeFrom(CodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadInt32(1,   Valor))  continue;
			if(input->ReadString(2,  Nome))   continue;
			if(input->ReadBool(3,    Flag))   continue;
			if(input->ReadInt32(4,   Valor1)) continue;
			if(input->ReadMessage(5, Sub))    continue;
			input->SkipLastField();
		}
	}

	void WriteTo(CodedOutputStream * output)
	{
		output->WriteInt32(1,   Valor);
		output->WriteString(2,  Nome);
		output->WriteBool(3,    Flag);
		output->WriteInt32(4,   Valor1);
		output->WriteMessage(5, Sub);
	}
};


TesteMensagem teste3;


int gpb_main()
{
	MemoryStream ms;
	CodedOutputStream cos = CodedOutputStream(&ms);
	CodedInputStream cis = CodedInputStream(&ms);
	
	TesteMensagem teste1;
	teste1.Valor = 123456789;
	teste1.Nome  = "olvebra";
	teste1.Flag  = true;
	teste1.Valor1 = 1024;
	
	teste1.Sub.SubValor = 234;
	teste1.Sub.SubStr = "ducker";
	
	teste1.WriteTo(&cos);
	
	Debug::HexDump(StdOut, ms.GetReadPtr(), ms.Length());
	
	// int bufferSize = ms.Length();
	// uint8_t buffer[bufferSize];
	// ms.Read(buffer, bufferSize);
	// Debug::HexDump(StdOut, buffer, bufferSize);

	TesteMensagem teste2 = TesteMensagem();
	teste2.MergeFrom(&cis);
	
	printf("\n");
	printf("[teste1: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS=%s]\n", teste1.Valor, teste1.Nome.c_str, teste1.Flag, teste1.Valor1, teste1.Sub.SubValor, teste1.Sub.SubStr.c_str);
	printf("[teste2: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS=%s]\n", teste2.Valor, teste2.Nome.c_str, teste2.Flag, teste2.Valor1, teste2.Sub.SubValor, teste2.Sub.SubStr.c_str);
	printf("[teste3: V=%d, S=%s, F=%d, V1=%d, SubV=%d, SubS=%s]\n", teste3.Valor, teste3.Nome.c_str, teste3.Flag, teste3.Valor1, teste3.Sub.SubValor, teste3.Sub.SubStr.c_str);

	return 0;
}
