// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: MachineOrientationStateUpdate.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MachineStates {

  /// <summary>Holder for reflection information generated from MachineOrientationStateUpdate.proto</summary>
  public static partial class MachineOrientationStateUpdateReflection {

    #region Descriptor
    /// <summary>File descriptor for MachineOrientationStateUpdate.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MachineOrientationStateUpdateReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiNNYWNoaW5lT3JpZW50YXRpb25TdGF0ZVVwZGF0ZS5wcm90bxINbWFjaGlu",
            "ZVN0YXRlcyK+AQoXTWFjaGluZU9yaWVudGF0aW9uU3RhdGUSEAoIcm9ib3Rf",
            "aWQYASACKA0SFQoNZm91bmRfbWFjaGluZRgCIAEoCBISCgptYWNoaW5lX2lk",
            "GAMgASgJEhMKC3pvbmVfbnVtYmVyGAQgASgNEhMKC3pvbmVfcHJlZml4GAUg",
            "ASgJEhMKC29yaWVudGF0aW9uGAYgASgBIicKCENvbXBUeXBlEgwKB0NPTVBf",
            "SUQQ8C4SDQoITVNHX1RZUEUQyQFCRgofb3JnLnJvYm9jdXBfbG9naXN0aWNz",
            "Lmxsc2ZfbXNnc0IjTWFjaGluZU9yaWVudGF0aW9uU3RhdGVVcGRhdGVQcm90",
            "b3M="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::MachineStates.MachineOrientationState), global::MachineStates.MachineOrientationState.Parser, new[]{ "RobotId", "FoundMachine", "MachineId", "ZoneNumber", "ZonePrefix", "Orientation" }, null, new[]{ typeof(global::MachineStates.MachineOrientationState.Types.CompType) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// [START messages]
  /// </summary>
  public sealed partial class MachineOrientationState : pb::IMessage<MachineOrientationState> {
    private static readonly pb::MessageParser<MachineOrientationState> _parser = new pb::MessageParser<MachineOrientationState>(() => new MachineOrientationState());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MachineOrientationState> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::MachineStates.MachineOrientationStateUpdateReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MachineOrientationState() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MachineOrientationState(MachineOrientationState other) : this() {
      _hasBits0 = other._hasBits0;
      robotId_ = other.robotId_;
      foundMachine_ = other.foundMachine_;
      machineId_ = other.machineId_;
      zoneNumber_ = other.zoneNumber_;
      zonePrefix_ = other.zonePrefix_;
      orientation_ = other.orientation_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MachineOrientationState Clone() {
      return new MachineOrientationState(this);
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

    /// <summary>Field number for the "found_machine" field.</summary>
    public const int FoundMachineFieldNumber = 2;
    private readonly static bool FoundMachineDefaultValue = false;

    private bool foundMachine_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool FoundMachine {
      get { if ((_hasBits0 & 2) != 0) { return foundMachine_; } else { return FoundMachineDefaultValue; } }
      set {
        _hasBits0 |= 2;
        foundMachine_ = value;
      }
    }
    /// <summary>Gets whether the "found_machine" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasFoundMachine {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "found_machine" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearFoundMachine() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "machine_id" field.</summary>
    public const int MachineIdFieldNumber = 3;
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

    /// <summary>Field number for the "zone_number" field.</summary>
    public const int ZoneNumberFieldNumber = 4;
    private readonly static uint ZoneNumberDefaultValue = 0;

    private uint zoneNumber_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ZoneNumber {
      get { if ((_hasBits0 & 4) != 0) { return zoneNumber_; } else { return ZoneNumberDefaultValue; } }
      set {
        _hasBits0 |= 4;
        zoneNumber_ = value;
      }
    }
    /// <summary>Gets whether the "zone_number" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasZoneNumber {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "zone_number" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearZoneNumber() {
      _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "zone_prefix" field.</summary>
    public const int ZonePrefixFieldNumber = 5;
    private readonly static string ZonePrefixDefaultValue = "";

    private string zonePrefix_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ZonePrefix {
      get { return zonePrefix_ ?? ZonePrefixDefaultValue; }
      set {
        zonePrefix_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "zone_prefix" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasZonePrefix {
      get { return zonePrefix_ != null; }
    }
    /// <summary>Clears the value of the "zone_prefix" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearZonePrefix() {
      zonePrefix_ = null;
    }

    /// <summary>Field number for the "orientation" field.</summary>
    public const int OrientationFieldNumber = 6;
    private readonly static double OrientationDefaultValue = 0D;

    private double orientation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Orientation {
      get { if ((_hasBits0 & 8) != 0) { return orientation_; } else { return OrientationDefaultValue; } }
      set {
        _hasBits0 |= 8;
        orientation_ = value;
      }
    }
    /// <summary>Gets whether the "orientation" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasOrientation {
      get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "orientation" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearOrientation() {
      _hasBits0 &= ~8;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MachineOrientationState);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MachineOrientationState other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (RobotId != other.RobotId) return false;
      if (FoundMachine != other.FoundMachine) return false;
      if (MachineId != other.MachineId) return false;
      if (ZoneNumber != other.ZoneNumber) return false;
      if (ZonePrefix != other.ZonePrefix) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Orientation, other.Orientation)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasRobotId) hash ^= RobotId.GetHashCode();
      if (HasFoundMachine) hash ^= FoundMachine.GetHashCode();
      if (HasMachineId) hash ^= MachineId.GetHashCode();
      if (HasZoneNumber) hash ^= ZoneNumber.GetHashCode();
      if (HasZonePrefix) hash ^= ZonePrefix.GetHashCode();
      if (HasOrientation) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Orientation);
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
      if (HasFoundMachine) {
        output.WriteRawTag(16);
        output.WriteBool(FoundMachine);
      }
      if (HasMachineId) {
        output.WriteRawTag(26);
        output.WriteString(MachineId);
      }
      if (HasZoneNumber) {
        output.WriteRawTag(32);
        output.WriteUInt32(ZoneNumber);
      }
      if (HasZonePrefix) {
        output.WriteRawTag(42);
        output.WriteString(ZonePrefix);
      }
      if (HasOrientation) {
        output.WriteRawTag(49);
        output.WriteDouble(Orientation);
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
      if (HasFoundMachine) {
        size += 1 + 1;
      }
      if (HasMachineId) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MachineId);
      }
      if (HasZoneNumber) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ZoneNumber);
      }
      if (HasZonePrefix) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ZonePrefix);
      }
      if (HasOrientation) {
        size += 1 + 8;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MachineOrientationState other) {
      if (other == null) {
        return;
      }
      if (other.HasRobotId) {
        RobotId = other.RobotId;
      }
      if (other.HasFoundMachine) {
        FoundMachine = other.FoundMachine;
      }
      if (other.HasMachineId) {
        MachineId = other.MachineId;
      }
      if (other.HasZoneNumber) {
        ZoneNumber = other.ZoneNumber;
      }
      if (other.HasZonePrefix) {
        ZonePrefix = other.ZonePrefix;
      }
      if (other.HasOrientation) {
        Orientation = other.Orientation;
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
          case 16: {
            FoundMachine = input.ReadBool();
            break;
          }
          case 26: {
            MachineId = input.ReadString();
            break;
          }
          case 32: {
            ZoneNumber = input.ReadUInt32();
            break;
          }
          case 42: {
            ZonePrefix = input.ReadString();
            break;
          }
          case 49: {
            Orientation = input.ReadDouble();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the MachineOrientationState message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum CompType {
        [pbr::OriginalName("COMP_ID")] CompId = 6000,
        [pbr::OriginalName("MSG_TYPE")] MsgType = 201,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
