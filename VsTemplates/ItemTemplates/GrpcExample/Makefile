
GRPCDIR := $(firstword $(wildcard ../../packages/Grpc.Tools.*))

ifeq (,$(GRPCDIR))
  $(error GRPC not found)
endif

PROTOC  := $(GRPCDIR)/tools/windows_x86/protoc.exe
GRPCGEN := $(GRPCDIR)/tools/windows_x86/grpc_csharp_plugin.exe
ROOTINC := $(GRPCDIR)/tools

MyProtos.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out . --grpc_out . --plugin=protoc-gen-grpc=$(GRPCGEN)

MyProtosGrpc.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out . --grpc_out . --plugin=protoc-gen-grpc=$(GRPCGEN)
