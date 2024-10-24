using System.Net.Sockets;
using System.Net;
using Simulator.Utility;
using Simulator.MPS;
using LlsfMsgs;

namespace Simulator.RobotEssentials {
    /// <summary>
    /// Class <c>TcpConnector</c> is used to communicate with
    /// The refbox to receive Machine Positions to relay to the MPSManager
    /// Listen to tcp AgentTask messages to relay to the RobotManager
    /// </summary>
    class TcpConnector : ConnectorBase {
        //TODO add second condition to assign team 2

        private Socket ConnectSocket;
        private Socket? ListenSocket;
        private IPEndPoint? listenEndpoint;
        private Robot? Robot;

        public Thread? ConnectThread;
        public Thread? ListenThread;

        public TcpConnector(Configurations config, string ip, int port, MpsManager mpsManager, RobotManager robotManager, MyLogger logger)
            : base(config, ip, port, logger) {
            //THIS CONSTRUCTOR IS USED TO COMMUNICATE WITH THE REFBOX TO GET ROBOTINFO MACHIEN INFO AND GAMESTATE
            MyLogger.Log("Starting Refbox TcpConnector for " + ip + ":" + port + "!");

            ConnectSocket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ConnectThread = new Thread(() => ReceiveThreadMethod(ConnectSocket));
            ConnectThread.Name = "Manager_TCP_ReceiveThread";

            PbHandler = new PBMessageHandlerMachineManager(Config, mpsManager, robotManager, MyLogger);

            Connect();
            ConnectThread.Start();
        }

        public TcpConnector(Configurations config, Robot robot, MyLogger logger)
            : base(config, robot.RobotConfig.Host, robot.RobotConfig.SendPort, logger) {
            //TODO LISTEN FOR INCOMMING AGENT TASK MESSAGES AND SEND THEM BACK
            MyLogger.Log("Starting RobotManager TcpConnector on port:" + robot.RobotConfig.SendPort + "!");
            Robot = robot;

            ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ConnectThread = new Thread(() => SendToAgent());
            ConnectThread.Name = Robot.RobotName + "Manager_TCP_SendThread";

            PbHandler = new PBMessageHandlerRobot(Config, robot, MyLogger);

            listenEndpoint = new IPEndPoint(IPAddress.Any, robot.RobotConfig.RecvPort);
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(listenEndpoint);
            ListenSocket.Listen(10);

            ListenThread = new Thread(() => AcceptClients());
            ListenThread.Name = "Manager_TCP_ReceiveThread";

            Connect();
            ListenThread.Start();
            ConnectThread.Start();
        }

        public void SendToAgent() {
            //TODO SEND AGENT TASK MESSAGES PERIODICALLY TO THE AGENT PROBABLY IMPLEMENT IN THE BASE CLASS
        }

        public void AcceptClients() {
            while (Running) {
                try {
                    // Accept incoming connections
                    Socket? clientSocket = ListenSocket?.Accept();
                    Console.WriteLine("Accepted a new connection.");

                    if (clientSocket == null) {
                        Thread.Sleep(1000);
                        continue;
                    }
                    // Start a new thread to handle this client
                    Thread clientThread = new Thread(() => ReceiveThreadMethod(clientSocket));
                    clientThread.Start();
                }
                catch (Exception e) {
                    Console.WriteLine($"Error accepting client: {e.Message}");
                }
            }
        }


        public void ReceiveThreadMethod(Socket socket) {
            MyLogger.Log("Starting the ReceiveThread!");
            if (socket == null) {
                throw new Exception("Socket is null");
            }
            if (PbHandler == null) {
                throw new Exception("PBHandler is null");
            }
            while (Running) {
                try {
                    if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0) {
                        MyLogger.Log("Connection closed by remote host.");
                        break;
                    }
                    if (socket.Available == 0) {
                        Thread.Sleep(50);
                        continue;
                    }
                    MyLogger.Log("Waiting for a message!");
                    var buffer = new byte[4096];
                    var message = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    int payload = PbHandler.CheckMessageHeader(buffer);
                    if (payload == -1) {
                        continue;
                    }
                    MyLogger.Log("Lines Receive " + message + " of " + payload);
                    int remainingBytes = payload + 8 - message;
                    while (remainingBytes > 0) {
                        MyLogger.Log($"Missing {remainingBytes} bytes, receiving more data...");
                        message = socket.Receive(buffer, message, remainingBytes, SocketFlags.None);
                        MyLogger.Log("Lines Receive " + message);
                        remainingBytes = payload + 8 - message;
                        if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0) {
                            MyLogger.Log("Connection closed by remote host.");
                            break;
                        }
                    }
                    PbHandler.HandleMessage(buffer);
                }
                catch (SocketException se) {
                    MyLogger.Log(se + " - Socket exception occurred in the ReceiveThread!");
                    return;
                }
                catch (Exception e) {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                }
            }
        }

        public bool Connect() {
            MyLogger.Log("Connecting ....");
            while (!ConnectSocket.Connected) {
                try {
                    ConnectSocket.Connect(Endpoint);
                    MyLogger.Log(".... connected!");
                }
                catch (SocketException) {
                    MyLogger.Log("Wasn't able to CONNECT to the " + IP + ":" + Port + "  retrying in a few seconds!");
                    Thread.Sleep(10000);
                }
            }
            return true;
        }

        public override void Stop() {
            try {
                Running = false;
                ConnectSocket.Close();
                ListenSocket?.Close();
            }
            catch (SocketException) {
                MyLogger.Log(" Something went wrong with closing the Connecting to the Teamserver!");
            }
            return;
        }
    }
}
