

CXXFLAGS := -g
OBJS := Config.o Logger.o ByteBuffer.o Print.o Stream.o TcpClient.o TcpListener.o Dns.o IPAddress.o Socket.o StringBuilder.o Util.o Debug.o Ptr.o Fd.o Protobuf.o BinaryWriter.o


all: output teste gpb


output: output.cpp $(OBJS)

teste: teste.cpp $(OBJS)

gpb: gpb.cpp $(OBJS)




clean:
	rm -f *.o output teste gpb

