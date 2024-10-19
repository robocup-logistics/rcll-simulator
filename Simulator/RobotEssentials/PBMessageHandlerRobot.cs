using System.Text;
using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    class PBMessageHandlerRobot : PBMessageHandlerBase {
        private Robot Owner;
        private readonly PBMessageFactoryBase Fact;
        public PBMessageHandlerRobot(Configurations config, Robot owner, MyLogger log) : base(config, log) {
            Owner = owner;
            Fact = new PBMessageFactoryRobot(Config, Owner, log);
        }

        protected override bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {

            string msg = "";

            switch (messageType) {
                case (int)BeaconSignal.Types.CompType.MsgType: {
                        switch (componentId) {
                            case (int)BeaconSignal.Types.CompType.CompId:
                                MessageParser<BeaconSignal> bsp = new(() => new BeaconSignal());
                                BeaconSignal bs = bsp.ParseFrom(stream, 12, payloadSize - 4);
                                //owner.SetGameState(ri);
                                MyLogger.Log("Parsing of the BeaconSignal Message was successful!");
                                msg = bs.ToString();
                                break;
                            case 2003:
                                MyLogger.Log("LogMessage.proto is not yet implemented!");
                                break;
                            default:
                                MyLogger.Log("Unknown MsgType " + messageType + " for component " + componentId);
                                break;
                        }

                        break;
                    }
                case (int)Machine.Types.CompType.MsgType: {
                        MessageParser<Machine> mp = new(() => new Machine());
                        Machine m = mp.ParseFrom(stream, 12, payloadSize - 4);
                        MyLogger.Log("Parsing of the VersionInfo Message was successful!");
                        msg = m.ToString();
                        break;
                    }
                case (int)VersionInfo.Types.CompType.MsgType: // VERSION INFO 
                    {
                        MessageParser<VersionInfo> vp = new(() => new VersionInfo());
                        VersionInfo vi = vp.ParseFrom(stream, 12, payloadSize - 4);
                        MyLogger.Log("Parsing of the VersionInfo Message was successful!");
                        msg = vi.ToString();
                        break;
                    }
                case (int)MachineInfo.Types.CompType.MsgType: {
                        if ((int)MachineInfo.Types.CompType.CompId != componentId) {
                            MyLogger.Log("Parsing of MachineInfo Message was aborted due to wrong CMP id!" +
                                         (int)MachineInfo.Types.CompType.CompId + "!=" + componentId);
                            return false;
                        }
                        //MyLogger.Log("In front of parsing the MachineInfo!");
                        MessageParser<MachineInfo> mip = new(() => new MachineInfo());
                        MachineInfo mi;
                        string str = Encoding.Default.GetString(stream);
                        //MyLogger.Log("String = " + str);
                        MyLogger.Log("[RobotHandler] Parsing of MachineInfo Message! cmpId = " + componentId + " msg type = " + messageType);
                        try {
                            mi = mip.ParseFrom(stream, 12, payloadSize - 4);
                        }
                        catch (Exception e) {
                            MyLogger.Log(e.ToString());
                            return false;
                        }
                        MyLogger.Log("Parsing of MachineInfo Message was successful!");
                        msg = mi.ToString();
                        MyLogger.Log("The Parsed message = " + msg);

                        break;
                    }
                case (int)GameState.Types.CompType.MsgType: {
                        MessageParser<GameState> gsp = new(() => new GameState());
                        GameState gs = gsp.ParseFrom(stream, 12, payloadSize - 4);
                        Timer.GetInstance(Config).UpdateTime(gs.GameTime);
                        if (gs.HasPointsCyan)
                            Config.Teams[0].Points = gs.PointsCyan;
                        if (gs.HasPointsMagenta)
                            Config.Teams[0].Points = gs.PointsMagenta;
                        MyLogger.Log("Parsing of GameState Message was successful!");
                        msg = gs.ToString();
                        break;
                    }
                case (int)RobotInfo.Types.CompType.MsgType: {
                        MessageParser<RobotInfo> rip = new(() => new RobotInfo());
                        RobotInfo ri = rip.ParseFrom(stream, 12, payloadSize - 4);
                        //owner.SetGameState(ri);
                        MyLogger.Log("Parsing of the RobotInfo Message was successful!");
                        msg = ri.ToString();
                        break;
                    }
                case (int)AgentTask.Types.CompType.MsgType: {
                        MessageParser<AgentTask> taskParser =
                            new(() => new AgentTask());

                        AgentTask task = taskParser.ParseFrom(stream, 12, payloadSize - 4);
                        MyLogger.Log("Parsing of the GripsMidLevelTasks was successful!");
                        if (Owner != null) {
                            Owner.SetAgentTasks(task);
                        }
                        else {
                            MyLogger.Log("Not a Robot so the GripsMidLevelTasks Message was ignored!");
                        }
                        msg = task.ToString();
                        break;
                    }
                case (int)AttentionMessage.Types.CompType.MsgType:
                    MessageParser<AttentionMessage> attentionParser =
                        new(() => new AttentionMessage());
                    AttentionMessage attention = attentionParser.ParseFrom(stream, 12, payloadSize - 4);
                    MyLogger.Log("Parsing of Attention Message was successful!");
                    msg = attention.ToString();

                    break;
                default: {
                        MyLogger.Log("Unknown MsgType " + messageType + " for component " + componentId);
                        break;
                    }
            }
            MyLogger.Log("Parsed message = " + msg);
            return true;
        }

    }
}
