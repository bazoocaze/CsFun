

CXXFLAGS := -g -pthread -L.
OBJS := Config.o Logger.o ByteBuffer.o Stream.o TcpClient.o TcpListener.o Dns.o IPAddress.o Socket.o StringBuilder.o Util.o Debug.o Ptr.o Fd.o Protobuf.o Binary.o Text.o Threading.o
EXECS := main teste gpb outro


all: $(EXECS)

redo: clean all

main: main.cpp $(OBJS)

teste: teste.cpp $(OBJS)

gpb: gpb.cpp $(OBJS)

outro: outro.cpp $(OBJS)


clean:
	rm -f *.o $(EXECS)

