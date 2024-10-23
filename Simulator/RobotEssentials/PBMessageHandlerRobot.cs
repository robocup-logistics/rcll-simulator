using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;

namespace Simulator.RobotEssentials {
    class PBMessageHandlerRobot : PBMessageHandlerBase {
        private Robot Robot;
        public PBMessageHandlerRobot(Configurations config, Robot robot, MyLogger log) : base(config, log) {
            Robot = robot;
        }

        protected override bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {
            string msg = "";
            switch (messageType) {
                case (int)AgentTask.Types.CompType.MsgType:
                    //TODO SEND NEW TASK TO ROBOT MANAGER
                    MessageParser<AgentTask> taskParser =
                        new(() => new AgentTask());

                    AgentTask task = taskParser.ParseFrom(stream, 12, payloadSize - 4);
                    MyLogger.Log("Parsing of the GripsMidLevelTasks was successful!");
                    Robot.HandleAgentTask(task);
                    msg = task.ToString();
                    break;
                default:
                    MyLogger.Log("Unknown MsgType " + messageType + " for component " + componentId);
                    return false;
            }
            MyLogger.Log("Handeld message = " + msg);
            return true;
        }

    }
}
