// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: MachineDescription.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from MachineDescription.proto</summary>
  public static partial class MachineDescriptionReflection {

    #region Descriptor
    /// <summary>File descriptor for MachineDescription.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MachineDescriptionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChhNYWNoaW5lRGVzY3JpcHRpb24ucHJvdG8SCWxsc2ZfbXNncyosCgpMaWdo",
            "dENvbG9yEgcKA1JFRBAAEgoKBllFTExPVxABEgkKBUdSRUVOEAIqKAoKTGln",
            "aHRTdGF0ZRIHCgNPRkYQABIGCgJPThABEgkKBUJMSU5LEAIqMAoEU1NPcBIJ",
            "CgVTVE9SRRABEgwKCFJFVFJJRVZFEAISDwoLQ0hBTkdFX0lORk8QAyonCgRD",
            "U09wEhAKDFJFVFJJRVZFX0NBUBABEg0KCU1PVU5UX0NBUBACQjsKH29yZy5y",
            "b2JvY3VwX2xvZ2lzdGljcy5sbHNmX21zZ3NCGE1hY2hpbmVEZXNjcmlwdGlv",
            "blByb3Rvcw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::LlsfMsgs.LightColor), typeof(global::LlsfMsgs.LightState), typeof(global::LlsfMsgs.SSOp), typeof(global::LlsfMsgs.CSOp), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum LightColor {
    [pbr::OriginalName("RED")] Red = 0,
    [pbr::OriginalName("YELLOW")] Yellow = 1,
    [pbr::OriginalName("GREEN")] Green = 2,
  }

  public enum LightState {
    [pbr::OriginalName("OFF")] Off = 0,
    [pbr::OriginalName("ON")] On = 1,
    [pbr::OriginalName("BLINK")] Blink = 2,
  }

  public enum SSOp {
    [pbr::OriginalName("STORE")] Store = 1,
    [pbr::OriginalName("RETRIEVE")] Retrieve = 2,
    [pbr::OriginalName("CHANGE_INFO")] ChangeInfo = 3,
  }

  public enum CSOp {
    [pbr::OriginalName("RETRIEVE_CAP")] RetrieveCap = 1,
    [pbr::OriginalName("MOUNT_CAP")] MountCap = 2,
  }

  #endregion

}

#endregion Designer generated code
