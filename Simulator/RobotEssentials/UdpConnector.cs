using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    /// <summary>
    /// Class <c>UdpConnector</c> is used for communication with the Refbox.
    /// </summary>
    class UdpConnector : ConnectorBase
    {
        private readonly PbMessageHandlerMachineManager PbHandler;
        private Configurations Config;
        private bool OnlySending;
        private UdpClient Client;

        public UdpConnector(string ip, int port, Robot? rob, MyLogger logger , bool onlySend) : base(ip, port, rob, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            PbHandler = new PbMessageHandlerMachineManager(MyLogger);
            Config = Configurations.GetInstance();
            
            ResolveIpAddress(ip);
            Endpoint = new IPEndPoint(Address, Port);
            RecvThread = new Thread(() => ReceiveUdpMethod());
            SendThread = new Thread(() => SendUdpMethod());

            PbFactory = Owner != null ? new PBMessageFactoryRobot(Owner, MyLogger) : new PBMessageFactoryBase(MyLogger);
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
            HandlerRobot = new PBMessageHandlerRobot(Owner, MyLogger);
        }

        public void ReceiveUdpMethod()
        {
            MyLogger.Log("Starting the ReceiveUDPMethod!");
            MyLogger.Log("Waiting on message on port " + Port);
            Endpoint = new IPEndPoint(Address, Port);
            MyLogger.Log("Broadcasts are = " + Client.EnableBroadcast);
            while (Running)
            {
                try
                {
                    MyLogger.Log("Waiting on message on port " + Port);
                    var message = Client.Receive(ref Endpoint);
                    var payload = PbHandler.CheckMessageHeader(message);
                    MyLogger.Log("Received " + message.Length + " bytes and decoded the payload as being " + payload);
                    PbHandler.HandleMessage(message);
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                }
            }

        }

        public void SendUdpMethod()
        {
            MyLogger.Log("Sending message to port " + Port);
            //SendEndpoint = new IPEndPoint(IPAddress.Any, port);
            //UdpClient client = new UdpClient(SendEndpoint);
            
            while (Running)
            {
                try
                {
                    MyLogger.Log("Sending a message to " + Address + ":" + Port + "!");
                    var message = PbFactory.CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                    if(message != Array.Empty<byte>())
                    {
                        Client.Send(message, message.Length,Address.ToString(), Port);
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the SendThread!");
                }
            }

        }
        
        public bool Start()
        {
            //PublicSendThread.Start();
            Running = true;
            SendThread.Start();
            Thread.Sleep(400);
            if (!OnlySending)
            {
                RecvThread.Start();
            }
            return true;
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
