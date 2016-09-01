// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: MyProtos.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Teste.Protos {

  /// <summary>Holder for reflection information generated from MyProtos.proto</summary>
  public static partial class MyProtosReflection {

    #region Descriptor
    /// <summary>File descriptor for MyProtos.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MyProtosReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5NeVByb3Rvcy5wcm90bxIMVGVzdGUuUHJvdG9zIiUKCU15TWVzc2FnZRIK",
            "CgJpZBgBIAEoBRIMCgRub21lGAIgASgJIl8KDVNlYXJjaFJlcXVlc3QSEQoJ",
            "ZmlsZV9tYXNrGAEgASgJEhEKCXN0YXJ0X2RpchgCIAEoCRIRCglyZWN1cnNp",
            "dmUYAyABKAgSFQoNaWdub3JlX2Vycm9ycxgEIAEoCCIyCg5TZWFyY2hSZXNw",
            "b25zZRIRCglmaWxlX25hbWUYASABKAkSDQoFZm91bmQYAiABKAgyVAoNU2Vh",
            "cmNoU2VydmljZRJDCgZTZWFyY2gSGy5UZXN0ZS5Qcm90b3MuU2VhcmNoUmVx",
            "dWVzdBocLlRlc3RlLlByb3Rvcy5TZWFyY2hSZXNwb25zZWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Teste.Protos.MyMessage), global::Teste.Protos.MyMessage.Parser, new[]{ "Id", "Nome" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Teste.Protos.SearchRequest), global::Teste.Protos.SearchRequest.Parser, new[]{ "FileMask", "StartDir", "Recursive", "IgnoreErrors" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Teste.Protos.SearchResponse), global::Teste.Protos.SearchResponse.Parser, new[]{ "FileName", "Found" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class MyMessage : pb::IMessage<MyMessage> {
    private static readonly pb::MessageParser<MyMessage> _parser = new pb::MessageParser<MyMessage>(() => new MyMessage());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MyMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Teste.Protos.MyProtosReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MyMessage() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MyMessage(MyMessage other) : this() {
      id_ = other.id_;
      nome_ = other.nome_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MyMessage Clone() {
      return new MyMessage(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private int id_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "nome" field.</summary>
    public const int NomeFieldNumber = 2;
    private string nome_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Nome {
      get { return nome_; }
      set {
        nome_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MyMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MyMessage other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Nome != other.Nome) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0) hash ^= Id.GetHashCode();
      if (Nome.Length != 0) hash ^= Nome.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
      if (Nome.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Nome);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Nome.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Nome);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MyMessage other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
      if (other.Nome.Length != 0) {
        Nome = other.Nome;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Id = input.ReadInt32();
            break;
          }
          case 18: {
            Nome = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class SearchRequest : pb::IMessage<SearchRequest> {
    private static readonly pb::MessageParser<SearchRequest> _parser = new pb::MessageParser<SearchRequest>(() => new SearchRequest());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SearchRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Teste.Protos.MyProtosReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchRequest(SearchRequest other) : this() {
      fileMask_ = other.fileMask_;
      startDir_ = other.startDir_;
      recursive_ = other.recursive_;
      ignoreErrors_ = other.ignoreErrors_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchRequest Clone() {
      return new SearchRequest(this);
    }

    /// <summary>Field number for the "file_mask" field.</summary>
    public const int FileMaskFieldNumber = 1;
    private string fileMask_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FileMask {
      get { return fileMask_; }
      set {
        fileMask_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "start_dir" field.</summary>
    public const int StartDirFieldNumber = 2;
    private string startDir_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string StartDir {
      get { return startDir_; }
      set {
        startDir_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "recursive" field.</summary>
    public const int RecursiveFieldNumber = 3;
    private bool recursive_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Recursive {
      get { return recursive_; }
      set {
        recursive_ = value;
      }
    }

    /// <summary>Field number for the "ignore_errors" field.</summary>
    public const int IgnoreErrorsFieldNumber = 4;
    private bool ignoreErrors_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IgnoreErrors {
      get { return ignoreErrors_; }
      set {
        ignoreErrors_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SearchRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SearchRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (FileMask != other.FileMask) return false;
      if (StartDir != other.StartDir) return false;
      if (Recursive != other.Recursive) return false;
      if (IgnoreErrors != other.IgnoreErrors) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (FileMask.Length != 0) hash ^= FileMask.GetHashCode();
      if (StartDir.Length != 0) hash ^= StartDir.GetHashCode();
      if (Recursive != false) hash ^= Recursive.GetHashCode();
      if (IgnoreErrors != false) hash ^= IgnoreErrors.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (FileMask.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(FileMask);
      }
      if (StartDir.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(StartDir);
      }
      if (Recursive != false) {
        output.WriteRawTag(24);
        output.WriteBool(Recursive);
      }
      if (IgnoreErrors != false) {
        output.WriteRawTag(32);
        output.WriteBool(IgnoreErrors);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (FileMask.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FileMask);
      }
      if (StartDir.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(StartDir);
      }
      if (Recursive != false) {
        size += 1 + 1;
      }
      if (IgnoreErrors != false) {
        size += 1 + 1;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SearchRequest other) {
      if (other == null) {
        return;
      }
      if (other.FileMask.Length != 0) {
        FileMask = other.FileMask;
      }
      if (other.StartDir.Length != 0) {
        StartDir = other.StartDir;
      }
      if (other.Recursive != false) {
        Recursive = other.Recursive;
      }
      if (other.IgnoreErrors != false) {
        IgnoreErrors = other.IgnoreErrors;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            FileMask = input.ReadString();
            break;
          }
          case 18: {
            StartDir = input.ReadString();
            break;
          }
          case 24: {
            Recursive = input.ReadBool();
            break;
          }
          case 32: {
            IgnoreErrors = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class SearchResponse : pb::IMessage<SearchResponse> {
    private static readonly pb::MessageParser<SearchResponse> _parser = new pb::MessageParser<SearchResponse>(() => new SearchResponse());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SearchResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Teste.Protos.MyProtosReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchResponse(SearchResponse other) : this() {
      fileName_ = other.fileName_;
      found_ = other.found_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SearchResponse Clone() {
      return new SearchResponse(this);
    }

    /// <summary>Field number for the "file_name" field.</summary>
    public const int FileNameFieldNumber = 1;
    private string fileName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FileName {
      get { return fileName_; }
      set {
        fileName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "found" field.</summary>
    public const int FoundFieldNumber = 2;
    private bool found_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Found {
      get { return found_; }
      set {
        found_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SearchResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SearchResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (FileName != other.FileName) return false;
      if (Found != other.Found) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (FileName.Length != 0) hash ^= FileName.GetHashCode();
      if (Found != false) hash ^= Found.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (FileName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(FileName);
      }
      if (Found != false) {
        output.WriteRawTag(16);
        output.WriteBool(Found);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (FileName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FileName);
      }
      if (Found != false) {
        size += 1 + 1;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SearchResponse other) {
      if (other == null) {
        return;
      }
      if (other.FileName.Length != 0) {
        FileName = other.FileName;
      }
      if (other.Found != false) {
        Found = other.Found;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            FileName = input.ReadString();
            break;
          }
          case 16: {
            Found = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
