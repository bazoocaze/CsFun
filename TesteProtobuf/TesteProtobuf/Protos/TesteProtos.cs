// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: TesteProtos.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace TesteProtobuf.Protos {

  /// <summary>Holder for reflection information generated from TesteProtos.proto</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class TesteProtosReflection {

    #region Descriptor
    /// <summary>File descriptor for TesteProtos.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TesteProtosReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFUZXN0ZVByb3Rvcy5wcm90bxIUVGVzdGVQcm90b2J1Zi5wcm90b3MaHmdv",
            "b2dsZS9wcm90b2J1Zi93cmFwcGVycy5wcm90bxoZZ29vZ2xlL3Byb3RvYnVm",
            "L2FueS5wcm90byL/AQoMUGVyc29uUHJvdG9zEgwKBG5hbWUYASABKAkSCgoC",
            "aWQYAiABKAUSDQoFZW1haWwYAyABKAkSPgoGcGhvbmVzGAQgAygLMi4uVGVz",
            "dGVQcm90b2J1Zi5wcm90b3MuUGVyc29uUHJvdG9zLlBob25lTnVtYmVyGlkK",
            "C1Bob25lTnVtYmVyEg4KBm51bWJlchgBIAEoCRI6CgR0eXBlGAIgASgOMiwu",
            "VGVzdGVQcm90b2J1Zi5wcm90b3MuUGVyc29uUHJvdG9zLlBob25lVHlwZSIr",
            "CglQaG9uZVR5cGUSCgoGTU9CSUxFEAASCAoESE9NRRABEggKBFdPUksQAiJH",
            "ChFBZGRyZXNzQm9va1Byb3RvcxIyCgZwZW9wbGUYASADKAsyIi5UZXN0ZVBy",
            "b3RvYnVmLnByb3Rvcy5QZXJzb25Qcm90b3MiKAoMTWV1Q2FiZWNhbGhvEgoK",
            "AmlkGAEgASgFEgwKBHRpcG8YAiABKAUiJgoJTWV1c0RhZG9zEgoKAmlkGAEg",
            "ASgFEg0KBXRleHRvGAIgASgJIm4KCU1ldVBhY290ZRIyCgZoZWFkZXIYASAB",
            "KAsyIi5UZXN0ZVByb3RvYnVmLnByb3Rvcy5NZXVDYWJlY2FsaG8SLQoEZGF0",
            "YRgCIAEoCzIfLlRlc3RlUHJvdG9idWYucHJvdG9zLk1ldXNEYWRvc2IGcHJv",
            "dG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.WrappersReflection.Descriptor, global::Google.Protobuf.WellKnownTypes.AnyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.PersonProtos), global::TesteProtobuf.Protos.PersonProtos.Parser, new[]{ "Name", "Id", "Email", "Phones" }, null, new[]{ typeof(global::TesteProtobuf.Protos.PersonProtos.Types.PhoneType) }, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber), global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber.Parser, new[]{ "Number", "Type" }, null, null, null)}),
            new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.AddressBookProtos), global::TesteProtobuf.Protos.AddressBookProtos.Parser, new[]{ "People" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.MeuCabecalho), global::TesteProtobuf.Protos.MeuCabecalho.Parser, new[]{ "Id", "Tipo" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.MeusDados), global::TesteProtobuf.Protos.MeusDados.Parser, new[]{ "Id", "Texto" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::TesteProtobuf.Protos.MeuPacote), global::TesteProtobuf.Protos.MeuPacote.Parser, new[]{ "Header", "Data" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PersonProtos : pb::IMessage<PersonProtos> {
    private static readonly pb::MessageParser<PersonProtos> _parser = new pb::MessageParser<PersonProtos>(() => new PersonProtos());
    public static pb::MessageParser<PersonProtos> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::TesteProtobuf.Protos.TesteProtosReflection.Descriptor.MessageTypes[0]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PersonProtos() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PersonProtos(PersonProtos other) : this() {
      name_ = other.name_;
      id_ = other.id_;
      email_ = other.email_;
      phones_ = other.phones_.Clone();
    }

    public PersonProtos Clone() {
      return new PersonProtos(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 2;
    private int id_;
    /// <summary>
    ///  Unique ID number for this person.
    /// </summary>
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "email" field.</summary>
    public const int EmailFieldNumber = 3;
    private string email_ = "";
    public string Email {
      get { return email_; }
      set {
        email_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "phones" field.</summary>
    public const int PhonesFieldNumber = 4;
    private static readonly pb::FieldCodec<global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber> _repeated_phones_codec
        = pb::FieldCodec.ForMessage(34, global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber.Parser);
    private readonly pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber> phones_ = new pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber>();
    public pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos.Types.PhoneNumber> Phones {
      get { return phones_; }
    }

    public override bool Equals(object other) {
      return Equals(other as PersonProtos);
    }

    public bool Equals(PersonProtos other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Id != other.Id) return false;
      if (Email != other.Email) return false;
      if(!phones_.Equals(other.phones_)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Id != 0) hash ^= Id.GetHashCode();
      if (Email.Length != 0) hash ^= Email.GetHashCode();
      hash ^= phones_.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Id != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Id);
      }
      if (Email.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Email);
      }
      phones_.WriteTo(output, _repeated_phones_codec);
    }

    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Email.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Email);
      }
      size += phones_.CalculateSize(_repeated_phones_codec);
      return size;
    }

    public void MergeFrom(PersonProtos other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
      if (other.Email.Length != 0) {
        Email = other.Email;
      }
      phones_.Add(other.phones_);
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 16: {
            Id = input.ReadInt32();
            break;
          }
          case 26: {
            Email = input.ReadString();
            break;
          }
          case 34: {
            phones_.AddEntriesFrom(input, _repeated_phones_codec);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the PersonProtos message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public static partial class Types {
      public enum PhoneType {
        [pbr::OriginalName("MOBILE")] Mobile = 0,
        [pbr::OriginalName("HOME")] Home = 1,
        [pbr::OriginalName("WORK")] Work = 2,
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
      public sealed partial class PhoneNumber : pb::IMessage<PhoneNumber> {
        private static readonly pb::MessageParser<PhoneNumber> _parser = new pb::MessageParser<PhoneNumber>(() => new PhoneNumber());
        public static pb::MessageParser<PhoneNumber> Parser { get { return _parser; } }

        public static pbr::MessageDescriptor Descriptor {
          get { return global::TesteProtobuf.Protos.PersonProtos.Descriptor.NestedTypes[0]; }
        }

        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        public PhoneNumber() {
          OnConstruction();
        }

        partial void OnConstruction();

        public PhoneNumber(PhoneNumber other) : this() {
          number_ = other.number_;
          type_ = other.type_;
        }

        public PhoneNumber Clone() {
          return new PhoneNumber(this);
        }

        /// <summary>Field number for the "number" field.</summary>
        public const int NumberFieldNumber = 1;
        private string number_ = "";
        public string Number {
          get { return number_; }
          set {
            number_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "type" field.</summary>
        public const int TypeFieldNumber = 2;
        private global::TesteProtobuf.Protos.PersonProtos.Types.PhoneType type_ = 0;
        public global::TesteProtobuf.Protos.PersonProtos.Types.PhoneType Type {
          get { return type_; }
          set {
            type_ = value;
          }
        }

        public override bool Equals(object other) {
          return Equals(other as PhoneNumber);
        }

        public bool Equals(PhoneNumber other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (Number != other.Number) return false;
          if (Type != other.Type) return false;
          return true;
        }

        public override int GetHashCode() {
          int hash = 1;
          if (Number.Length != 0) hash ^= Number.GetHashCode();
          if (Type != 0) hash ^= Type.GetHashCode();
          return hash;
        }

        public override string ToString() {
          return pb::JsonFormatter.ToDiagnosticString(this);
        }

        public void WriteTo(pb::CodedOutputStream output) {
          if (Number.Length != 0) {
            output.WriteRawTag(10);
            output.WriteString(Number);
          }
          if (Type != 0) {
            output.WriteRawTag(16);
            output.WriteEnum((int) Type);
          }
        }

        public int CalculateSize() {
          int size = 0;
          if (Number.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Number);
          }
          if (Type != 0) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
          }
          return size;
        }

        public void MergeFrom(PhoneNumber other) {
          if (other == null) {
            return;
          }
          if (other.Number.Length != 0) {
            Number = other.Number;
          }
          if (other.Type != 0) {
            Type = other.Type;
          }
        }

        public void MergeFrom(pb::CodedInputStream input) {
          uint tag;
          while ((tag = input.ReadTag()) != 0) {
            switch(tag) {
              default:
                input.SkipLastField();
                break;
              case 10: {
                Number = input.ReadString();
                break;
              }
              case 16: {
                type_ = (global::TesteProtobuf.Protos.PersonProtos.Types.PhoneType) input.ReadEnum();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  /// <summary>
  ///  Our address book file is just one of these.
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class AddressBookProtos : pb::IMessage<AddressBookProtos> {
    private static readonly pb::MessageParser<AddressBookProtos> _parser = new pb::MessageParser<AddressBookProtos>(() => new AddressBookProtos());
    public static pb::MessageParser<AddressBookProtos> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::TesteProtobuf.Protos.TesteProtosReflection.Descriptor.MessageTypes[1]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public AddressBookProtos() {
      OnConstruction();
    }

    partial void OnConstruction();

    public AddressBookProtos(AddressBookProtos other) : this() {
      people_ = other.people_.Clone();
    }

    public AddressBookProtos Clone() {
      return new AddressBookProtos(this);
    }

    /// <summary>Field number for the "people" field.</summary>
    public const int PeopleFieldNumber = 1;
    private static readonly pb::FieldCodec<global::TesteProtobuf.Protos.PersonProtos> _repeated_people_codec
        = pb::FieldCodec.ForMessage(10, global::TesteProtobuf.Protos.PersonProtos.Parser);
    private readonly pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos> people_ = new pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos>();
    public pbc::RepeatedField<global::TesteProtobuf.Protos.PersonProtos> People {
      get { return people_; }
    }

    public override bool Equals(object other) {
      return Equals(other as AddressBookProtos);
    }

    public bool Equals(AddressBookProtos other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!people_.Equals(other.people_)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      hash ^= people_.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      people_.WriteTo(output, _repeated_people_codec);
    }

    public int CalculateSize() {
      int size = 0;
      size += people_.CalculateSize(_repeated_people_codec);
      return size;
    }

    public void MergeFrom(AddressBookProtos other) {
      if (other == null) {
        return;
      }
      people_.Add(other.people_);
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            people_.AddEntriesFrom(input, _repeated_people_codec);
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class MeuCabecalho : pb::IMessage<MeuCabecalho> {
    private static readonly pb::MessageParser<MeuCabecalho> _parser = new pb::MessageParser<MeuCabecalho>(() => new MeuCabecalho());
    public static pb::MessageParser<MeuCabecalho> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::TesteProtobuf.Protos.TesteProtosReflection.Descriptor.MessageTypes[2]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public MeuCabecalho() {
      OnConstruction();
    }

    partial void OnConstruction();

    public MeuCabecalho(MeuCabecalho other) : this() {
      id_ = other.id_;
      tipo_ = other.tipo_;
    }

    public MeuCabecalho Clone() {
      return new MeuCabecalho(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private int id_;
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "tipo" field.</summary>
    public const int TipoFieldNumber = 2;
    private int tipo_;
    public int Tipo {
      get { return tipo_; }
      set {
        tipo_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as MeuCabecalho);
    }

    public bool Equals(MeuCabecalho other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Tipo != other.Tipo) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0) hash ^= Id.GetHashCode();
      if (Tipo != 0) hash ^= Tipo.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
      if (Tipo != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Tipo);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Tipo != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Tipo);
      }
      return size;
    }

    public void MergeFrom(MeuCabecalho other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
      if (other.Tipo != 0) {
        Tipo = other.Tipo;
      }
    }

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
          case 16: {
            Tipo = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class MeusDados : pb::IMessage<MeusDados> {
    private static readonly pb::MessageParser<MeusDados> _parser = new pb::MessageParser<MeusDados>(() => new MeusDados());
    public static pb::MessageParser<MeusDados> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::TesteProtobuf.Protos.TesteProtosReflection.Descriptor.MessageTypes[3]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public MeusDados() {
      OnConstruction();
    }

    partial void OnConstruction();

    public MeusDados(MeusDados other) : this() {
      id_ = other.id_;
      texto_ = other.texto_;
    }

    public MeusDados Clone() {
      return new MeusDados(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private int id_;
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "texto" field.</summary>
    public const int TextoFieldNumber = 2;
    private string texto_ = "";
    public string Texto {
      get { return texto_; }
      set {
        texto_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    public override bool Equals(object other) {
      return Equals(other as MeusDados);
    }

    public bool Equals(MeusDados other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Texto != other.Texto) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Id != 0) hash ^= Id.GetHashCode();
      if (Texto.Length != 0) hash ^= Texto.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
      if (Texto.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Texto);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Texto.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Texto);
      }
      return size;
    }

    public void MergeFrom(MeusDados other) {
      if (other == null) {
        return;
      }
      if (other.Id != 0) {
        Id = other.Id;
      }
      if (other.Texto.Length != 0) {
        Texto = other.Texto;
      }
    }

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
            Texto = input.ReadString();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class MeuPacote : pb::IMessage<MeuPacote> {
    private static readonly pb::MessageParser<MeuPacote> _parser = new pb::MessageParser<MeuPacote>(() => new MeuPacote());
    public static pb::MessageParser<MeuPacote> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::TesteProtobuf.Protos.TesteProtosReflection.Descriptor.MessageTypes[4]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public MeuPacote() {
      OnConstruction();
    }

    partial void OnConstruction();

    public MeuPacote(MeuPacote other) : this() {
      Header = other.header_ != null ? other.Header.Clone() : null;
      Data = other.data_ != null ? other.Data.Clone() : null;
    }

    public MeuPacote Clone() {
      return new MeuPacote(this);
    }

    /// <summary>Field number for the "header" field.</summary>
    public const int HeaderFieldNumber = 1;
    private global::TesteProtobuf.Protos.MeuCabecalho header_;
    public global::TesteProtobuf.Protos.MeuCabecalho Header {
      get { return header_; }
      set {
        header_ = value;
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 2;
    private global::TesteProtobuf.Protos.MeusDados data_;
    public global::TesteProtobuf.Protos.MeusDados Data {
      get { return data_; }
      set {
        data_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as MeuPacote);
    }

    public bool Equals(MeuPacote other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Header, other.Header)) return false;
      if (!object.Equals(Data, other.Data)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (header_ != null) hash ^= Header.GetHashCode();
      if (data_ != null) hash ^= Data.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (header_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Header);
      }
      if (data_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Data);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (header_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Header);
      }
      if (data_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Data);
      }
      return size;
    }

    public void MergeFrom(MeuPacote other) {
      if (other == null) {
        return;
      }
      if (other.header_ != null) {
        if (header_ == null) {
          header_ = new global::TesteProtobuf.Protos.MeuCabecalho();
        }
        Header.MergeFrom(other.Header);
      }
      if (other.data_ != null) {
        if (data_ == null) {
          data_ = new global::TesteProtobuf.Protos.MeusDados();
        }
        Data.MergeFrom(other.Data);
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            if (header_ == null) {
              header_ = new global::TesteProtobuf.Protos.MeuCabecalho();
            }
            input.ReadMessage(header_);
            break;
          }
          case 18: {
            if (data_ == null) {
              data_ = new global::TesteProtobuf.Protos.MeusDados();
            }
            input.ReadMessage(data_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code