

PROTOC:=..\..\packages\Google.Protobuf.Tools.3.0.0\tools\windows_x86\protoc.exe
DEFROOT:=..\..\packages\Google.Protobuf.Tools.3.0.0\tools

TesteProtos.cs: TesteProtos.proto
	$(PROTOC) -I $(DEFROOT) --proto_path . $< --csharp_out .
