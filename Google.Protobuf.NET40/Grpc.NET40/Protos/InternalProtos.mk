
# Localiza o diretorio do NuPacote Google.Protobuf.Tools
PROTOC_DIR := $(firstword $(wildcard ../../packages/Google.Protobuf.Tools.*))

ifeq (,$(PROTOC_DIR))
  $(error NuPackage Google.Protobuf.Tools not found)
endif

PROTOC := $(PROTOC_DIR)/tools/windows_x86/protoc.exe
ROOTINC := $(PROTOC_DIR)/tools

InternalProtos.cs: InternalProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out .
