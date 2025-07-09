using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;

namespace Simulator.RobotEssentials;
class PBMessageHandlerRobot : PBMessageHandlerBase {
    private Robot Robot;
    public PBMessageHandlerRobot(Configurations config, Robot robot, MyLogger log) : base(config, log) {
        Robot = robot;
    }

    protected override bool ProcessMessage(byte[] stream, int componentId, int messageType, int payloadSize) {
        string msg = "";
        switch (messageType) {
            case (int)AgentTask.Types.CompType.MsgType:
                MessageParser<AgentTask> taskParser =
                    new(() => new AgentTask());

                AgentTask task = taskParser.ParseFrom(stream, 12, payloadSize - 4);
                MyLogger.Info("Parsing of the AgentTask was successful!");
                Robot.HandleAgentTaskMessage(task);
                msg = task.ToString();
                break;
            default:
                MyLogger.Warn("Unknown MsgType " + messageType + " for component " + componentId);
                return false;
        }
        MyLogger.Debug("Handeld message = " + msg);
        return true;
    }


}
