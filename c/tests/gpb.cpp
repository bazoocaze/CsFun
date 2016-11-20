#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "ByteBuffer.h"
#include "Debug.h"
#include "Protobuf.h"



class Teste : public IMessage
{
public:
	int Valor;
	char * Nome;
	bool Flag;


	int CalculateSize()
	{
		int ret = 0;
		ret += CodedOutputStream::CalculateInt32Size(1, Valor);
		ret += CodedOutputStream::CalculateStringSize(2, Nome);
		ret += CodedOutputStream::CalculateBoolSize(3, Flag);
		return ret;
	}

	void MergeFrom(CodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadInt32(1,  &Valor)) continue;
			if(input->ReadString(2, &Nome))  continue;
			if(input->ReadBool(3,   &Flag))  continue;
			input->SkipLastField();
		}
	}

	void WriteTo(CodedOutputStream * output)
	{
		output->WriteInt32(1,  Valor);
		output->WriteString(2, Nome);
		output->WriteBool(3,   Flag);
	}
};


int main()
{
	ByteBuffer tmp;
	CodedOutputStream cos = CodedOutputStream(&tmp);
	CodedInputStream cis = CodedInputStream(&tmp);
	
	Teste teste1;
	teste1.Valor = 123456789;
	teste1.Nome = strdup("olvebra");
	teste1.Flag = true;

	teste1.WriteTo(&cos);

	Debug::HexDump(tmp.Ptr(), tmp.Length());

	Teste teste2;
	teste2.MergeFrom(&cis);
	printf("\n[Valor=%d, Nome=%s, Flag=%d]\n", teste2.Valor, teste2.Nome, teste2.Flag);

	return 0;
}
