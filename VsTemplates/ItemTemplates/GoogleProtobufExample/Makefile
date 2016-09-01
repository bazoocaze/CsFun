
PROTOC := ../../packages/Google.Protobuf.Tools.3.0.0/tools/windows_x86/protoc.exe
ROOTINC := ../../packages/Google.Protobuf.Tools.3.0.0/tools

MyProtos.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out .
