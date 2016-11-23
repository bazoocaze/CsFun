

CXXFLAGS := -g -pthread -L.
OBJS := Logger.o ByteBuffer.o Stream.o TcpClient.o TcpListener.o Dns.o IPAddress.o Socket.o StringBuilder.o Util.o Debug.o Ptr.o Fd.o Protobuf.o Binary.o Text.o Threading.o
EXECS := main


all: $(EXECS)

redo: clean all

main: main.cpp teste.cpp outro.cpp gpb.cpp $(OBJS)

clean:
	rm -f *.o $(EXECS)

