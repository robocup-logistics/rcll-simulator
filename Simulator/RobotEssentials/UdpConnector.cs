using System.Net;
using System.Net.Sockets;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials {
    /// <summary>
    /// Class <c>UdpConnector</c> is used to communicate with
    /// The refbox to send Beacon signals on behalf of a robot
    /// Listen to RobotInfo messages to relay to the robot
    /// Listen to udp AgentTask messages to relay to the RobotManager
    /// </summary>
    class UdpConnector : ConnectorBase {

        public UdpClient? SendClient;
        public UdpClient? RecvClient;
        public IPEndPoint? receiveEndpoint;

        public Thread? SendThread;
        public Thread? RecvThread;

        public UdpConnector(Configurations config, string refboxIp, int refboxPort,
                            Robot robot, MyLogger logger)
            : base(config, refboxIp, refboxPort, logger) {
            // IN THIS CONSTRUCTOR, THIS CLASS IS SENDING THE BEACON SIGNAL TO REFBOX FOR ROBOT
            Console.WriteLine("ip" + refboxIp + "port" + refboxPort, "rob" + robot, "logger" + logger, "sendBeacon");

            SendThread = new Thread(() => SendBeaconMethod());
            SendThread.Name = "Robot" + robot.JerseyNumber + "_UDP_SENNDER_THREAD";

            PbFactory = new PBMessageFactoryRobot(Config, robot, MyLogger);
            SendClient = new UdpClient();
            SendClient.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public UdpConnector(Configurations config, Robot robot, MyLogger logger)
            : base(config, robot.RobotConfig.Host, robot.RobotConfig.SendPort, logger) {
            // IN THIS CONSTRUCTOR THE CLASS IS WAITING FOR AGENT TASK MESSAGES ON A SPECIFIC PORT AND SEND BACK ON ANOTHER ONe
            MyLogger.Log("Starting UdpConnector for RobotManager to receive AgentTask messages!");

            PbHandler = new PBMessageHandlerRobot(Config, robot, MyLogger);
            PbFactory = new PBMessageFactoryRobot(Config, robot, MyLogger);

            receiveEndpoint = new IPEndPoint(IPAddress.Any, robot.RobotConfig.RecvPort);
            RecvClient = new UdpClient();
            RecvClient.Client.Bind(receiveEndpoint);

            RecvThread = new Thread(() => ReceiveAgentTask());
            RecvThread.Name = robot.RobotName + "mpsManager_UDP_ReceiveThread";
            SendThread = new Thread(() => SendAgentTask());
            SendThread.Name = robot.RobotName + "mpsManager_UDP_SendThread";

            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        public void SendAgentTask() {
            //TODO SEND AGENT TASK MESSAGES PERIODICALLY TO THE AGENT PROBABLY IMPLEMENT IN THE BASE CLASS
        }

    }
}
