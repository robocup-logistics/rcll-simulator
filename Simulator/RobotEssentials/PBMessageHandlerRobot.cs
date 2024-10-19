using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    class PBMessageHandlerRobot : PBMessageHandlerBase {
        private Robot Owner;
        public PBMessageHandlerRobot(Configurations config, Robot owner, MyLogger log) : base(config, log) {
            Owner = owner;
        }

        protected override bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {

            string msg = "";

            switch (messageType) {
                case (int)RobotInfo.Types.CompType.MsgType:
                    //TODO MAINTANCE
                    MessageParser<RobotInfo> rip = new(() => new RobotInfo());
                    RobotInfo ri = rip.ParseFrom(stream, 12, payloadSize - 4);
                    //owner.SetGameState(ri);
                    MyLogger.Log("Parsing of the RobotInfo Message was successful!");
                    msg = ri.ToString();
                    break;
                case (int)AgentTask.Types.CompType.MsgType:
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
                default:
                    MyLogger.Log("Unknown MsgType " + messageType + " for component " + componentId);
                    break;
            }
            MyLogger.Log("Parsed message = " + msg);
            return true;
        }

    }
}
