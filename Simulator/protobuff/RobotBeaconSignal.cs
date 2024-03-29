// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: RobotBeaconSignal.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from RobotBeaconSignal.proto</summary>
  public static partial class RobotBeaconSignalReflection {

    #region Descriptor
    /// <summary>File descriptor for RobotBeaconSignal.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RobotBeaconSignalReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdSb2JvdEJlYWNvblNpZ25hbC5wcm90bxIJbGxzZl9tc2dzGhJCZWFjb25T",
            "aWduYWwucHJvdG8itgEKEVJvYm90QmVhY29uU2lnbmFsEi0KDGJlYWNvblNp",
            "Z25hbBgBIAIoCzIXLmxsc2ZfbXNncy5CZWFjb25TaWduYWwSDwoHdGFza19p",
            "ZBgCIAEoBRIPCgdydW5uaW5nGAMgAigIEhIKCm9wcnNfc3RhY2sYBCABKAkS",
            "EwoLaG9sZFByb2R1Y3QYBSABKAgiJwoIQ29tcFR5cGUSDAoHQ09NUF9JRBCO",
            "JxINCghNU0dfVFlQRRC+BUI6Ch9vcmcucm9ib2N1cF9sb2dpc3RpY3MubGxz",
            "Zl9tc2dzQhdSb2JvdEJlYWNvblNpZ25hbFByb3Rvcw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::LlsfMsgs.BeaconSignalReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.RobotBeaconSignal), global::LlsfMsgs.RobotBeaconSignal.Parser, new[]{ "BeaconSignal", "TaskId", "Running", "OprsStack", "HoldProduct" }, null, new[]{ typeof(global::LlsfMsgs.RobotBeaconSignal.Types.CompType) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class RobotBeaconSignal : pb::IMessage<RobotBeaconSignal> {
    private static readonly pb::MessageParser<RobotBeaconSignal> _parser = new pb::MessageParser<RobotBeaconSignal>(() => new RobotBeaconSignal());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RobotBeaconSignal> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.RobotBeaconSignalReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RobotBeaconSignal() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RobotBeaconSignal(RobotBeaconSignal other) : this() {
      _hasBits0 = other._hasBits0;
      beaconSignal_ = other.beaconSignal_ != null ? other.beaconSignal_.Clone() : null;
      taskId_ = other.taskId_;
      running_ = other.running_;
      oprsStack_ = other.oprsStack_;
      holdProduct_ = other.holdProduct_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RobotBeaconSignal Clone() {
      return new RobotBeaconSignal(this);
    }

    /// <summary>Field number for the "beaconSignal" field.</summary>
    public const int BeaconSignalFieldNumber = 1;
    private global::LlsfMsgs.BeaconSignal beaconSignal_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::LlsfMsgs.BeaconSignal BeaconSignal {
      get { return beaconSignal_; }
      set {
        beaconSignal_ = value;
      }
    }

    /// <summary>Field number for the "task_id" field.</summary>
    public const int TaskIdFieldNumber = 2;
    private readonly static int TaskIdDefaultValue = 0;

    private int taskId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int TaskId {
      get { if ((_hasBits0 & 1) != 0) { return taskId_; } else { return TaskIdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        taskId_ = value;
      }
    }
    /// <summary>Gets whether the "task_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasTaskId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "task_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearTaskId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "running" field.</summary>
    public const int RunningFieldNumber = 3;
    private readonly static bool RunningDefaultValue = false;

    private bool running_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Running {
      get { if ((_hasBits0 & 2) != 0) { return running_; } else { return RunningDefaultValue; } }
      set {
        _hasBits0 |= 2;
        running_ = value;
      }
    }
    /// <summary>Gets whether the "running" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasRunning {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "running" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearRunning() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "oprs_stack" field.</summary>
    public const int OprsStackFieldNumber = 4;
    private readonly static string OprsStackDefaultValue = "";

    private string oprsStack_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string OprsStack {
      get { return oprsStack_ ?? OprsStackDefaultValue; }
      set {
        oprsStack_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "oprs_stack" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasOprsStack {
      get { return oprsStack_ != null; }
    }
    /// <summary>Clears the value of the "oprs_stack" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearOprsStack() {
      oprsStack_ = null;
    }

    /// <summary>Field number for the "holdProduct" field.</summary>
    public const int HoldProductFieldNumber = 5;
    private readonly static bool HoldProductDefaultValue = false;

    private bool holdProduct_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HoldProduct {
      get { if ((_hasBits0 & 4) != 0) { return holdProduct_; } else { return HoldProductDefaultValue; } }
      set {
        _hasBits0 |= 4;
        holdProduct_ = value;
      }
    }
    /// <summary>Gets whether the "holdProduct" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasHoldProduct {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "holdProduct" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearHoldProduct() {
      _hasBits0 &= ~4;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RobotBeaconSignal);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RobotBeaconSignal other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(BeaconSignal, other.BeaconSignal)) return false;
      if (TaskId != other.TaskId) return false;
      if (Running != other.Running) return false;
      if (OprsStack != other.OprsStack) return false;
      if (HoldProduct != other.HoldProduct) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (beaconSignal_ != null) hash ^= BeaconSignal.GetHashCode();
      if (HasTaskId) hash ^= TaskId.GetHashCode();
      if (HasRunning) hash ^= Running.GetHashCode();
      if (HasOprsStack) hash ^= OprsStack.GetHashCode();
      if (HasHoldProduct) hash ^= HoldProduct.GetHashCode();
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
      if (beaconSignal_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(BeaconSignal);
      }
      if (HasTaskId) {
        output.WriteRawTag(16);
        output.WriteInt32(TaskId);
      }
      if (HasRunning) {
        output.WriteRawTag(24);
        output.WriteBool(Running);
      }
      if (HasOprsStack) {
        output.WriteRawTag(34);
        output.WriteString(OprsStack);
      }
      if (HasHoldProduct) {
        output.WriteRawTag(40);
        output.WriteBool(HoldProduct);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (beaconSignal_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(BeaconSignal);
      }
      if (HasTaskId) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(TaskId);
      }
      if (HasRunning) {
        size += 1 + 1;
      }
      if (HasOprsStack) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(OprsStack);
      }
      if (HasHoldProduct) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RobotBeaconSignal other) {
      if (other == null) {
        return;
      }
      if (other.beaconSignal_ != null) {
        if (beaconSignal_ == null) {
          BeaconSignal = new global::LlsfMsgs.BeaconSignal();
        }
        BeaconSignal.MergeFrom(other.BeaconSignal);
      }
      if (other.HasTaskId) {
        TaskId = other.TaskId;
      }
      if (other.HasRunning) {
        Running = other.Running;
      }
      if (other.HasOprsStack) {
        OprsStack = other.OprsStack;
      }
      if (other.HasHoldProduct) {
        HoldProduct = other.HoldProduct;
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
          case 10: {
            if (beaconSignal_ == null) {
              BeaconSignal = new global::LlsfMsgs.BeaconSignal();
            }
            input.ReadMessage(BeaconSignal);
            break;
          }
          case 16: {
            TaskId = input.ReadInt32();
            break;
          }
          case 24: {
            Running = input.ReadBool();
            break;
          }
          case 34: {
            OprsStack = input.ReadString();
            break;
          }
          case 40: {
            HoldProduct = input.ReadBool();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the RobotBeaconSignal message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 5006,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 702,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
