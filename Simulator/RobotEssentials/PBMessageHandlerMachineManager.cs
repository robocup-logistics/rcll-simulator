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
    class PbMessageHandlerMachineManager : PBMessageHandlerBase
    {
        private readonly MpsManager Manager;
        public PbMessageHandlerMachineManager(MyLogger log) : base(log)
        {
            Manager = MpsManager.GetInstance();
        }

        public new bool HandleMessage(byte[] stream)
        {

            /*      Each row is 4 bytes
             * 1.   Protocol version, Cipher, Reserved byte1 , reserved byte2
             * 2.   Payload size 
             * 3.   component ID and Message type each 2 bytes. Used to detect the Protobuff message
             * 
             * */
            if (stream.Length < 4)
            {
                MyLogger.Log("The received Message is to short to be parsed!");
                return false;
            }
            if (FrameHeader.Version != stream[0])
            {
                MyLogger.Log("Version is different!");
                return false;
            }
            if (FrameHeader.Cipher != stream[1])
            {
                MyLogger.Log("Cipher is different!");
                return false;
            }
            /*if (FrameHeader.Reserved != Stream[2] && FrameHeader.Reserved2 != Stream[3])
            {
                MyLogger.Log("Reserved is different!");
            }*/
            int payloadsize = BytesToInt(stream, 4, 4);
            //MyLogger.Log("The payload has : " + payloadsize.ToString() + " bytes!");
            int cmpId = BytesToInt(stream, 8, 2);
            int msgtype = BytesToInt(stream, 10, 2);
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
                case (int)Machine.Types.CompType.MsgType:
                    {
                        MessageParser<Machine> mp = new(() => new Machine());
                        Machine m = mp.ParseFrom(stream, 12, payloadsize - 4);
                        MyLogger.Log("Parsing of the MachineInfo Message was successful!");
                        msg = m.ToString();
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
                        MyLogger.Log("Parsing of MachineInfo Message! cmpId = " + cmpId + " msg type = " + msgtype);
                        try
                        {
                            mi = mip.ParseFrom(stream, 12, payloadsize - 4);
                        }
                        catch (Exception e)
                        {
                            MyLogger.Log(e.ToString());
                            return false;
                        }

                        MyLogger.Log("Parsing of MachineInfo Message was successful!");
                        msg = mi.ToString();
                        MyLogger.Log("The Parsed message = " + msg);
                        if (mi.Machines.Count < MpsManager.GetInstance().Machines.Count)
                        {
                            MyLogger.Log("MachineInfo is not containing all machines!");
                            return false;
                        }
                        ZonesManager.GetInstance().ZoneManagerMutex.WaitOne();
                        if (Manager.AllMachineSet)
                        {
                            //MyLogger.Log("All machines already placed! (after parsed)");
                            ZonesManager.GetInstance().ZoneManagerMutex.ReleaseMutex();
                            return true;
                        }
                        Manager.PlaceMachines(mi);
                        ZonesManager.GetInstance().ZoneManagerMutex.ReleaseMutex();
                        break;
                    }
                case (int)GameState.Types.CompType.MsgType:
                    {
                        MessageParser<GameState> gsp = new(() => new GameState());
                        GameState gs = gsp.ParseFrom(stream, 12, payloadsize - 4);
                        Timer.GetInstance().UpdateTime(gs.GameTime);
                        if (gs.HasPointsCyan)
                            Configurations.GetInstance().Teams[0].Points = gs.PointsCyan;
                        if (gs.HasPointsMagenta)
                            Configurations.GetInstance().Teams[0].Points = gs.PointsMagenta;
                        MyLogger.Log("Parsing of GameState Message was successful!");
                        msg = gs.ToString();
                        break;
                    }
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
