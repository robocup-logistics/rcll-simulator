// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ProductColor.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace LlsfMsgs {

  /// <summary>Holder for reflection information generated from ProductColor.proto</summary>
  public static partial class ProductColorReflection {

    #region Descriptor
    /// <summary>File descriptor for ProductColor.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProductColorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJQcm9kdWN0Q29sb3IucHJvdG8SCWxsc2ZfbXNncypMCglSaW5nQ29sb3IS",
            "DQoJUklOR19CTFVFEAESDgoKUklOR19HUkVFThACEg8KC1JJTkdfT1JBTkdF",
            "EAMSDwoLUklOR19ZRUxMT1cQBCo6CglCYXNlQ29sb3ISDAoIQkFTRV9SRUQQ",
            "ARIOCgpCQVNFX0JMQUNLEAISDwoLQkFTRV9TSUxWRVIQAyonCghDYXBDb2xv",
            "chINCglDQVBfQkxBQ0sQARIMCghDQVBfR1JFWRACQjUKH29yZy5yb2JvY3Vw",
            "X2xvZ2lzdGljcy5sbHNmX21zZ3NCElByb2R1Y3RDb2xvclByb3Rvcw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::LlsfMsgs.RingColor), typeof(global::LlsfMsgs.BaseColor), typeof(global::LlsfMsgs.CapColor), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum RingColor {
    [pbr::OriginalName("RING_BLUE")] RingBlue = 1,
    [pbr::OriginalName("RING_GREEN")] RingGreen = 2,
    [pbr::OriginalName("RING_ORANGE")] RingOrange = 3,
    [pbr::OriginalName("RING_YELLOW")] RingYellow = 4,
  }

  public enum BaseColor {
    [pbr::OriginalName("BASE_RED")] BaseRed = 1,
    [pbr::OriginalName("BASE_BLACK")] BaseBlack = 2,
    [pbr::OriginalName("BASE_SILVER")] BaseSilver = 3,
  }

  public enum CapColor {
    [pbr::OriginalName("CAP_BLACK")] CapBlack = 1,
    [pbr::OriginalName("CAP_GREY")] CapGrey = 2,
  }

  #endregion

}

#endregion Designer generated code