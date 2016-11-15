
GPBTDIR := $(firstword $(wildcard ../../packages/Google.Protobuf.Tools.*))

ifeq (,$(GPBTDIR))
  $(error Google Protobuffer Tools not found)
endif

PROTOC  := $(GPBTDIR)/tools/windows_x86/protoc.exe
ROOTINC := $(GPBTDIR)/tools

MyProtos.cs: MyProtos.proto
	$(PROTOC) -I $(ROOTINC) -I . $< --csharp_out .
