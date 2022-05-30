using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials
{
    class PBMessageHandlerRobot : PBMessageHandlerBase
    {
        private Robot Owner;
        private readonly PBMessageFactoryBase Fact;
        public PBMessageHandlerRobot(Robot owner, MyLogger log) : base(log)
        {
            Owner = owner;
            Fact = new PBMessageFactoryRobot(Owner, log);
        }
        public new bool HandleMessage(byte[] Stream)
        {

            /*      Each row is 4 bytes
             * 1.   Protocol version, Cipher, Reserved byte1 , reserved byte2
             * 2.   Payload size 
             * 3.   component ID and Message type each 2 bytes. Used to detect the Protobuff message
             * 
             * */
            if (Stream.Length < 4)
            {
                MyLogger.Log("The received Message is to short to be parsed!");
                return false;
            }
            if (FrameHeader.Version != Stream[0])
            {
                MyLogger.Log("Version is different!");
                return false;
            }
            if (FrameHeader.Cipher != Stream[1])
            {
                MyLogger.Log("Cipher is different!");
                return false;
            }
            /*if (FrameHeader.Reserved != Stream[2] && FrameHeader.Reserved2 != Stream[3])
            {
                MyLogger.Log("Reserved is different!");
            }*/
            int payloadsize = BytesToInt(Stream, 4, 4);
            //MyLogger.Log("The payload has : " + payloadsize.ToString() + " bytes!");
            int cmpId = BytesToInt(Stream, 8, 2);
            int msgtype = BytesToInt(Stream, 10, 2);
            string msg = "";
            //MyLogger.Log("The Recieved message is from component : " + cmpId.ToString() + " and the message type is = " + msgtype.ToString() + " and payloadsize = " + payloadsize);
            //MyLogger.Log("Length of the stream = " + Stream.Length);
            
            if (payloadsize == 0)
            {
                MyLogger.Log("The payload is " + payloadsize + " so we stop here!");
                return false;
            }
            switch (msgtype)
            {
                case (int)BeaconSignal.Types.CompType.MsgType:
                    {
                        switch (cmpId)
                        {
                            case (int)BeaconSignal.Types.CompType.CompId:
                                MessageParser<BeaconSignal> bsp = new(() => new BeaconSignal());
                                BeaconSignal bs = bsp.ParseFrom(Stream, 12, payloadsize - 4);
                                //owner.SetGameState(ri);
                                MyLogger.Log("Parsing of the BeaconSignal Message was successful!");
                                msg = bs.ToString();
                                break;
                            case 2003:
                                MyLogger.Log("LogMessage.proto is not yet implemented!");
                                break;
                            default:
                                MyLogger.Log("Unknown MsgType " + msgtype + " for component " + cmpId);
                                break;
                        }

                        break;
                    }
                case (int)Machine.Types.CompType.MsgType:
                    {
                        MessageParser<Machine> mp = new(() => new Machine());
                        Machine m = mp.ParseFrom(Stream, 12, payloadsize - 4);
                        MyLogger.Log("Parsing of the VersionInfo Message was successful!");
                        msg = m.ToString();
                        break;
                    }
                case (int)VersionInfo.Types.CompType.MsgType: // VERSION INFO 
                    {
                        MessageParser<VersionInfo> vp = new(() => new VersionInfo());
                        VersionInfo vi = vp.ParseFrom(Stream, 12, payloadsize - 4);
                        MyLogger.Log("Parsing of the VersionInfo Message was successful!");
                        msg = vi.ToString();
                        break;
                    }
                case (int)MachineInfo.Types.CompType.MsgType:
                    {
                        if ((int)MachineInfo.Types.CompType.CompId != cmpId)
                        {
                            MyLogger.Log("Parsing of MachineInfo Message was aborted due to wrong CMP id!" + (int)MachineInfo.Types.CompType.CompId + "!=" + cmpId);
                            return false;
                        }
                        //MyLogger.Log("In front of parsing the MachineInfo!");
                        MessageParser<MachineInfo> mip = new(() => new MachineInfo());
                        MachineInfo mi;
                        string str = Encoding.Default.GetString(Stream);
                        //MyLogger.Log("String = " + str);
                        MyLogger.Log("Parsing of MachineInfo Message! cmpId = " + cmpId + " msg type = " + msgtype);
                        try
                        {
                            mi = mip.ParseFrom(Stream, 12, payloadsize - 4);
                        }
                        catch (Exception e)
                        {
                            MyLogger.Log(e.ToString());
                            return false;
                        }

                        MyLogger.Log("Parsing of MachineInfo Message was successful!");
                        msg = mi.ToString();
                        MyLogger.Log("The Parsed message = " + msg);
                        
                        break;
                    }
                case (int)GameState.Types.CompType.MsgType:
                    {
                        MessageParser<GameState> gsp = new(() => new GameState());
                        GameState gs = gsp.ParseFrom(Stream, 12, payloadsize - 4);
                        Timer.GetInstance().UpdateTime(gs.GameTime);
                        if (gs.HasPointsCyan)
                            Configurations.GetInstance().Teams[0].Points = gs.PointsCyan;
                        if (gs.HasPointsMagenta)
                            Configurations.GetInstance().Teams[0].Points = gs.PointsMagenta;
                        MyLogger.Log("Parsing of GameState Message was successful!");
                        msg = gs.ToString();
                        break;
                    }
                case (int)RobotInfo.Types.CompType.MsgType:
                    {
                        MessageParser<RobotInfo> rip = new(() => new RobotInfo());
                        RobotInfo ri = rip.ParseFrom(Stream, 12, payloadsize - 4);
                        //owner.SetGameState(ri);
                        MyLogger.Log("Parsing of the RobotInfo Message was successful!");
                        msg = ri.ToString();
                        break;
                    }
                case (int)GripsMidlevelTasks.Types.CompType.MsgType:
                    {
                        MessageParser<GripsMidlevelTasks> midlevelParser =
                            new(() => new GripsMidlevelTasks());

                        GripsMidlevelTasks task = midlevelParser.ParseFrom(Stream, 12, payloadsize - 4);
                        MyLogger.Log("Parsing of the GripsMidLevelTasks was successful!");
                        if (Owner != null)
                        {
                            Owner.SetGripsTasks(task);
                        }
                        else
                        {
                            MyLogger.Log("Not a Robot so the GripsMidLevelTasks Message was ignored!");
                        }
                        msg = task.ToString();
                        break;
                    }
                case (int)GripsPrepareMachine.Types.CompType.MsgType:
                    {
                        MessageParser<GripsPrepareMachine> prepareParser =
                            new(() => new GripsPrepareMachine());

                        GripsPrepareMachine prepareTask = prepareParser.ParseFrom(Stream, 12, payloadsize - 4);
                        MyLogger.Log("Parsing of PrepareMachineTask was successful");
                        MyLogger.Log(prepareTask.ToString());
                        break;
                    }
                case (int)AttentionMessage.Types.CompType.MsgType:
                    MessageParser<AttentionMessage> attentionParser =
                        new(() => new AttentionMessage());
                    AttentionMessage attention = attentionParser.ParseFrom(Stream, 12, payloadsize - 4);
                    MyLogger.Log("Parsing of Attention Message was successful!");
                    msg = attention.ToString();

                    break;
                default:
                    {
                        MyLogger.Log("Unknown MsgType " + msgtype + " for component " + cmpId);
                        break;
                    }
            }
            MyLogger.Log("Parsed message = " + msg);
            return true;
        }

    }
}
