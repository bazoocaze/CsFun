
PROTOC  := ../../packages/Grpc.Tools.1.0.0/tools/windows_x86/protoc.exe
GRPCGEN := ../../packages/Grpc.Tools.1.0.0/tools/windows_x86/grpc_csharp_plugin.exe
ROOTINC := ../../packages/Grpc.Tools.1.0.0/tools

MyProtos.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out . --grpc_out . --plugin=protoc-gen-grpc=$(GRPCGEN)

MyProtosGrpc.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out . --grpc_out . --plugin=protoc-gen-grpc=$(GRPCGEN)
