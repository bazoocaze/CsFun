#include <stdio.h>
#include <stdlib.h>
#include <string.h>


#include "Ptr.h"
#include "Protobuf.h"
#include "TcpClient.h"
#include "Stream.h"
#include "Binary.h"

/*

class RpcPacket : public IMessage
{
public:
	String Name;
	int    Id;
	int    Options;
	int    PayloadSize;
	void * Payload;

	int CalculateSize() const
	{
		int ret = 0;
		ret += CodedOutputStream::CalculateStringSize(1, Name);
		ret += CodedOutputStream::CalculateInt32Size( 2, Id);
		ret += CodedOutputStream::CalculateInt32Size( 3, Options);
		ret += CodedOutputStream::CalculateInt32Size( 4, PayloadSize);
		// ret += CodedOutputStream::CalculateBytesSize( 5, PayloadSize);
		return ret;
	}

	void WriteTo(CodedOutputStream * output) 
	{
		output->WriteString(1, Name);
		output->WriteInt32( 2, Id);
		output->WriteInt32( 3, Options);
		output->WriteInt32( 4, PayloadSize);
		// output->WriteBytes( 5, Payload, PayloadSize);
	}

	void MergeFrom(CodedInputStream * input)
	{
		while(input->ReadTag())
		{
			if(input->ReadString(1, Name)) continue;
			if(input->ReadInt32( 2, Id)) continue;
			if(input->ReadInt32( 3, Options)) continue;
			if(input->ReadInt32( 4, PayloadSize)) continue;
			// if(input->ReadBytes( 5, Payload, PayloadSize)) continue;
			input->SkipLastField();
		}
	}
};


class MyServer
{
	void Start();
	void Stop();
};


class PbClient
{
protected:
	TcpClient m_client;
public:
	bool Connect(const IPAddress & address, int port)
	{
		m_client.Close();
	}

	void Close()
	{
		m_client.Close();
	}

	const char * GetLastErrorMsg()
	{
	}

	RpcPacket WaitResponse(int id)
	{
		int fd = m_client.GetFd();
		ByteBuffer buffer;
		RpcPacket ret;
		while(true)
		{
			BytePtr tmp = buffer.LockWrite(128);
			int lidos = read(fd, tmp.Ptr, tmp.Size);
			if(lidos < 0)
			{
				// error
				return ret;
			}
			if(lidos == 0)
			{
				// eos
				return ret;
			}
			buffer.ConfirmWrite(lidos);
		}	
	}

	RpcPacket SendRequest(RpcPacket& request)
	{
		int fd = m_client.GetFd();
		FdStream stream = FdStream(fd);
		BinaryWriter bw = BinaryWriter(&stream);
		CodedOutputStream cos = CodedOutputStream();
		bw.WriteInt32(request.CalculateSize());
		request.WriteTo(&cos);
		RpcPacket resp = WaitResponse(request.Id);
	}
};


class MyClient
{
protected:
	PbClient m_client;
public:
	bool Connect(const IPAddress & address, int port)
	{
		m_client.Close();
		if(!m_client.Connect(address, port)) return false;
		return true;
	}
	void Close()
	{
		m_client.Close();
	}
	const char * GetLastErrorMsg()
	{
		return m_client.GetLastErrorMsg();
	}
	void FillFrameBuffer(int fbId, int color)
	{
	}
};



void TestarCliente()
{
	MyClient client;
	if(!client.Connect(IPAddress::Loopback, 12345))
	{
		printf("[Cliente: falha de conexão: %s\n", client.GetLastErrorMsg());
		return;
	}
	client.FillFrameBuffer(0, 0xFF000000);
	client.Close();
}


 * */

void teste_main()
{
	printf("[BEGIN]\n");
	// TestarCliente();
	printf("[END]\n");
}
