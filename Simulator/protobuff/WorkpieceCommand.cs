// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: WorkpieceCommand.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace GazsimMsgs {

  /// <summary>Holder for reflection information generated from WorkpieceCommand.proto</summary>
  public static partial class WorkpieceCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for WorkpieceCommand.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static WorkpieceCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChZXb3JrcGllY2VDb21tYW5kLnByb3RvEgtnYXpzaW1fbXNncyKWAQoQV29y",
            "a3BpZWNlQ29tbWFuZBIlCgdjb21tYW5kGAEgAigOMhQuZ2F6c2ltX21zZ3Mu",
            "Q29tbWFuZBIhCgVjb2xvchgCIAMoDjISLmdhenNpbV9tc2dzLkNvbG9yEhEK",
            "CXB1Y2tfbmFtZRgDIAIoCRIlCgp0ZWFtX2NvbG9yGAQgASgOMhEuZ2F6c2lt",
            "X21zZ3MuVGVhbSJHCg9Xb3JrcGllY2VSZXN1bHQSIQoFY29sb3IYASACKA4y",
            "Ei5nYXpzaW1fbXNncy5Db2xvchIRCglwdWNrX25hbWUYAiACKAkqaAoFQ29s",
            "b3ISBwoDUkVEEAASCQoFR1JFRU4QARIICgRCTFVFEAISCAoER1JFWRADEgkK",
            "BUJMQUNLEAQSCgoGWUVMTE9XEAUSCgoGT1JBTkdFEAYSCgoGU0lMVkVSEAcS",
            "CAoETk9ORRAIKkEKB0NvbW1hbmQSDAoIQUREX1JJTkcQABILCgdBRERfQ0FQ",
            "EAESDgoKUkVNT1ZFX0NBUBACEgsKB0RFTElWRVIQAyodCgRUZWFtEggKBENZ",
            "QU4QABILCgdNQUdFTlRBEAE="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::GazsimMsgs.Color), typeof(global::GazsimMsgs.Command), typeof(global::GazsimMsgs.Team), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::GazsimMsgs.WorkpieceCommand), global::GazsimMsgs.WorkpieceCommand.Parser, new[]{ "Command", "Color", "PuckName", "TeamColor" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::GazsimMsgs.WorkpieceResult), global::GazsimMsgs.WorkpieceResult.Parser, new[]{ "Color", "PuckName" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  /// The possible colors of the ring
  /// </summary>
  public enum Color {
    [pbr::OriginalName("RED")] Red = 0,
    [pbr::OriginalName("GREEN")] Green = 1,
    [pbr::OriginalName("BLUE")] Blue = 2,
    [pbr::OriginalName("GREY")] Grey = 3,
    [pbr::OriginalName("BLACK")] Black = 4,
    [pbr::OriginalName("YELLOW")] Yellow = 5,
    [pbr::OriginalName("ORANGE")] Orange = 6,
    [pbr::OriginalName("SILVER")] Silver = 7,
    [pbr::OriginalName("NONE")] None = 8,
  }

  public enum Command {
    [pbr::OriginalName("ADD_RING")] AddRing = 0,
    [pbr::OriginalName("ADD_CAP")] AddCap = 1,
    [pbr::OriginalName("REMOVE_CAP")] RemoveCap = 2,
    [pbr::OriginalName("DELIVER")] Deliver = 3,
  }

  public enum Team {
    [pbr::OriginalName("CYAN")] Cyan = 0,
    [pbr::OriginalName("MAGENTA")] Magenta = 1,
  }

  #endregion

  #region Messages
  /// <summary>
  /// Time stamp and duration structure.
  /// Can be used for absolute times or
  /// durations alike.
  /// </summary>
  public sealed partial class WorkpieceCommand : pb::IMessage<WorkpieceCommand> {
    private static readonly pb::MessageParser<WorkpieceCommand> _parser = new pb::MessageParser<WorkpieceCommand>(() => new WorkpieceCommand());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<WorkpieceCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::GazsimMsgs.WorkpieceCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceCommand(WorkpieceCommand other) : this() {
      _hasBits0 = other._hasBits0;
      command_ = other.command_;
      color_ = other.color_.Clone();
      puckName_ = other.puckName_;
      teamColor_ = other.teamColor_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceCommand Clone() {
      return new WorkpieceCommand(this);
    }

    /// <summary>Field number for the "command" field.</summary>
    public const int CommandFieldNumber = 1;
    private readonly static global::GazsimMsgs.Command CommandDefaultValue = global::GazsimMsgs.Command.AddRing;

    private global::GazsimMsgs.Command command_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::GazsimMsgs.Command Command {
      get { if ((_hasBits0 & 1) != 0) { return command_; } else { return CommandDefaultValue; } }
      set {
        _hasBits0 |= 1;
        command_ = value;
      }
    }
    /// <summary>Gets whether the "command" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasCommand {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "command" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearCommand() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "color" field.</summary>
    public const int ColorFieldNumber = 2;
    private static readonly pb::FieldCodec<global::GazsimMsgs.Color> _repeated_color_codec
        = pb::FieldCodec.ForEnum(16, x => (int) x, x => (global::GazsimMsgs.Color) x);
    private readonly pbc::RepeatedField<global::GazsimMsgs.Color> color_ = new pbc::RepeatedField<global::GazsimMsgs.Color>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::GazsimMsgs.Color> Color {
      get { return color_; }
    }

    /// <summary>Field number for the "puck_name" field.</summary>
    public const int PuckNameFieldNumber = 3;
    private readonly static string PuckNameDefaultValue = "";

    private string puckName_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string PuckName {
      get { return puckName_ ?? PuckNameDefaultValue; }
      set {
        puckName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "puck_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasPuckName {
      get { return puckName_ != null; }
    }
    /// <summary>Clears the value of the "puck_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearPuckName() {
      puckName_ = null;
    }

    /// <summary>Field number for the "team_color" field.</summary>
    public const int TeamColorFieldNumber = 4;
    private readonly static global::GazsimMsgs.Team TeamColorDefaultValue = global::GazsimMsgs.Team.Cyan;

    private global::GazsimMsgs.Team teamColor_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::GazsimMsgs.Team TeamColor {
      get { if ((_hasBits0 & 2) != 0) { return teamColor_; } else { return TeamColorDefaultValue; } }
      set {
        _hasBits0 |= 2;
        teamColor_ = value;
      }
    }
    /// <summary>Gets whether the "team_color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasTeamColor {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "team_color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearTeamColor() {
      _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as WorkpieceCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(WorkpieceCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Command != other.Command) return false;
      if(!color_.Equals(other.color_)) return false;
      if (PuckName != other.PuckName) return false;
      if (TeamColor != other.TeamColor) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasCommand) hash ^= Command.GetHashCode();
      hash ^= color_.GetHashCode();
      if (HasPuckName) hash ^= PuckName.GetHashCode();
      if (HasTeamColor) hash ^= TeamColor.GetHashCode();
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
      if (HasCommand) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Command);
      }
      color_.WriteTo(output, _repeated_color_codec);
      if (HasPuckName) {
        output.WriteRawTag(26);
        output.WriteString(PuckName);
      }
      if (HasTeamColor) {
        output.WriteRawTag(32);
        output.WriteEnum((int) TeamColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasCommand) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Command);
      }
      size += color_.CalculateSize(_repeated_color_codec);
      if (HasPuckName) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PuckName);
      }
      if (HasTeamColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) TeamColor);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(WorkpieceCommand other) {
      if (other == null) {
        return;
      }
      if (other.HasCommand) {
        Command = other.Command;
      }
      color_.Add(other.color_);
      if (other.HasPuckName) {
        PuckName = other.PuckName;
      }
      if (other.HasTeamColor) {
        TeamColor = other.TeamColor;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Command = (global::GazsimMsgs.Command) input.ReadEnum();
            break;
          }
          case 18:
          case 16: {
            color_.AddEntriesFrom(input, _repeated_color_codec);
            break;
          }
          case 26: {
            PuckName = input.ReadString();
            break;
          }
          case 32: {
            TeamColor = (global::GazsimMsgs.Team) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  public sealed partial class WorkpieceResult : pb::IMessage<WorkpieceResult> {
    private static readonly pb::MessageParser<WorkpieceResult> _parser = new pb::MessageParser<WorkpieceResult>(() => new WorkpieceResult());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<WorkpieceResult> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::GazsimMsgs.WorkpieceCommandReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceResult() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceResult(WorkpieceResult other) : this() {
      _hasBits0 = other._hasBits0;
      color_ = other.color_;
      puckName_ = other.puckName_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public WorkpieceResult Clone() {
      return new WorkpieceResult(this);
    }

    /// <summary>Field number for the "color" field.</summary>
    public const int ColorFieldNumber = 1;
    private readonly static global::GazsimMsgs.Color ColorDefaultValue = global::GazsimMsgs.Color.Red;

    private global::GazsimMsgs.Color color_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::GazsimMsgs.Color Color {
      get { if ((_hasBits0 & 1) != 0) { return color_; } else { return ColorDefaultValue; } }
      set {
        _hasBits0 |= 1;
        color_ = value;
      }
    }
    /// <summary>Gets whether the "color" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasColor {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "color" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearColor() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "puck_name" field.</summary>
    public const int PuckNameFieldNumber = 2;
    private readonly static string PuckNameDefaultValue = "";

    private string puckName_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string PuckName {
      get { return puckName_ ?? PuckNameDefaultValue; }
      set {
        puckName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "puck_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasPuckName {
      get { return puckName_ != null; }
    }
    /// <summary>Clears the value of the "puck_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearPuckName() {
      puckName_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as WorkpieceResult);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(WorkpieceResult other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Color != other.Color) return false;
      if (PuckName != other.PuckName) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasColor) hash ^= Color.GetHashCode();
      if (HasPuckName) hash ^= PuckName.GetHashCode();
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
      if (HasColor) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Color);
      }
      if (HasPuckName) {
        output.WriteRawTag(18);
        output.WriteString(PuckName);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Color);
      }
      if (HasPuckName) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PuckName);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(WorkpieceResult other) {
      if (other == null) {
        return;
      }
      if (other.HasColor) {
        Color = other.Color;
      }
      if (other.HasPuckName) {
        PuckName = other.PuckName;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Color = (global::GazsimMsgs.Color) input.ReadEnum();
            break;
          }
          case 18: {
            PuckName = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
