using System;
using System.Net;
using System.Collections.Generic;
using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;
using Team = LlsfMsgs.Team;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    class PBMessageFactoryBase {
        public ulong SequenzNr;
        public Timer? Timer;
        public readonly MyLogger MyLogger;
        public readonly Configurations Config;
        public enum MessageTypes {
            BeaconSignal, // Sending periodcally to detect refbox and robots
            RobotInfo,
            ReportAllMachines,
            MachineOrientation,
            PrsTask,
            PrepareMachine,
            SimulationTime,
            GripsPrepareMachine,
            GripsBeaconSignal,
            AgentTask,
            SimSynchTime,
            ExplorationInfo, //send from the refbox to the robot
            GameState, // send from the refbox to the robot
            MachineInfo, // Get send from the refbox to the robot in production to update information for the robot
            OrderInfo, // gets periodically sent from the refbox to update the current open orders
            MachineReport //Sending message to the Refbox
        }



        public PBMessageFactoryBase(Configurations config, MyLogger log) {
            log.Info("Created a PBMessageFactoryBase!");
            SequenzNr = 0;
            MyLogger = log;
            Timer = null;
            Config = config;
        }

        public virtual byte[] CreateMessage(MessageTypes mtype) {
            Timer ??= Timer.GetInstance(Config);
            ushort cmp = 0;
            ushort msg = 0;
            uint payloadsize = 0;
            byte[] bytes;
            var time = new Time();
            // Insert the Time for Message-Types Beacon,Robot,Machine
            if (mtype is MessageTypes.RobotInfo or MessageTypes.MachineReport) {
                time.Nsec = Timer.Nsec;
                time.Sec = Timer.Sec;
            }
            MyLogger.Log("[BaseMessageFactory] Creating a : " + mtype + " message!");
            switch (mtype) {
                case MessageTypes.BeaconSignal: {
                        var Signal = new BeaconSignal() {
                            Number = 0,
                            PeerName = "Test"
                        };

                        cmp = (ushort)BeaconSignal.Types.CompType.CompId;
                        msg = (ushort)BeaconSignal.Types.CompType.MsgType;
                        payloadsize = (uint)Signal.CalculateSize() + 4;
                        bytes = Signal.ToByteArray();
                        break;
                    }

                case MessageTypes.SimSynchTime:
                    var simtime = new SimTimeSync {
                        Paused = Timer.Paused,
                        RealTimeFactor = Timer.TimeFactor,
                        SimTime = GetTimeMessage()
                    };
                    cmp = (ushort)SimTimeSync.Types.CompType.CompId;
                    msg = (ushort)SimTimeSync.Types.CompType.MsgType;
                    payloadsize = (uint)simtime.CalculateSize() + 4;
                    bytes = simtime.ToByteArray();
                    // MyLogger.Log("Created message = " + simtime.ToString());
                    //Configurations.GetInstance().Time;
                    break;
                case MessageTypes.GameState:
                    var gamestate = new GameState() {
                        GameTime = GetTimeMessage(),
                        Phase = GameState.Types.Phase.Exploration,
                        PointsCyan = 0,
                        PointsMagenta = 0,
                        State = GameState.Types.State.Init,
                        TeamCyan = Config.Teams[0].Name,
                        TeamMagenta = Config.Teams[1].Name,
                    };
                    cmp = (ushort)GameState.Types.CompType.CompId;
                    msg = (ushort)GameState.Types.CompType.MsgType;
                    payloadsize = (uint)gamestate.CalculateSize() + 4;
                    bytes = gamestate.ToByteArray();
                    break;
                default:
                    return Array.Empty<byte>();
            }
            var fh = new FrameHeader(payloadsize);
            var mh = new MessageHeader(cmp, msg);
            var mb = new MessageBody(bytes);
            var message = new Message(fh, mh, mb);
            return message.GetBytes();
        }


        public Time GetTimeMessage() {
            Timer ??= Timer.GetInstance(Config);
            return Timer.GetTime();
        }

    }

    class Message {
        readonly byte[] Bytes;
        FrameHeader framehead;
        MessageHeader messagehead;
        MessageBody messagebody;

        public Message(FrameHeader fh, MessageHeader mh, MessageBody mb) {
            framehead = fh;
            messagehead = mh;
            messagebody = mb;
            var length = fh.GetBytes().Length + mh.GetBytes().Length + mb.GetBytes().Length;
            Bytes = new byte[length];
            Buffer.BlockCopy(framehead.GetBytes(), 0, Bytes, 0, framehead.Length);
            Buffer.BlockCopy(messagehead.GetBytes(), 0, Bytes, framehead.Length, messagehead.Length);
            Buffer.BlockCopy(messagebody.GetBytes(), 0, Bytes, framehead.Length + messagehead.Length, messagebody.Length);
            //PrintBytes(Bytes);
        }

        public byte[] GetBytes() {
            return Bytes;
        }

        private void PrintBytes(IEnumerable<byte> toPrint) {
            Console.WriteLine("Printing Bytes = ");
            foreach (var b in toPrint) {
                Console.Write("\\x{0}", b.ToString().PadLeft(2, '0'));
            }
            Console.WriteLine("");
        }

    }

    class FrameHeader {
        public static byte Version = 2;
        public static byte Cipher = 0;
        public static byte Reserved = 0;
        public static byte Reserved2 = 0;
        byte[] Bytes;
        public int Length;
        public FrameHeader(uint payload) {
            Bytes = new byte[8];
            Bytes[0] = Version;
            Bytes[1] = Cipher;
            Bytes[2] = Reserved;
            Bytes[3] = Reserved2;
            var payloadbytes = GetBytesfrom32(payload);
            Bytes[4] = payloadbytes[0];
            Bytes[5] = payloadbytes[1];
            Bytes[6] = payloadbytes[2];
            Bytes[7] = payloadbytes[3];
            Length = Bytes.Length;
        }
        public byte[] GetBytes() {
            return Bytes;
        }
        public byte[] GetBytesfrom32(uint val) {
            var netorder = IPAddress.HostToNetworkOrder(checked((int)val));
            var res = BitConverter.GetBytes(netorder);
            return res;
        }
    }

    class MessageBody {
        byte[] Bytes;
        public int Length;
        public MessageBody(byte[] bytearray) {
            Bytes = bytearray;
            Length = Bytes.Length;
        }
        public byte[] GetBytes() {
            return Bytes;
        }
    }

    class MessageHeader {
        byte[] Bytes;
        public int Length;
        public MessageHeader(ushort cmp, ushort msg) {
            Bytes = new byte[4];
            var cmpBytes = GetBytesfrom16(cmp);
            var msgBytes = GetBytesfrom16(msg);
            Bytes[0] = cmpBytes[0];
            Bytes[1] = cmpBytes[1];
            Bytes[2] = msgBytes[0];
            Bytes[3] = msgBytes[1];
            Length = Bytes.Length;
        }
        public byte[] GetBytesfrom16(ushort val) {
            var netorder = IPAddress.HostToNetworkOrder(checked((short)val));
            var res = BitConverter.GetBytes(netorder);
            return res;
        }
        public byte[] GetBytes() {
            return Bytes;
        }
    }
}
