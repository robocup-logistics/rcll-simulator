// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: WorkpieceInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from WorkpieceInfo.proto</summary>
  public static partial class WorkpieceInfoReflection {

    #region Descriptor
    /// <summary>File descriptor for WorkpieceInfo.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static WorkpieceInfoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChNXb3JrcGllY2VJbmZvLnByb3RvEglsbHNmX21zZ3MaClRlYW0ucHJvdG8a",
            "ElByb2R1Y3RDb2xvci5wcm90byKGAgoJV29ya3BpZWNlEgoKAmlkGAEgAigN",
            "EhIKCmF0X21hY2hpbmUYAiACKAkSDwoHdmlzaWJsZRgDIAEoAhIjCgp0ZWFt",
            "X2NvbG9yGAQgASgOMg8ubGxzZl9tc2dzLlRlYW0SKAoKYmFzZV9jb2xvchgF",
            "IAEoDjIULmxsc2ZfbXNncy5CYXNlQ29sb3ISKQoLcmluZ19jb2xvcnMYBiAD",
            "KA4yFC5sbHNmX21zZ3MuUmluZ0NvbG9yEiYKCWNhcF9jb2xvchgHIAEoDjIT",
            "Lmxsc2ZfbXNncy5DYXBDb2xvciImCghDb21wVHlwZRIMCgdDT01QX0lEENAP",
            "EgwKCE1TR19UWVBFEDciYQoNV29ya3BpZWNlSW5mbxIoCgp3b3JrcGllY2Vz",
            "GAEgAygLMhQubGxzZl9tc2dzLldvcmtwaWVjZSImCghDb21wVHlwZRIMCgdD",
            "T01QX0lEENAPEgwKCE1TR19UWVBFEDgicAoQV29ya3BpZWNlQWRkUmluZxIK",
            "CgJpZBgBIAIoDRIoCgpyaW5nX2NvbG9yGAIgAigOMhQubGxzZl9tc2dzLlJp",
            "bmdDb2xvciImCghDb21wVHlwZRIMCgdDT01QX0lEENAPEgwKCE1TR19UWVBF",
            "EDlCNgofb3JnLnJvYm9jdXBfbG9naXN0aWNzLmxsc2ZfbXNnc0ITV29ya3Bp",
            "ZWNlSW5mb1Byb3Rvcw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::LlsfMsgs.TeamReflection.Descriptor, global::LlsfMsgs.ProductColorReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.Workpiece), global::LlsfMsgs.Workpiece.Parser, new[]{ "Id", "AtMachine", "Visible", "TeamColor", "BaseColor", "RingColors", "CapColor" }, null, new[]{ typeof(global::LlsfMsgs.Workpiece.Types.CompType) }, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.WorkpieceInfo), global::LlsfMsgs.WorkpieceInfo.Parser, new[]{ "Workpieces" }, null, new[]{ typeof(global::LlsfMsgs.WorkpieceInfo.Types.CompType) }, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.WorkpieceAddRing), global::LlsfMsgs.WorkpieceAddRing.Parser, new[]{ "Id", "RingColor" }, null, new[]{ typeof(global::LlsfMsgs.WorkpieceAddRing.Types.CompType) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Workpiece : pb::IMessage<Workpiece>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Workpiece> _parser = new pb::MessageParser<Workpiece>(() => new Workpiece());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Workpiece> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.WorkpieceInfoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Workpiece() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Workpiece(Workpiece other) : this() {
      _hasBits0 = other._hasBits0;
      id_ = other.id_;
      atMachine_ = other.atMachine_;
      visible_ = other.visible_;
      teamColor_ = other.teamColor_;
      baseColor_ = other.baseColor_;
      ringColors_ = other.ringColors_.Clone();
      capColor_ = other.capColor_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Workpiece Clone() {
      return new Workpiece(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private readonly static uint IdDefaultValue = 0;

    private uint id_;
    /// <summary>
    /// Puck unique ID
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Id {
      get { if ((_hasBits0 & 1) != 0) { return id_; } else { return IdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        id_ = value;
      }
    }
    /// <summary>Gets whether the "id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "at_machine" field.</summary>
    public const int AtMachineFieldNumber = 2;
    private readonly static string AtMachineDefaultValue = "";

    private string atMachine_;
    /// <summary>
    /// Machine which detected the workpiece
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AtMachine {
      get { return atMachine_ ?? AtMachineDefaultValue; }
      set {
        atMachine_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "at_machine" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasAtMachine {
      get { return atMachine_ != null; }
    }
    /// <summary>Clears the value of the "at_machine" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearAtMachine() {
      atMachine_ = null;
    }

    /// <summary>Field number for the "visible" field.</summary>
    public const int VisibleFieldNumber = 3;
    private readonly static float VisibleDefaultValue = 0F;

    private float visible_;
    /// <summary>
    /// last seen at machine
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Visible {
      get { if ((_hasBits0 & 2) != 0) { return visible_; } else { return VisibleDefaultValue; } }
      set {
        _hasBits0 |= 2;
        visible_ = value;
      }
    }
    /// <summary>Gets whether the "visible" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasVisible {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "visible" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearVisible() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "team_color" field.</summary>
    public const int TeamColorFieldNumber = 4;
    private readonly static global::LlsfMsgs.Team TeamColorDefaultValue = global::LlsfMsgs.Team.Cyan;

    private global::LlsfMsgs.Team teamColor_;
    /// <summary>
    /// Team the puck belongs to
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.Team TeamColor {
      get { if ((_hasBits0 & 4) != 0) { return teamColor_; } else { return TeamColorDefaultValue; } }
      set {
        _hasBits0 |= 4;
        teamColor_ = value;
      }
    }
    /// <summary>Gets whether the "team_color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasTeamColor {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "team_color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearTeamColor() {
      _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "base_color" field.</summary>
    public const int BaseColorFieldNumber = 5;
    private readonly static global::LlsfMsgs.BaseColor BaseColorDefaultValue = global::LlsfMsgs.BaseColor.BaseUncolored;

    private global::LlsfMsgs.BaseColor baseColor_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.BaseColor BaseColor {
      get { if ((_hasBits0 & 8) != 0) { return baseColor_; } else { return BaseColorDefaultValue; } }
      set {
        _hasBits0 |= 8;
        baseColor_ = value;
      }
    }
    /// <summary>Gets whether the "base_color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasBaseColor {
      get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "base_color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearBaseColor() {
      _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "ring_colors" field.</summary>
    public const int RingColorsFieldNumber = 6;
    private static readonly pb::FieldCodec<global::LlsfMsgs.RingColor> _repeated_ringColors_codec
        = pb::FieldCodec.ForEnum(48, x => (int) x, x => (global::LlsfMsgs.RingColor) x);
    private readonly pbc::RepeatedField<global::LlsfMsgs.RingColor> ringColors_ = new pbc::RepeatedField<global::LlsfMsgs.RingColor>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::LlsfMsgs.RingColor> RingColors {
      get { return ringColors_; }
    }

    /// <summary>Field number for the "cap_color" field.</summary>
    public const int CapColorFieldNumber = 7;
    private readonly static global::LlsfMsgs.CapColor CapColorDefaultValue = global::LlsfMsgs.CapColor.CapBlack;

    private global::LlsfMsgs.CapColor capColor_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.CapColor CapColor {
      get { if ((_hasBits0 & 16) != 0) { return capColor_; } else { return CapColorDefaultValue; } }
      set {
        _hasBits0 |= 16;
        capColor_ = value;
      }
    }
    /// <summary>Gets whether the "cap_color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasCapColor {
      get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "cap_color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearCapColor() {
      _hasBits0 &= ~16;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Workpiece);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Workpiece other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (AtMachine != other.AtMachine) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Visible, other.Visible)) return false;
      if (TeamColor != other.TeamColor) return false;
      if (BaseColor != other.BaseColor) return false;
      if(!ringColors_.Equals(other.ringColors_)) return false;
      if (CapColor != other.CapColor) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasId) hash ^= Id.GetHashCode();
      if (HasAtMachine) hash ^= AtMachine.GetHashCode();
      if (HasVisible) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Visible);
      if (HasTeamColor) hash ^= TeamColor.GetHashCode();
      if (HasBaseColor) hash ^= BaseColor.GetHashCode();
      hash ^= ringColors_.GetHashCode();
      if (HasCapColor) hash ^= CapColor.GetHashCode();
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
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteUInt32(Id);
      }
      if (HasAtMachine) {
        output.WriteRawTag(18);
        output.WriteString(AtMachine);
      }
      if (HasVisible) {
        output.WriteRawTag(29);
        output.WriteFloat(Visible);
      }
      if (HasTeamColor) {
        output.WriteRawTag(32);
        output.WriteEnum((int) TeamColor);
      }
      if (HasBaseColor) {
        output.WriteRawTag(40);
        output.WriteEnum((int) BaseColor);
      }
      ringColors_.WriteTo(output, _repeated_ringColors_codec);
      if (HasCapColor) {
        output.WriteRawTag(56);
        output.WriteEnum((int) CapColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteUInt32(Id);
      }
      if (HasAtMachine) {
        output.WriteRawTag(18);
        output.WriteString(AtMachine);
      }
      if (HasVisible) {
        output.WriteRawTag(29);
        output.WriteFloat(Visible);
      }
      if (HasTeamColor) {
        output.WriteRawTag(32);
        output.WriteEnum((int) TeamColor);
      }
      if (HasBaseColor) {
        output.WriteRawTag(40);
        output.WriteEnum((int) BaseColor);
      }
      ringColors_.WriteTo(ref output, _repeated_ringColors_codec);
      if (HasCapColor) {
        output.WriteRawTag(56);
        output.WriteEnum((int) CapColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasId) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Id);
      }
      if (HasAtMachine) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AtMachine);
      }
      if (HasVisible) {
        size += 1 + 4;
      }
      if (HasTeamColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) TeamColor);
      }
      if (HasBaseColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) BaseColor);
      }
      size += ringColors_.CalculateSize(_repeated_ringColors_codec);
      if (HasCapColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) CapColor);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Workpiece other) {
      if (other == null) {
        return;
      }
      if (other.HasId) {
        Id = other.Id;
      }
      if (other.HasAtMachine) {
        AtMachine = other.AtMachine;
      }
      if (other.HasVisible) {
        Visible = other.Visible;
      }
      if (other.HasTeamColor) {
        TeamColor = other.TeamColor;
      }
      if (other.HasBaseColor) {
        BaseColor = other.BaseColor;
      }
      ringColors_.Add(other.ringColors_);
      if (other.HasCapColor) {
        CapColor = other.CapColor;
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
          case 8: {
            Id = input.ReadUInt32();
            break;
          }
          case 18: {
            AtMachine = input.ReadString();
            break;
          }
          case 29: {
            Visible = input.ReadFloat();
            break;
          }
          case 32: {
            TeamColor = (global::LlsfMsgs.Team) input.ReadEnum();
            break;
          }
          case 40: {
            BaseColor = (global::LlsfMsgs.BaseColor) input.ReadEnum();
            break;
          }
          case 50:
          case 48: {
            ringColors_.AddEntriesFrom(input, _repeated_ringColors_codec);
            break;
          }
          case 56: {
            CapColor = (global::LlsfMsgs.CapColor) input.ReadEnum();
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
          case 8: {
            Id = input.ReadUInt32();
            break;
          }
          case 18: {
            AtMachine = input.ReadString();
            break;
          }
          case 29: {
            Visible = input.ReadFloat();
            break;
          }
          case 32: {
            TeamColor = (global::LlsfMsgs.Team) input.ReadEnum();
            break;
          }
          case 40: {
            BaseColor = (global::LlsfMsgs.BaseColor) input.ReadEnum();
            break;
          }
          case 50:
          case 48: {
            ringColors_.AddEntriesFrom(ref input, _repeated_ringColors_codec);
            break;
          }
          case 56: {
            CapColor = (global::LlsfMsgs.CapColor) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the Workpiece message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 2000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 55,
      }

    }
    #endregion

  }

  public sealed partial class WorkpieceInfo : pb::IMessage<WorkpieceInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<WorkpieceInfo> _parser = new pb::MessageParser<WorkpieceInfo>(() => new WorkpieceInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<WorkpieceInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.WorkpieceInfoReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceInfo(WorkpieceInfo other) : this() {
      workpieces_ = other.workpieces_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceInfo Clone() {
      return new WorkpieceInfo(this);
    }

    /// <summary>Field number for the "workpieces" field.</summary>
    public const int WorkpiecesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::LlsfMsgs.Workpiece> _repeated_workpieces_codec
        = pb::FieldCodec.ForMessage(10, global::LlsfMsgs.Workpiece.Parser);
    private readonly pbc::RepeatedField<global::LlsfMsgs.Workpiece> workpieces_ = new pbc::RepeatedField<global::LlsfMsgs.Workpiece>();
    /// <summary>
    /// List of all known pucks
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::LlsfMsgs.Workpiece> Workpieces {
      get { return workpieces_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as WorkpieceInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(WorkpieceInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!workpieces_.Equals(other.workpieces_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= workpieces_.GetHashCode();
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
      workpieces_.WriteTo(output, _repeated_workpieces_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      workpieces_.WriteTo(ref output, _repeated_workpieces_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += workpieces_.CalculateSize(_repeated_workpieces_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(WorkpieceInfo other) {
      if (other == null) {
        return;
      }
      workpieces_.Add(other.workpieces_);
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
            workpieces_.AddEntriesFrom(input, _repeated_workpieces_codec);
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
            workpieces_.AddEntriesFrom(ref input, _repeated_workpieces_codec);
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the WorkpieceInfo message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 2000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 56,
      }

    }
    #endregion

  }

  public sealed partial class WorkpieceAddRing : pb::IMessage<WorkpieceAddRing>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<WorkpieceAddRing> _parser = new pb::MessageParser<WorkpieceAddRing>(() => new WorkpieceAddRing());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<WorkpieceAddRing> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.WorkpieceInfoReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceAddRing() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceAddRing(WorkpieceAddRing other) : this() {
      _hasBits0 = other._hasBits0;
      id_ = other.id_;
      ringColor_ = other.ringColor_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceAddRing Clone() {
      return new WorkpieceAddRing(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private readonly static uint IdDefaultValue = 0;

    private uint id_;
    /// <summary>
    /// Puck unique ID
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Id {
      get { if ((_hasBits0 & 1) != 0) { return id_; } else { return IdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        id_ = value;
      }
    }
    /// <summary>Gets whether the "id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "ring_color" field.</summary>
    public const int RingColorFieldNumber = 2;
    private readonly static global::LlsfMsgs.RingColor RingColorDefaultValue = global::LlsfMsgs.RingColor.RingBlue;

    private global::LlsfMsgs.RingColor ringColor_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.RingColor RingColor {
      get { if ((_hasBits0 & 2) != 0) { return ringColor_; } else { return RingColorDefaultValue; } }
      set {
        _hasBits0 |= 2;
        ringColor_ = value;
      }
    }
    /// <summary>Gets whether the "ring_color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasRingColor {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "ring_color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearRingColor() {
      _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as WorkpieceAddRing);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(WorkpieceAddRing other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (RingColor != other.RingColor) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasId) hash ^= Id.GetHashCode();
      if (HasRingColor) hash ^= RingColor.GetHashCode();
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
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteUInt32(Id);
      }
      if (HasRingColor) {
        output.WriteRawTag(16);
        output.WriteEnum((int) RingColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteUInt32(Id);
      }
      if (HasRingColor) {
        output.WriteRawTag(16);
        output.WriteEnum((int) RingColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasId) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Id);
      }
      if (HasRingColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) RingColor);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(WorkpieceAddRing other) {
      if (other == null) {
        return;
      }
      if (other.HasId) {
        Id = other.Id;
      }
      if (other.HasRingColor) {
        RingColor = other.RingColor;
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
          case 8: {
            Id = input.ReadUInt32();
            break;
          }
          case 16: {
            RingColor = (global::LlsfMsgs.RingColor) input.ReadEnum();
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
          case 8: {
            Id = input.ReadUInt32();
            break;
          }
          case 16: {
            RingColor = (global::LlsfMsgs.RingColor) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the WorkpieceAddRing message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 2000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 57,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
