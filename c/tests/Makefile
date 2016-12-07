

# -O0 -g -Wall -std=gnu++11 -pthread -L. -nodefaultlibs -fno-rtti -fno-exceptions
CXXFLAGS := -O0 -g -Wall -std=gnu++11 -pthread -L. -fno-rtti -fno-exceptions
# CXXFLAGS := -O3 --save-temps -Wall -pthread -L.
OBJS := Logger.o ByteBuffer.o Stream.o TcpClient.o TcpListener.o Dns.o IPAddress.o Socket.o StringBuilder.o Util.o Debug.o Ptr.o Fd.o Protobuf.o Binary.o Text.o Threading.o IO.o FdSelect.o MemoryStream.o
EXECS := main libzec.a


all: $(EXECS)

redo: clean all

main: main.cpp teste.cpp outro.cpp gpb.cpp $(OBJS)

libzec.a: $(OBJS)
	ar rc $@ $^

install-lib: libzec.a
	cp libzec.a /usr/local/lib

clean:
	rm -f *.o *.ii *.s $(EXECS)
	rm -rf Debug

check:
	cppcheck --enable=all . 2>&1

