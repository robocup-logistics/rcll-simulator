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
            SendClient.Client.Bind(Endpoint);

            SendThread.Start();
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
            RecvClient.EnableBroadcast = true;

            SendClient = new UdpClient();
            SendClient.EnableBroadcast = true;
            // SendClient.Client.Bind(Endpoint);

            RecvThread = new Thread(() => ReceiveAgentTask());
            RecvThread.Name = robot.RobotName + "mpsManager_UDP_ReceiveThread";
            SendThread = new Thread(() => SendAgentTask());
            SendThread.Name = robot.RobotName + "mpsManager_UDP_SendThread";

            SendThread.Start();
            RecvThread.Start();
        }

        public void ReceiveAgentTask() {
            Console.WriteLine("adsas");
            MyLogger.Log("Starting the ReceiveUDPMethod!");

            if (RecvClient == null) {
                throw new Exception("RecvClient is null");
            }
            if (PbHandler == null) {
                throw new Exception("PBHandler is null");
            }

            while (Running) {
                try {
                    Console.WriteLine("HUI");
                    MyLogger.Log("Waiting on message on port " + Port);

                    // Receive the message
                    var message = RecvClient.Receive(ref receiveEndpoint);

                    Console.WriteLine("SHSLF");
                    MyLogger.Log("Received " + message.Length + " bytes.");

                    // Check the message header and get the payload size
                    var payload = PbHandler.CheckMessageHeader(message);
                    MyLogger.Log("Decoded the payload as being " + payload);

                    // If payload is invalid, skip processing
                    if (payload == -1) {
                        continue;
                    }

                    Console.WriteLine("SHSLF");
                    // If the payload indicates missing data
                    if (payload > message.Length - 8) {
                        int remainingBytes = payload + 8 - message.Length;
                        Console.WriteLine("dsaf");

                        MyLogger.Log($"Missing {remainingBytes} bytes, receiving more data...");

                        // Receive the remaining data (This part assumes RecvClient is capable of receiving in chunks)
                        while (remainingBytes > 0) {
                            var extraData = RecvClient.Receive(ref receiveEndpoint);

                            var combinedMessage = new byte[message.Length + extraData.Length];
                            Buffer.BlockCopy(message, 0, combinedMessage, 0, message.Length);
                            Buffer.BlockCopy(extraData, 0, combinedMessage, message.Length, extraData.Length);

                            message = combinedMessage;

                            remainingBytes -= extraData.Length;
                        }
                    }

                    // Now that we have the full message, process it
                    Console.WriteLine("Received message: " + message.Length + " bytes");
                    PbHandler.HandleMessage(message);
                }
                catch (Exception e) {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                    Thread.Sleep(1000); // Small delay to avoid tight loop on exceptions
                }
            }
        }

        public void SendAgentTask() {
            if(PbFactory == null) {
                throw new Exception("PBFactory is null");
            }
            if(SendClient == null) {
                throw new Exception("SendClient is null");
            }
            while(!Running) {
                var task = PbFactory.GetAgentTask();
                if(task != null) {
                    SendClient.Send(task.GetBytes(), task.GetBytes().Length, Endpoint);
                }
                Thread.Sleep(500);
                var lastTask = PbFactory.GetLastTask();
                if(lastTask != null) {
                    SendClient.Send(lastTask.GetBytes(), lastTask.GetBytes().Length);
                }
                Thread.Sleep(500);
            }
        }

        private void SendBeaconMethod() {
            if(PbFactory == null) {
                throw new Exception("PBFactory is null");
            }
            if(SendClient == null) {
                throw new Exception("SendClient is null");
            }
            while(!Running) {
                var msg = PbFactory.CreateBeaconSignal();
                SendClient.Send(msg.GetBytes(), msg.GetBytes().Length);
                Thread.Sleep(500);
            }
        }
    }
}
