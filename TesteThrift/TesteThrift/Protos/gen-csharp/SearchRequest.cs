/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;


/// <summary>
/// That just about covers the basics. Take a look in the test/ folder for more
/// detailed examples. After you run this file, your generated code shows up
/// in folders with names gen-<language>. The generated code isn't too scary
/// to look at. It even has pretty indentation.
/// </summary>
#if !SILVERLIGHT
[Serializable]
#endif
public partial class SearchRequest : TBase
{
  private string _FileMask;
  private string _StartPath;
  private bool _IgnoreErrors;

  public string FileMask
  {
    get
    {
      return _FileMask;
    }
    set
    {
      __isset.FileMask = true;
      this._FileMask = value;
    }
  }

  public string StartPath
  {
    get
    {
      return _StartPath;
    }
    set
    {
      __isset.StartPath = true;
      this._StartPath = value;
    }
  }

  public bool IgnoreErrors
  {
    get
    {
      return _IgnoreErrors;
    }
    set
    {
      __isset.IgnoreErrors = true;
      this._IgnoreErrors = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool FileMask;
    public bool StartPath;
    public bool IgnoreErrors;
  }

  public SearchRequest() {
  }

  public void Read (TProtocol iprot)
  {
    TField field;
    iprot.ReadStructBegin();
    while (true)
    {
      field = iprot.ReadFieldBegin();
      if (field.Type == TType.Stop) { 
        break;
      }
      switch (field.ID)
      {
        case 1:
          if (field.Type == TType.String) {
            FileMask = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 2:
          if (field.Type == TType.String) {
            StartPath = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.Bool) {
            IgnoreErrors = iprot.ReadBool();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        default: 
          TProtocolUtil.Skip(iprot, field.Type);
          break;
      }
      iprot.ReadFieldEnd();
    }
    iprot.ReadStructEnd();
  }

  public void Write(TProtocol oprot) {
    TStruct struc = new TStruct("SearchRequest");
    oprot.WriteStructBegin(struc);
    TField field = new TField();
    if (FileMask != null && __isset.FileMask) {
      field.Name = "FileMask";
      field.Type = TType.String;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(FileMask);
      oprot.WriteFieldEnd();
    }
    if (StartPath != null && __isset.StartPath) {
      field.Name = "StartPath";
      field.Type = TType.String;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(StartPath);
      oprot.WriteFieldEnd();
    }
    if (__isset.IgnoreErrors) {
      field.Name = "IgnoreErrors";
      field.Type = TType.Bool;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      oprot.WriteBool(IgnoreErrors);
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("SearchRequest(");
    sb.Append("FileMask: ");
    sb.Append(FileMask);
    sb.Append(",StartPath: ");
    sb.Append(StartPath);
    sb.Append(",IgnoreErrors: ");
    sb.Append(IgnoreErrors);
    sb.Append(")");
    return sb.ToString();
  }

}
