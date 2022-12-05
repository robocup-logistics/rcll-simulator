// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BeaconSignal.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from BeaconSignal.proto</summary>
  public static partial class BeaconSignalReflection {

    #region Descriptor
    /// <summary>File descriptor for BeaconSignal.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BeaconSignalReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJCZWFjb25TaWduYWwucHJvdG8SCWxsc2ZfbXNncxoKVGltZS5wcm90bxoK",
            "VGVhbS5wcm90bxoMUG9zZTJELnByb3RvGg9BZ2VudFRhc2sucHJvdG8iswIK",
            "DEJlYWNvblNpZ25hbBIdCgR0aW1lGAEgAigLMg8ubGxzZl9tc2dzLlRpbWUS",
            "CwoDc2VxGAIgAigEEg4KBm51bWJlchgDIAIoDRIRCgl0ZWFtX25hbWUYBCAC",
            "KAkSEQoJcGVlcl9uYW1lGAUgAigJEiMKCnRlYW1fY29sb3IYBiABKA4yDy5s",
            "bHNmX21zZ3MuVGVhbRIfCgRwb3NlGAcgASgLMhEubGxzZl9tc2dzLlBvc2Uy",
            "RBIiCgR0YXNrGAkgASgLMhQubGxzZl9tc2dzLkFnZW50VGFzaxIvCg5maW5p",
            "c2hlZF90YXNrcxgKIAMoCzIXLmxsc2ZfbXNncy5GaW5pc2hlZFRhc2siJgoI",
            "Q29tcFR5cGUSDAoHQ09NUF9JRBDQDxIMCghNU0dfVFlQRRABIjIKDEZpbmlz",
            "aGVkVGFzaxIOCgZUYXNrSWQYASACKA0SEgoKc3VjY2Vzc2Z1bBgCIAIoCEI1",
            "Ch9vcmcucm9ib2N1cF9sb2dpc3RpY3MubGxzZl9tc2dzQhJCZWFjb25TaWdu",
            "YWxQcm90b3M="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Llsfmsgs.TimeReflection.Descriptor, global::LlsfMsgs.TeamReflection.Descriptor, global::LlsfMsgs.Pose2DReflection.Descriptor, global::LlsfMsgs.AgentTaskReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.BeaconSignal), global::LlsfMsgs.BeaconSignal.Parser, new[]{ "Time", "Seq", "Number", "TeamName", "PeerName", "TeamColor", "Pose", "Task", "FinishedTasks" }, null, new[]{ typeof(global::LlsfMsgs.BeaconSignal.Types.CompType) }, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.FinishedTask), global::LlsfMsgs.FinishedTask.Parser, new[]{ "TaskId", "Successful" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BeaconSignal : pb::IMessage<BeaconSignal>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<BeaconSignal> _parser = new pb::MessageParser<BeaconSignal>(() => new BeaconSignal());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BeaconSignal> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.BeaconSignalReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BeaconSignal() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BeaconSignal(BeaconSignal other) : this() {
      _hasBits0 = other._hasBits0;
      time_ = other.time_ != null ? other.time_.Clone() : null;
      seq_ = other.seq_;
      number_ = other.number_;
      teamName_ = other.teamName_;
      peerName_ = other.peerName_;
      teamColor_ = other.teamColor_;
      pose_ = other.pose_ != null ? other.pose_.Clone() : null;
      task_ = other.task_ != null ? other.task_.Clone() : null;
      finishedTasks_ = other.finishedTasks_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BeaconSignal Clone() {
      return new BeaconSignal(this);
    }

    /// <summary>Field number for the "time" field.</summary>
    public const int TimeFieldNumber = 1;
    private global::Llsfmsgs.Time time_;
    /// <summary>
    /// Local time in UTC
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Llsfmsgs.Time Time {
      get { return time_; }
      set {
        time_ = value;
      }
    }

    /// <summary>Field number for the "seq" field.</summary>
    public const int SeqFieldNumber = 2;
    private readonly static ulong SeqDefaultValue = 0UL;

    private ulong seq_;
    /// <summary>
    /// Sequence number
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Seq {
      get { if ((_hasBits0 & 1) != 0) { return seq_; } else { return SeqDefaultValue; } }
      set {
        _hasBits0 |= 1;
        seq_ = value;
      }
    }
    /// <summary>Gets whether the "seq" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasSeq {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "seq" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearSeq() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "number" field.</summary>
    public const int NumberFieldNumber = 3;
    private readonly static uint NumberDefaultValue = 0;

    private uint number_;
    /// <summary>
    /// The robot's jersey number
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Number {
      get { if ((_hasBits0 & 2) != 0) { return number_; } else { return NumberDefaultValue; } }
      set {
        _hasBits0 |= 2;
        number_ = value;
      }
    }
    /// <summary>Gets whether the "number" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasNumber {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "number" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearNumber() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "team_name" field.</summary>
    public const int TeamNameFieldNumber = 4;
    private readonly static string TeamNameDefaultValue = "";

    private string teamName_;
    /// <summary>
    /// The robot's team name
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string TeamName {
      get { return teamName_ ?? TeamNameDefaultValue; }
      set {
        teamName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "team_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasTeamName {
      get { return teamName_ != null; }
    }
    /// <summary>Clears the value of the "team_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearTeamName() {
      teamName_ = null;
    }

    /// <summary>Field number for the "peer_name" field.</summary>
    public const int PeerNameFieldNumber = 5;
    private readonly static string PeerNameDefaultValue = "";

    private string peerName_;
    /// <summary>
    /// The robot's name
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string PeerName {
      get { return peerName_ ?? PeerNameDefaultValue; }
      set {
        peerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "peer_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasPeerName {
      get { return peerName_ != null; }
    }
    /// <summary>Clears the value of the "peer_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearPeerName() {
      peerName_ = null;
    }

    /// <summary>Field number for the "team_color" field.</summary>
    public const int TeamColorFieldNumber = 6;
    private readonly static global::LlsfMsgs.Team TeamColorDefaultValue = global::LlsfMsgs.Team.Cyan;

    private global::LlsfMsgs.Team teamColor_;
    /// <summary>
    /// Team color, teams MUST sent this
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

    /// <summary>Field number for the "pose" field.</summary>
    public const int PoseFieldNumber = 7;
    private global::LlsfMsgs.Pose2D pose_;
    /// <summary>
    /// Position and orientation of the
    /// robot on the LLSF playing field
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.Pose2D Pose {
      get { return pose_; }
      set {
        pose_ = value;
      }
    }

    /// <summary>Field number for the "task" field.</summary>
    public const int TaskFieldNumber = 9;
    private global::LlsfMsgs.AgentTask task_;
    /// <summary>
    /// Maybe needed for future generalisation of beacon signal and tracking of robots current activity (used by GRIPS currently)
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.AgentTask Task {
      get { return task_; }
      set {
        task_ = value;
      }
    }

    /// <summary>Field number for the "finished_tasks" field.</summary>
    public const int FinishedTasksFieldNumber = 10;
    private static readonly pb::FieldCodec<global::LlsfMsgs.FinishedTask> _repeated_finishedTasks_codec
        = pb::FieldCodec.ForMessage(82, global::LlsfMsgs.FinishedTask.Parser);
    private readonly pbc::RepeatedField<global::LlsfMsgs.FinishedTask> finishedTasks_ = new pbc::RepeatedField<global::LlsfMsgs.FinishedTask>();
    /// <summary>
    /// a list of all the tasks that the robot has done. Contains the ID of the task and the result
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::LlsfMsgs.FinishedTask> FinishedTasks {
      get { return finishedTasks_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BeaconSignal);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BeaconSignal other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Time, other.Time)) return false;
      if (Seq != other.Seq) return false;
      if (Number != other.Number) return false;
      if (TeamName != other.TeamName) return false;
      if (PeerName != other.PeerName) return false;
      if (TeamColor != other.TeamColor) return false;
      if (!object.Equals(Pose, other.Pose)) return false;
      if (!object.Equals(Task, other.Task)) return false;
      if(!finishedTasks_.Equals(other.finishedTasks_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (time_ != null) hash ^= Time.GetHashCode();
      if (HasSeq) hash ^= Seq.GetHashCode();
      if (HasNumber) hash ^= Number.GetHashCode();
      if (HasTeamName) hash ^= TeamName.GetHashCode();
      if (HasPeerName) hash ^= PeerName.GetHashCode();
      if (HasTeamColor) hash ^= TeamColor.GetHashCode();
      if (pose_ != null) hash ^= Pose.GetHashCode();
      if (task_ != null) hash ^= Task.GetHashCode();
      hash ^= finishedTasks_.GetHashCode();
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
      if (time_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Time);
      }
      if (HasSeq) {
        output.WriteRawTag(16);
        output.WriteUInt64(Seq);
      }
      if (HasNumber) {
        output.WriteRawTag(24);
        output.WriteUInt32(Number);
      }
      if (HasTeamName) {
        output.WriteRawTag(34);
        output.WriteString(TeamName);
      }
      if (HasPeerName) {
        output.WriteRawTag(42);
        output.WriteString(PeerName);
      }
      if (HasTeamColor) {
        output.WriteRawTag(48);
        output.WriteEnum((int) TeamColor);
      }
      if (pose_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(Pose);
      }
      if (task_ != null) {
        output.WriteRawTag(74);
        output.WriteMessage(Task);
      }
      finishedTasks_.WriteTo(output, _repeated_finishedTasks_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (time_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Time);
      }
      if (HasSeq) {
        output.WriteRawTag(16);
        output.WriteUInt64(Seq);
      }
      if (HasTeamName) {
        output.WriteRawTag(34);
        output.WriteString(TeamName);
      }
      if (HasPeerName) {
        output.WriteRawTag(42);
        output.WriteString(PeerName);
      }
      if (HasTeamColor) {
        output.WriteRawTag(48);
        output.WriteEnum((int) TeamColor);
      }
      if (pose_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(Pose);
      }
      if (HasNumber) {
        output.WriteRawTag(64);
        output.WriteUInt32(Number);
      }
      if (task_ != null) {
        output.WriteRawTag(74);
        output.WriteMessage(Task);
      }
      finishedTasks_.WriteTo(ref output, _repeated_finishedTasks_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (time_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Time);
      }
      if (HasSeq) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Seq);
      }
      if (HasNumber) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Number);
      }
      if (HasTeamName) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(TeamName);
      }
      if (HasPeerName) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PeerName);
      }
      if (HasTeamColor) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) TeamColor);
      }
      if (pose_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Pose);
      }
      if (task_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Task);
      }
      size += finishedTasks_.CalculateSize(_repeated_finishedTasks_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BeaconSignal other) {
      if (other == null) {
        return;
      }
      if (other.time_ != null) {
        if (time_ == null) {
          Time = new global::Llsfmsgs.Time();
        }
        Time.MergeFrom(other.Time);
      }
      if (other.HasSeq) {
        Seq = other.Seq;
      }
      if (other.HasNumber) {
        Number = other.Number;
      }
      if (other.HasTeamName) {
        TeamName = other.TeamName;
      }
      if (other.HasPeerName) {
        PeerName = other.PeerName;
      }
      if (other.HasTeamColor) {
        TeamColor = other.TeamColor;
      }
      if (other.pose_ != null) {
        if (pose_ == null) {
          Pose = new global::LlsfMsgs.Pose2D();
        }
        Pose.MergeFrom(other.Pose);
      }
      if (other.task_ != null) {
        if (task_ == null) {
          Task = new global::LlsfMsgs.AgentTask();
        }
        Task.MergeFrom(other.Task);
      }
      finishedTasks_.Add(other.finishedTasks_);
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
            if (time_ == null) {
              Time = new global::Llsfmsgs.Time();
            }
            input.ReadMessage(Time);
            break;
          }
          case 16: {
            Seq = input.ReadUInt64();
            break;
          }
          case 24: {
            Number = input.ReadUInt32();
            break;
          }
          case 34: {
            TeamName = input.ReadString();
            break;
          }
          case 42: {
            PeerName = input.ReadString();
            break;
          }
          case 48: {
            TeamColor = (global::LlsfMsgs.Team) input.ReadEnum();
            break;
          }
          case 58: {
            if (pose_ == null) {
              Pose = new global::LlsfMsgs.Pose2D();
            }
            input.ReadMessage(Pose);
            break;
          }
          case 74: {
            if (task_ == null) {
              Task = new global::LlsfMsgs.AgentTask();
            }
            input.ReadMessage(Task);
            break;
          }
          case 82: {
            finishedTasks_.AddEntriesFrom(input, _repeated_finishedTasks_codec);
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
            if (time_ == null) {
              Time = new global::Llsfmsgs.Time();
            }
            input.ReadMessage(Time);
            break;
          }
          case 16: {
            Seq = input.ReadUInt64();
            break;
          }
          case 34: {
            TeamName = input.ReadString();
            break;
          }
          case 42: {
            PeerName = input.ReadString();
            break;
          }
          case 48: {
            TeamColor = (global::LlsfMsgs.Team) input.ReadEnum();
            break;
          }
          case 58: {
            if (pose_ == null) {
              Pose = new global::LlsfMsgs.Pose2D();
            }
            input.ReadMessage(Pose);
            break;
          }
          case 64: {
            Number = input.ReadUInt32();
            break;
          }
          case 74: {
            if (task_ == null) {
              Task = new global::LlsfMsgs.AgentTask();
            }
            input.ReadMessage(Task);
            break;
          }
          case 82: {
            finishedTasks_.AddEntriesFrom(ref input, _repeated_finishedTasks_codec);
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the BeaconSignal message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 2000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 1,
      }

    }
    #endregion

  }

  public sealed partial class FinishedTask : pb::IMessage<FinishedTask>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<FinishedTask> _parser = new pb::MessageParser<FinishedTask>(() => new FinishedTask());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FinishedTask> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.BeaconSignalReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FinishedTask() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FinishedTask(FinishedTask other) : this() {
      _hasBits0 = other._hasBits0;
      taskId_ = other.taskId_;
      successful_ = other.successful_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FinishedTask Clone() {
      return new FinishedTask(this);
    }

    /// <summary>Field number for the "TaskId" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private readonly static uint TaskIdDefaultValue = 0;

    private uint taskId_;
    /// <summary>
    /// the specific task thats been done
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TaskId {
      get { if ((_hasBits0 & 1) != 0) { return taskId_; } else { return TaskIdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        taskId_ = value;
      }
    }
    /// <summary>Gets whether the "TaskId" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasTaskId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "TaskId" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearTaskId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "successful" field.</summary>
    public const int SuccessfulFieldNumber = 2;
    private readonly static bool SuccessfulDefaultValue = false;

    private bool successful_;
    /// <summary>
    /// the result of the task thats been done
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Successful {
      get { if ((_hasBits0 & 2) != 0) { return successful_; } else { return SuccessfulDefaultValue; } }
      set {
        _hasBits0 |= 2;
        successful_ = value;
      }
    }
    /// <summary>Gets whether the "successful" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasSuccessful {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "successful" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearSuccessful() {
      _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FinishedTask);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FinishedTask other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (TaskId != other.TaskId) return false;
      if (Successful != other.Successful) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasTaskId) hash ^= TaskId.GetHashCode();
      if (HasSuccessful) hash ^= Successful.GetHashCode();
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
      if (HasTaskId) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
      if (HasSuccessful) {
        output.WriteRawTag(16);
        output.WriteBool(Successful);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasTaskId) {
        output.WriteRawTag(8);
        output.WriteUInt32(TaskId);
      }
      if (HasSuccessful) {
        output.WriteRawTag(16);
        output.WriteBool(Successful);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasTaskId) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TaskId);
      }
      if (HasSuccessful) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FinishedTask other) {
      if (other == null) {
        return;
      }
      if (other.HasTaskId) {
        TaskId = other.TaskId;
      }
      if (other.HasSuccessful) {
        Successful = other.Successful;
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
            TaskId = input.ReadUInt32();
            break;
          }
          case 16: {
            Successful = input.ReadBool();
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
            TaskId = input.ReadUInt32();
            break;
          }
          case 16: {
            Successful = input.ReadBool();
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
