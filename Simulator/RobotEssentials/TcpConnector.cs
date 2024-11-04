using System.Net.Sockets;
using System.Net;
using Simulator.Utility;
using Simulator.MPS;

namespace Simulator.RobotEssentials;
/// <summary>
/// Class <c>TcpConnector</c> is used to communicate with
/// The refbox to receive Machine Positions to relay to the MPSManager
/// Listen to tcp AgentTask messages to relay to the RobotManager
/// </summary>
class TcpConnector : ConnectorBase {
    private Socket ConnectSocket;
    private Socket? ListenSocket;
    private IPEndPoint? listenEndpoint;
    private Robot? Robot;

    public Thread? ConnectThread;
    public Thread? ListenThread;

    public TcpConnector(Configurations config, string ip, int port, MpsManager mpsManager, RobotManager robotManager,
                        GTMonitor? gtMonitor, MyLogger logger)
        : base(config, ip, port, logger) {
        //THIS CONSTRUCTOR IS USED TO COMMUNICATE WITH THE REFBOX TO GET ROBOTINFO MACHIEN INFO AND GAMESTATE
        MyLogger.Info("Starting Refbox TcpConnector for " + ip + ":" + port + "!");

        ConnectSocket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        ConnectThread = new Thread(() => ReceiveThreadMethod(ConnectSocket));
        ConnectThread.Name = "Manager_TCP_ReceiveThread";

        PbHandler = new PBMessageHandlerMachineManager(Config, mpsManager, robotManager, gtMonitor, MyLogger);

        Connect();
        ConnectThread.Start();
    }

    public TcpConnector(Configurations config, Robot robot, MyLogger logger)
        : base(config, robot.RobotConfig.Host, robot.RobotConfig.SendPort, logger) {
        MyLogger.Info("Starting AgentTask TcpConnector on port:" + robot.RobotConfig.RecvPort + "!");
        Robot = robot;

        ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        ConnectThread = new Thread(() => SendToAgent());
        ConnectThread.Name = Robot.RobotName + "Manager_TCP_SendThread";

        PbHandler = new PBMessageHandlerRobot(Config, robot, MyLogger);
        PbFactory = new PBMessageFactoryRobot(Config, robot, MyLogger);

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
        if (PbFactory == null) {
            throw new Exception("PBFactory is null");
        }
        while (Running) {
            try {
                var task = PbFactory.GetAgentTask();
                if (task != null) {
                    ConnectSocket.Send(task.GetBytes());
                }
                Thread.Sleep(500);
                var lastTask = PbFactory.GetLastTask();
                if (lastTask != null) {
                    ConnectSocket.Send(lastTask.GetBytes());
                }
                Thread.Sleep(500);
                if(!Config.RobotReportDirect && ReportMessages.Count > 0) {
                    lock(ReportMessages) {
                        ConnectSocket.Send(ReportMessages.Dequeue());
                        Thread.Sleep(200);
                    }
                }
            }
            catch (SocketException se) {
                MyLogger.Error(se + " - Socket exception occurred in the SendToAgentThread!");
                return;
            }
            catch (Exception e) {
                MyLogger.Error(e + " - Something went wrong with the SendToAgentThread!");
            }
        }
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
        MyLogger.Info("Starting the ReceiveThread!");
        if (socket == null) {
            throw new Exception("Socket is null");
        }
        if (PbHandler == null) {
            throw new Exception("PBHandler is null");
        }
        while (Running) {
            try {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0) {
                    MyLogger.Warn("Connection closed by remote host.");
                    break;
                }
                if (socket.Available == 0) {
                    Thread.Sleep(50);
                    continue;
                }
                MyLogger.Debug("Waiting for a message!");
                var buffer = new byte[4096];
                var message = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                int payload = PbHandler.CheckMessageHeader(buffer);
                if (payload == -1) {
                    continue;
                }
                MyLogger.Debug("Lines Receive " + message + " of " + payload);
                int remainingBytes = payload + 8 - message;
                while (remainingBytes > 0) {
                    MyLogger.Debug($"Missing {remainingBytes} bytes, receiving more data...");
                    message = socket.Receive(buffer, message, remainingBytes, SocketFlags.None);
                    MyLogger.Debug("Lines Receive " + message);
                    remainingBytes = payload + 8 - message;
                    if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0) {
                        MyLogger.Warn("Connection closed by remote host.");
                        break;
                    }
                }
                PbHandler.HandleMessage(buffer);
            }
            catch (SocketException se) {
                MyLogger.Error(se + " - Socket exception occurred in the ReceiveThread!");
                return;
            }
            catch (Exception e) {
                MyLogger.Error(e + " - Something went wrong with the ReceiveThread!");
            }
        }
    }

    public bool Connect() {
        MyLogger.Info("Connecting ....");
        while (!ConnectSocket.Connected) {
            try {
                ConnectSocket.Connect(Endpoint);
                MyLogger.Info(".... connected!");
            }
            catch (SocketException) {
                MyLogger.Warn("Wasn't able to CONNECT to the " + IP + ":" + Port + "  retrying in a few seconds!");
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
            MyLogger.Error(" Something went wrong with closing the TCPConection!");
        }
        return;
    }
}
