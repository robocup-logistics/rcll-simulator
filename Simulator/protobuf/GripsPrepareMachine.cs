// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: GripsPrepareMachine.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from GripsPrepareMachine.proto</summary>
  public static partial class GripsPrepareMachineReflection {

    #region Descriptor
    /// <summary>File descriptor for GripsPrepareMachine.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GripsPrepareMachineReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChlHcmlwc1ByZXBhcmVNYWNoaW5lLnByb3RvEglsbHNmX21zZ3MilQEKE0dy",
            "aXBzUHJlcGFyZU1hY2hpbmUSEAoIcm9ib3RfaWQYASACKA0SEgoKbWFjaGlu",
            "ZV9pZBgCIAIoCRIVCg1tYWNoaW5lX3BvaW50GAMgAigJEhgKEG1hY2hpbmVf",
            "cHJlcGFyZWQYBCACKAgiJwoIQ29tcFR5cGUSDAoHQ09NUF9JRBCIJxINCghN",
            "U0dfVFlQRRD3A0I8Ch9vcmcucm9ib2N1cF9sb2dpc3RpY3MubGxzZl9tc2dz",
            "QhlHcmlwc1ByZXBhcmVNYWNoaW5lUHJvdG9z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::LlsfMsgs.GripsPrepareMachine), global::LlsfMsgs.GripsPrepareMachine.Parser, new[]{ "RobotId", "MachineId", "MachinePoint", "MachinePrepared" }, null, new[]{ typeof(global::LlsfMsgs.GripsPrepareMachine.Types.CompType) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// Instructs the Teamserver to prepare the machine
  /// </summary>
  public sealed partial class GripsPrepareMachine : pb::IMessage<GripsPrepareMachine> {
    private static readonly pb::MessageParser<GripsPrepareMachine> _parser = new pb::MessageParser<GripsPrepareMachine>(() => new GripsPrepareMachine());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GripsPrepareMachine> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::LlsfMsgs.GripsPrepareMachineReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GripsPrepareMachine() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GripsPrepareMachine(GripsPrepareMachine other) : this() {
      _hasBits0 = other._hasBits0;
      robotId_ = other.robotId_;
      machineId_ = other.machineId_;
      machinePoint_ = other.machinePoint_;
      machinePrepared_ = other.machinePrepared_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GripsPrepareMachine Clone() {
      return new GripsPrepareMachine(this);
    }

    /// <summary>Field number for the "robot_id" field.</summary>
    public const int RobotIdFieldNumber = 1;
    private readonly static uint RobotIdDefaultValue = 0;

    private uint robotId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint RobotId {
      get { if ((_hasBits0 & 1) != 0) { return robotId_; } else { return RobotIdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        robotId_ = value;
      }
    }
    /// <summary>Gets whether the "robot_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasRobotId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "robot_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearRobotId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "machine_id" field.</summary>
    public const int MachineIdFieldNumber = 2;
    private readonly static string MachineIdDefaultValue = "";

    private string machineId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MachineId {
      get { return machineId_ ?? MachineIdDefaultValue; }
      set {
        machineId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "machine_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasMachineId {
      get { return machineId_ != null; }
    }
    /// <summary>Clears the value of the "machine_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearMachineId() {
      machineId_ = null;
    }

    /// <summary>Field number for the "machine_point" field.</summary>
    public const int MachinePointFieldNumber = 3;
    private readonly static string MachinePointDefaultValue = "";

    private string machinePoint_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MachinePoint {
      get { return machinePoint_ ?? MachinePointDefaultValue; }
      set {
        machinePoint_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "machine_point" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasMachinePoint {
      get { return machinePoint_ != null; }
    }
    /// <summary>Clears the value of the "machine_point" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearMachinePoint() {
      machinePoint_ = null;
    }

    /// <summary>Field number for the "machine_prepared" field.</summary>
    public const int MachinePreparedFieldNumber = 4;
    private readonly static bool MachinePreparedDefaultValue = false;

    private bool machinePrepared_;
    /// <summary>
    /// true if machine successfully prepared from teamserver
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool MachinePrepared {
      get { if ((_hasBits0 & 2) != 0) { return machinePrepared_; } else { return MachinePreparedDefaultValue; } }
      set {
        _hasBits0 |= 2;
        machinePrepared_ = value;
      }
    }
    /// <summary>Gets whether the "machine_prepared" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasMachinePrepared {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "machine_prepared" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearMachinePrepared() {
      _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GripsPrepareMachine);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GripsPrepareMachine other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (RobotId != other.RobotId) return false;
      if (MachineId != other.MachineId) return false;
      if (MachinePoint != other.MachinePoint) return false;
      if (MachinePrepared != other.MachinePrepared) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasRobotId) hash ^= RobotId.GetHashCode();
      if (HasMachineId) hash ^= MachineId.GetHashCode();
      if (HasMachinePoint) hash ^= MachinePoint.GetHashCode();
      if (HasMachinePrepared) hash ^= MachinePrepared.GetHashCode();
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
      if (HasRobotId) {
        output.WriteRawTag(8);
        output.WriteUInt32(RobotId);
      }
      if (HasMachineId) {
        output.WriteRawTag(18);
        output.WriteString(MachineId);
      }
      if (HasMachinePoint) {
        output.WriteRawTag(26);
        output.WriteString(MachinePoint);
      }
      if (HasMachinePrepared) {
        output.WriteRawTag(32);
        output.WriteBool(MachinePrepared);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasRobotId) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(RobotId);
      }
      if (HasMachineId) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MachineId);
      }
      if (HasMachinePoint) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MachinePoint);
      }
      if (HasMachinePrepared) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GripsPrepareMachine other) {
      if (other == null) {
        return;
      }
      if (other.HasRobotId) {
        RobotId = other.RobotId;
      }
      if (other.HasMachineId) {
        MachineId = other.MachineId;
      }
      if (other.HasMachinePoint) {
        MachinePoint = other.MachinePoint;
      }
      if (other.HasMachinePrepared) {
        MachinePrepared = other.MachinePrepared;
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
            RobotId = input.ReadUInt32();
            break;
          }
          case 18: {
            MachineId = input.ReadString();
            break;
          }
          case 26: {
            MachinePoint = input.ReadString();
            break;
          }
          case 32: {
            MachinePrepared = input.ReadBool();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the GripsPrepareMachine message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 5000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 503,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code