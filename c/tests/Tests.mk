##
## Auto Generated makefile by CodeLite IDE
## any manual changes will be erased      
##
## Debug
ProjectName            :=Tests
ConfigurationName      :=Debug
WorkspacePath          := "/dados/zdesenv/CodeliteWorkspace"
ProjectPath            := "/dados/zdesenv/CsFun/c/tests"
IntermediateDirectory  :=./Debug
OutDir                 := $(IntermediateDirectory)
CurrentFileName        :=
CurrentFilePath        :=
CurrentFileFullPath    :=
User                   :=usuario
Date                   :=30/11/16
CodeLitePath           :="/home/usuario/.codelite"
LinkerName             :=/usr/bin/g++
SharedObjectLinkerName :=/usr/bin/g++ -shared -fPIC
ObjectSuffix           :=.o
DependSuffix           :=.o.d
PreprocessSuffix       :=.i
DebugSwitch            :=-g 
IncludeSwitch          :=-I
LibrarySwitch          :=-l
OutputSwitch           :=-o 
LibraryPathSwitch      :=-L
PreprocessorSwitch     :=-D
SourceSwitch           :=-c 
OutputFile             :=$(IntermediateDirectory)/$(ProjectName)
Preprocessors          :=
ObjectSwitch           :=-o 
ArchiveOutputSwitch    := 
PreprocessOnlySwitch   :=-E
ObjectsFileList        :="Tests.txt"
PCHCompileFlags        :=
MakeDirCommand         :=mkdir -p
LinkOptions            :=  
IncludePath            :=  $(IncludeSwitch). 
IncludePCH             := 
RcIncludePath          := 
Libs                   := $(LibrarySwitch)pthread 
ArLibs                 :=  "pthread" 
LibPath                := $(LibraryPathSwitch). 

##
## Common variables
## AR, CXX, CC, AS, CXXFLAGS and CFLAGS can be overriden using an environment variables
##
AR       := /usr/bin/ar rcu
CXX      := /usr/bin/g++
CC       := /usr/bin/gcc
CXXFLAGS :=  -g -O0 -Wall $(Preprocessors)
CFLAGS   :=  -g -O0 -Wall $(Preprocessors)
ASFLAGS  := 
AS       := /usr/bin/as


##
## User defined environment variables
##
CodeLiteDir:=/usr/share/codelite
LANG:=en_US.UTF-8
Objects0=$(IntermediateDirectory)/main.cpp$(ObjectSuffix) $(IntermediateDirectory)/Binary.cpp$(ObjectSuffix) $(IntermediateDirectory)/ByteBuffer.cpp$(ObjectSuffix) $(IntermediateDirectory)/Debug.cpp$(ObjectSuffix) $(IntermediateDirectory)/Dns.cpp$(ObjectSuffix) $(IntermediateDirectory)/Fd.cpp$(ObjectSuffix) $(IntermediateDirectory)/IPAddress.cpp$(ObjectSuffix) $(IntermediateDirectory)/Logger.cpp$(ObjectSuffix) $(IntermediateDirectory)/Text.cpp$(ObjectSuffix) $(IntermediateDirectory)/Threading.cpp$(ObjectSuffix) \
	$(IntermediateDirectory)/Util.cpp$(ObjectSuffix) $(IntermediateDirectory)/Protobuf.cpp$(ObjectSuffix) $(IntermediateDirectory)/Ptr.cpp$(ObjectSuffix) $(IntermediateDirectory)/Socket.cpp$(ObjectSuffix) $(IntermediateDirectory)/Stream.cpp$(ObjectSuffix) $(IntermediateDirectory)/StringBuilder.cpp$(ObjectSuffix) $(IntermediateDirectory)/TcpClient.cpp$(ObjectSuffix) $(IntermediateDirectory)/TcpListener.cpp$(ObjectSuffix) $(IntermediateDirectory)/outro.cpp$(ObjectSuffix) $(IntermediateDirectory)/gpb.cpp$(ObjectSuffix) \
	$(IntermediateDirectory)/teste.cpp$(ObjectSuffix) $(IntermediateDirectory)/IO.cpp$(ObjectSuffix) $(IntermediateDirectory)/FdSelect.cpp$(ObjectSuffix) $(IntermediateDirectory)/MemoryStream.cpp$(ObjectSuffix) 



Objects=$(Objects0) 

##
## Main Build Targets 
##
.PHONY: all clean PreBuild PrePreBuild PostBuild MakeIntermediateDirs
all: $(OutputFile)

$(OutputFile): $(IntermediateDirectory)/.d $(Objects) 
	@$(MakeDirCommand) $(@D)
	@echo "" > $(IntermediateDirectory)/.d
	@echo $(Objects0)  > $(ObjectsFileList)
	$(LinkerName) $(OutputSwitch)$(OutputFile) @$(ObjectsFileList) $(LibPath) $(Libs) $(LinkOptions)

MakeIntermediateDirs:
	@test -d ./Debug || $(MakeDirCommand) ./Debug


$(IntermediateDirectory)/.d:
	@test -d ./Debug || $(MakeDirCommand) ./Debug

PreBuild:


##
## Objects
##
$(IntermediateDirectory)/main.cpp$(ObjectSuffix): main.cpp $(IntermediateDirectory)/main.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/main.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/main.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/main.cpp$(DependSuffix): main.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/main.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/main.cpp$(DependSuffix) -MM "main.cpp"

$(IntermediateDirectory)/main.cpp$(PreprocessSuffix): main.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/main.cpp$(PreprocessSuffix) "main.cpp"

$(IntermediateDirectory)/Binary.cpp$(ObjectSuffix): Binary.cpp $(IntermediateDirectory)/Binary.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Binary.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Binary.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Binary.cpp$(DependSuffix): Binary.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Binary.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Binary.cpp$(DependSuffix) -MM "Binary.cpp"

$(IntermediateDirectory)/Binary.cpp$(PreprocessSuffix): Binary.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Binary.cpp$(PreprocessSuffix) "Binary.cpp"

$(IntermediateDirectory)/ByteBuffer.cpp$(ObjectSuffix): ByteBuffer.cpp $(IntermediateDirectory)/ByteBuffer.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/ByteBuffer.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/ByteBuffer.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/ByteBuffer.cpp$(DependSuffix): ByteBuffer.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/ByteBuffer.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/ByteBuffer.cpp$(DependSuffix) -MM "ByteBuffer.cpp"

$(IntermediateDirectory)/ByteBuffer.cpp$(PreprocessSuffix): ByteBuffer.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/ByteBuffer.cpp$(PreprocessSuffix) "ByteBuffer.cpp"

$(IntermediateDirectory)/Debug.cpp$(ObjectSuffix): Debug.cpp $(IntermediateDirectory)/Debug.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Debug.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Debug.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Debug.cpp$(DependSuffix): Debug.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Debug.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Debug.cpp$(DependSuffix) -MM "Debug.cpp"

$(IntermediateDirectory)/Debug.cpp$(PreprocessSuffix): Debug.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Debug.cpp$(PreprocessSuffix) "Debug.cpp"

$(IntermediateDirectory)/Dns.cpp$(ObjectSuffix): Dns.cpp $(IntermediateDirectory)/Dns.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Dns.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Dns.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Dns.cpp$(DependSuffix): Dns.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Dns.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Dns.cpp$(DependSuffix) -MM "Dns.cpp"

$(IntermediateDirectory)/Dns.cpp$(PreprocessSuffix): Dns.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Dns.cpp$(PreprocessSuffix) "Dns.cpp"

$(IntermediateDirectory)/Fd.cpp$(ObjectSuffix): Fd.cpp $(IntermediateDirectory)/Fd.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Fd.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Fd.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Fd.cpp$(DependSuffix): Fd.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Fd.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Fd.cpp$(DependSuffix) -MM "Fd.cpp"

$(IntermediateDirectory)/Fd.cpp$(PreprocessSuffix): Fd.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Fd.cpp$(PreprocessSuffix) "Fd.cpp"

$(IntermediateDirectory)/IPAddress.cpp$(ObjectSuffix): IPAddress.cpp $(IntermediateDirectory)/IPAddress.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/IPAddress.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/IPAddress.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/IPAddress.cpp$(DependSuffix): IPAddress.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/IPAddress.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/IPAddress.cpp$(DependSuffix) -MM "IPAddress.cpp"

$(IntermediateDirectory)/IPAddress.cpp$(PreprocessSuffix): IPAddress.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/IPAddress.cpp$(PreprocessSuffix) "IPAddress.cpp"

$(IntermediateDirectory)/Logger.cpp$(ObjectSuffix): Logger.cpp $(IntermediateDirectory)/Logger.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Logger.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Logger.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Logger.cpp$(DependSuffix): Logger.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Logger.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Logger.cpp$(DependSuffix) -MM "Logger.cpp"

$(IntermediateDirectory)/Logger.cpp$(PreprocessSuffix): Logger.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Logger.cpp$(PreprocessSuffix) "Logger.cpp"

$(IntermediateDirectory)/Text.cpp$(ObjectSuffix): Text.cpp $(IntermediateDirectory)/Text.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Text.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Text.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Text.cpp$(DependSuffix): Text.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Text.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Text.cpp$(DependSuffix) -MM "Text.cpp"

$(IntermediateDirectory)/Text.cpp$(PreprocessSuffix): Text.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Text.cpp$(PreprocessSuffix) "Text.cpp"

$(IntermediateDirectory)/Threading.cpp$(ObjectSuffix): Threading.cpp $(IntermediateDirectory)/Threading.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Threading.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Threading.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Threading.cpp$(DependSuffix): Threading.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Threading.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Threading.cpp$(DependSuffix) -MM "Threading.cpp"

$(IntermediateDirectory)/Threading.cpp$(PreprocessSuffix): Threading.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Threading.cpp$(PreprocessSuffix) "Threading.cpp"

$(IntermediateDirectory)/Util.cpp$(ObjectSuffix): Util.cpp $(IntermediateDirectory)/Util.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Util.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Util.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Util.cpp$(DependSuffix): Util.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Util.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Util.cpp$(DependSuffix) -MM "Util.cpp"

$(IntermediateDirectory)/Util.cpp$(PreprocessSuffix): Util.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Util.cpp$(PreprocessSuffix) "Util.cpp"

$(IntermediateDirectory)/Protobuf.cpp$(ObjectSuffix): Protobuf.cpp $(IntermediateDirectory)/Protobuf.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Protobuf.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Protobuf.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Protobuf.cpp$(DependSuffix): Protobuf.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Protobuf.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Protobuf.cpp$(DependSuffix) -MM "Protobuf.cpp"

$(IntermediateDirectory)/Protobuf.cpp$(PreprocessSuffix): Protobuf.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Protobuf.cpp$(PreprocessSuffix) "Protobuf.cpp"

$(IntermediateDirectory)/Ptr.cpp$(ObjectSuffix): Ptr.cpp $(IntermediateDirectory)/Ptr.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Ptr.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Ptr.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Ptr.cpp$(DependSuffix): Ptr.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Ptr.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Ptr.cpp$(DependSuffix) -MM "Ptr.cpp"

$(IntermediateDirectory)/Ptr.cpp$(PreprocessSuffix): Ptr.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Ptr.cpp$(PreprocessSuffix) "Ptr.cpp"

$(IntermediateDirectory)/Socket.cpp$(ObjectSuffix): Socket.cpp $(IntermediateDirectory)/Socket.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Socket.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Socket.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Socket.cpp$(DependSuffix): Socket.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Socket.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Socket.cpp$(DependSuffix) -MM "Socket.cpp"

$(IntermediateDirectory)/Socket.cpp$(PreprocessSuffix): Socket.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Socket.cpp$(PreprocessSuffix) "Socket.cpp"

$(IntermediateDirectory)/Stream.cpp$(ObjectSuffix): Stream.cpp $(IntermediateDirectory)/Stream.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/Stream.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/Stream.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/Stream.cpp$(DependSuffix): Stream.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/Stream.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/Stream.cpp$(DependSuffix) -MM "Stream.cpp"

$(IntermediateDirectory)/Stream.cpp$(PreprocessSuffix): Stream.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/Stream.cpp$(PreprocessSuffix) "Stream.cpp"

$(IntermediateDirectory)/StringBuilder.cpp$(ObjectSuffix): StringBuilder.cpp $(IntermediateDirectory)/StringBuilder.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/StringBuilder.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/StringBuilder.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/StringBuilder.cpp$(DependSuffix): StringBuilder.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/StringBuilder.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/StringBuilder.cpp$(DependSuffix) -MM "StringBuilder.cpp"

$(IntermediateDirectory)/StringBuilder.cpp$(PreprocessSuffix): StringBuilder.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/StringBuilder.cpp$(PreprocessSuffix) "StringBuilder.cpp"

$(IntermediateDirectory)/TcpClient.cpp$(ObjectSuffix): TcpClient.cpp $(IntermediateDirectory)/TcpClient.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/TcpClient.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/TcpClient.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/TcpClient.cpp$(DependSuffix): TcpClient.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/TcpClient.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/TcpClient.cpp$(DependSuffix) -MM "TcpClient.cpp"

$(IntermediateDirectory)/TcpClient.cpp$(PreprocessSuffix): TcpClient.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/TcpClient.cpp$(PreprocessSuffix) "TcpClient.cpp"

$(IntermediateDirectory)/TcpListener.cpp$(ObjectSuffix): TcpListener.cpp $(IntermediateDirectory)/TcpListener.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/TcpListener.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/TcpListener.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/TcpListener.cpp$(DependSuffix): TcpListener.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/TcpListener.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/TcpListener.cpp$(DependSuffix) -MM "TcpListener.cpp"

$(IntermediateDirectory)/TcpListener.cpp$(PreprocessSuffix): TcpListener.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/TcpListener.cpp$(PreprocessSuffix) "TcpListener.cpp"

$(IntermediateDirectory)/outro.cpp$(ObjectSuffix): outro.cpp $(IntermediateDirectory)/outro.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/outro.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/outro.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/outro.cpp$(DependSuffix): outro.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/outro.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/outro.cpp$(DependSuffix) -MM "outro.cpp"

$(IntermediateDirectory)/outro.cpp$(PreprocessSuffix): outro.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/outro.cpp$(PreprocessSuffix) "outro.cpp"

$(IntermediateDirectory)/gpb.cpp$(ObjectSuffix): gpb.cpp $(IntermediateDirectory)/gpb.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/gpb.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/gpb.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/gpb.cpp$(DependSuffix): gpb.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/gpb.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/gpb.cpp$(DependSuffix) -MM "gpb.cpp"

$(IntermediateDirectory)/gpb.cpp$(PreprocessSuffix): gpb.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/gpb.cpp$(PreprocessSuffix) "gpb.cpp"

$(IntermediateDirectory)/teste.cpp$(ObjectSuffix): teste.cpp $(IntermediateDirectory)/teste.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/teste.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/teste.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/teste.cpp$(DependSuffix): teste.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/teste.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/teste.cpp$(DependSuffix) -MM "teste.cpp"

$(IntermediateDirectory)/teste.cpp$(PreprocessSuffix): teste.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/teste.cpp$(PreprocessSuffix) "teste.cpp"

$(IntermediateDirectory)/IO.cpp$(ObjectSuffix): IO.cpp $(IntermediateDirectory)/IO.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/IO.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/IO.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/IO.cpp$(DependSuffix): IO.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/IO.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/IO.cpp$(DependSuffix) -MM "IO.cpp"

$(IntermediateDirectory)/IO.cpp$(PreprocessSuffix): IO.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/IO.cpp$(PreprocessSuffix) "IO.cpp"

$(IntermediateDirectory)/FdSelect.cpp$(ObjectSuffix): FdSelect.cpp $(IntermediateDirectory)/FdSelect.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/FdSelect.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/FdSelect.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/FdSelect.cpp$(DependSuffix): FdSelect.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/FdSelect.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/FdSelect.cpp$(DependSuffix) -MM "FdSelect.cpp"

$(IntermediateDirectory)/FdSelect.cpp$(PreprocessSuffix): FdSelect.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/FdSelect.cpp$(PreprocessSuffix) "FdSelect.cpp"

$(IntermediateDirectory)/MemoryStream.cpp$(ObjectSuffix): MemoryStream.cpp $(IntermediateDirectory)/MemoryStream.cpp$(DependSuffix)
	$(CXX) $(IncludePCH) $(SourceSwitch) "/dados/zdesenv/CsFun/c/tests/MemoryStream.cpp" $(CXXFLAGS) $(ObjectSwitch)$(IntermediateDirectory)/MemoryStream.cpp$(ObjectSuffix) $(IncludePath)
$(IntermediateDirectory)/MemoryStream.cpp$(DependSuffix): MemoryStream.cpp
	@$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) -MG -MP -MT$(IntermediateDirectory)/MemoryStream.cpp$(ObjectSuffix) -MF$(IntermediateDirectory)/MemoryStream.cpp$(DependSuffix) -MM "MemoryStream.cpp"

$(IntermediateDirectory)/MemoryStream.cpp$(PreprocessSuffix): MemoryStream.cpp
	$(CXX) $(CXXFLAGS) $(IncludePCH) $(IncludePath) $(PreprocessOnlySwitch) $(OutputSwitch) $(IntermediateDirectory)/MemoryStream.cpp$(PreprocessSuffix) "MemoryStream.cpp"


-include $(IntermediateDirectory)/*$(DependSuffix)
##
## Clean
##
clean:
	$(RM) -r ./Debug/


