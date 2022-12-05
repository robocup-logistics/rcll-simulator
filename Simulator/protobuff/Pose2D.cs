// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Pose2D.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from Pose2D.proto</summary>
  public static partial class Pose2DReflection {

    #region Descriptor
    /// <summary>File descriptor for Pose2D.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static Pose2DReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxQb3NlMkQucHJvdG8SCWxsc2ZfbXNncxoKVGltZS5wcm90byJPCgZQb3Nl",
            "MkQSIgoJdGltZXN0YW1wGAEgAigLMg8ubGxzZl9tc2dzLlRpbWUSCQoBeBgC",
            "IAIoAhIJCgF5GAMgAigCEgsKA29yaRgEIAIoAkIvCh9vcmcucm9ib2N1cF9s",
            "b2dpc3RpY3MubGxzZl9tc2dzQgxQb3NlMkRQcm90b3M="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Llsfmsgs.TimeReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.Pose2D), global::LlsfMsgs.Pose2D.Parser, new[]{ "Timestamp", "X", "Y", "Ori" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// Pose information on 2D map
  /// Data is relative to the LLSF field frame
  /// </summary>
  public sealed partial class Pose2D : pb::IMessage<Pose2D>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Pose2D> _parser = new pb::MessageParser<Pose2D>(() => new Pose2D());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Pose2D> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.Pose2DReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pose2D() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pose2D(Pose2D other) : this() {
      _hasBits0 = other._hasBits0;
      timestamp_ = other.timestamp_ != null ? other.timestamp_.Clone() : null;
      x_ = other.x_;
      y_ = other.y_;
      ori_ = other.ori_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pose2D Clone() {
      return new Pose2D(this);
    }

    /// <summary>Field number for the "timestamp" field.</summary>
    public const int TimestampFieldNumber = 1;
    private global::Llsfmsgs.Time timestamp_;
    /// <summary>
    /// Time when this pose was measured
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Llsfmsgs.Time Timestamp {
      get { return timestamp_; }
      set {
        timestamp_ = value;
      }
    }

    /// <summary>Field number for the "x" field.</summary>
    public const int XFieldNumber = 2;
    private readonly static float XDefaultValue = 0F;

    private float x_;
    /// <summary>
    /// X/Y coordinates in meters
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float X {
      get { if ((_hasBits0 & 1) != 0) { return x_; } else { return XDefaultValue; } }
      set {
        _hasBits0 |= 1;
        x_ = value;
      }
    }
    /// <summary>Gets whether the "x" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasX {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "x" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearX() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "y" field.</summary>
    public const int YFieldNumber = 3;
    private readonly static float YDefaultValue = 0F;

    private float y_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Y {
      get { if ((_hasBits0 & 2) != 0) { return y_; } else { return YDefaultValue; } }
      set {
        _hasBits0 |= 2;
        y_ = value;
      }
    }
    /// <summary>Gets whether the "y" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasY {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "y" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearY() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "ori" field.</summary>
    public const int OriFieldNumber = 4;
    private readonly static float OriDefaultValue = 0F;

    private float ori_;
    /// <summary>
    /// Orientation in rad
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Ori {
      get { if ((_hasBits0 & 4) != 0) { return ori_; } else { return OriDefaultValue; } }
      set {
        _hasBits0 |= 4;
        ori_ = value;
      }
    }
    /// <summary>Gets whether the "ori" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasOri {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "ori" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearOri() {
      _hasBits0 &= ~4;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Pose2D);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Pose2D other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Timestamp, other.Timestamp)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(X, other.X)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Y, other.Y)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Ori, other.Ori)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (timestamp_ != null) hash ^= Timestamp.GetHashCode();
      if (HasX) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(X);
      if (HasY) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Y);
      if (HasOri) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Ori);
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (timestamp_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Timestamp);
      }
      if (HasX) {
        output.WriteRawTag(21);
        output.WriteFloat(X);
      }
      if (HasY) {
        output.WriteRawTag(29);
        output.WriteFloat(Y);
      }
      if (HasOri) {
        output.WriteRawTag(37);
        output.WriteFloat(Ori);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (timestamp_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Timestamp);
      }
      if (HasX) {
        output.WriteRawTag(21);
        output.WriteFloat(X);
      }
      if (HasY) {
        output.WriteRawTag(29);
        output.WriteFloat(Y);
      }
      if (HasOri) {
        output.WriteRawTag(37);
        output.WriteFloat(Ori);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (timestamp_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Timestamp);
      }
      if (HasX) {
        size += 1 + 4;
      }
      if (HasY) {
        size += 1 + 4;
      }
      if (HasOri) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Pose2D other) {
      if (other == null) {
        return;
      }
      if (other.timestamp_ != null) {
        if (timestamp_ == null) {
          Timestamp = new global::Llsfmsgs.Time();
        }
        Timestamp.MergeFrom(other.Timestamp);
      }
      if (other.HasX) {
        X = other.X;
      }
      if (other.HasY) {
        Y = other.Y;
      }
      if (other.HasOri) {
        Ori = other.Ori;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (timestamp_ == null) {
              Timestamp = new global::Llsfmsgs.Time();
            }
            input.ReadMessage(Timestamp);
            break;
          }
          case 21: {
            X = input.ReadFloat();
            break;
          }
          case 29: {
            Y = input.ReadFloat();
            break;
          }
          case 37: {
            Ori = input.ReadFloat();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (timestamp_ == null) {
              Timestamp = new global::Llsfmsgs.Time();
            }
            input.ReadMessage(Timestamp);
            break;
          }
          case 21: {
            X = input.ReadFloat();
            break;
          }
          case 29: {
            Y = input.ReadFloat();
            break;
          }
          case 37: {
            Ori = input.ReadFloat();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
