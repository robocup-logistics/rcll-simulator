using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    /// <summary>
    /// Class <c>TcpConnector</c> is used for communication with the Teamserver.
    /// </summary>
    class TcpConnector : ConnectorBase
    {
        //TODO add second condition to assign team 2

        private Socket Socket;
        private EventWaitHandle WaitSend;
        private ManualResetEvent WakePeerUpEvent;
        public TcpConnector(Configurations config, string ip, int port, Robot rob, MyLogger logger) : base(config, ip, port, rob, logger)
        {
            MyLogger.Log("Starting Robot TcpConnector for " + ip + ":" + port + "!");


            ResolveIpAddress(ip);
            Endpoint = new IPEndPoint(Address, Port);
            Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            RecvThread = new Thread(() => ReceiveThreadMethod());
            RecvThread.Name = "Robot" + rob.JerseyNumber + "_TCP_ReceiveThread";
            if (Owner != null)
            {
                SendThread = new Thread(() => SendThreadMethod());
                SendThread.Name = "Robot" + rob.JerseyNumber + "_TCP_SendThread";

            }

            PbFactory = Owner != null ? new PBMessageFactoryRobot(Config, Owner, MyLogger) : new PBMessageFactoryBase(Config, MyLogger);

            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
            PbHandler = new PBMessageHandlerRobot(Config, Owner, MyLogger);
        }

        public TcpConnector(Configurations config, string ip, int port, MpsManager manager, MyLogger logger) : base(
            config, ip, port, null, logger)
        {
            MyLogger.Log("Starting MPS Manager TcpConnector for " + ip + ":" + port + "!");


            ResolveIpAddress(ip);
            Endpoint = new IPEndPoint(Address, Port);
            Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            RecvThread = new Thread(() => ReceiveThreadMethod());
            RecvThread.Name = "MpsManager_TCP_ReceiveThread";
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
            PbHandler = new PBMessageHandlerMachineManager(Config, manager, MyLogger);
            RecvThread.Start();
        }
        public bool Close()
        {
            Socket.Close();
            return true;
        }

        public bool Connect()
        {
            MyLogger.Log("Connecting ....");
            while (!Socket.Connected)
            {
                try
                {
                    Socket.Connect(Endpoint);
                    MyLogger.Log(".... connected!");
                }
                catch (SocketException)
                {
                    MyLogger.Log("Wasn't able to CONNECT to the " + IP + ":" + Port + "  retrying in a few seconds!");
                    Thread.Sleep(10000);
                }
            }
            return true;
        }

        public bool Start()
        {
            try
            {
                MyLogger.Log("Starting ....");
                if (Connect())
                {
                    Running = true;
                    MyLogger.Log(".... Started");
                    if (SendThread != null)
                    {
                        SendThread.Start();
                    }
                    RecvThread.Start();
                }
                else
                {
                    MyLogger.Log(".... couldn't start..");
                    return false;
                }
            }
            catch (SocketException)
            {
                MyLogger.Log("STARTING the teamserver didn't work");
                Running = false;
                return false;
            }
            /*finally
            {
                Socket?.Close();
            }*/
            return true;
        }

        public bool Stop()
        {
            try
            {
                Close();
                Running = false;
            }
            catch (SocketException)
            {
                MyLogger.Log(" Something went wrong with closing the Connecting to the Teamserver!");
                return false;
            }
            return true;
        }

        public void SendThreadMethod()
        {
            MyLogger.Log("Starting the SendThread!");
            if (Socket == null)
            {
                return;
            }
            while (Running)
            {
                try
                {
                    //MyLogger.Log("Waiting for a send!");
                    //WaitSend.WaitOne();
                    //MyLogger.Log("Got a new Message to send!");
                    while (true)
                    {
                        byte[] msg;
                        if (Messages.Count == 0)
                        {
                            //robot sending a Gripsbeacon message every time he enters. Maybe reduce this spam in the future
                            //msg = CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                            msg = CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                        }
                        else
                        {
                            msg = Messages.Dequeue();
                        }
                        if (msg != Array.Empty<byte>())
                        {
                            Socket.Send(msg);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the sending to the Teamserver!");
                    if (!Socket.Connected)
                    {
                        MyLogger.Log("the connection is lost retry to connect!");
                        while (!Socket.Connected)
                        {
                            Socket.Connect(Endpoint);
                        }
                    }
                }
            }
        }



        public void ReceiveThreadMethod()
        {
            MyLogger.Log("Starting the ReceiveThread!");
            while (Running)
            {
                try
                {
                    if (Socket.Available == 0)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                    //MyLogger.Log("Waiting for a message!");
                    var buffer = new byte[4096];
                    var message = Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    var payload = PbHandler.CheckMessageHeader(buffer);
                    if (payload == -1)
                    {
                        continue;
                    }
                    //MyLogger.Log("Lines Receive " + message);
                    if (payload > message)
                    {
                        //MyLogger.Log("Missing bytes, we need to receive more " + message + "/" + payload);
                        message = Socket.Receive(buffer, message, payload + 16 - message, SocketFlags.None);
                        //MyLogger.Log("Lines Receive " + message);
                    }
                    //MyLogger.Log("Received a message!");
                    PbHandler.HandleMessage(buffer);
                    //MyLogger.Log("Handled the message!");
                    //MyLogger.Log(message.ToString());
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                }
            }
        }

        public bool GetConnected()
        {
            return Socket.Connected;
        }
        public void HandleMessage(IAsyncResult res)
        {

        }

        public void SetWakeUpEvent(ManualResetEvent mre)
        {
            WakePeerUpEvent = mre;
        }
    }
}
