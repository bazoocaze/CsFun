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
	return 0;
}
