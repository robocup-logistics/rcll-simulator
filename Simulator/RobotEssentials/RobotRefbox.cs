using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    class RobotRefbox : ConnectorBase, IConnector
    {
        private readonly PbMessageHandlerMachineManager PbHandler;
        //readonly IPAddress Address;
        private Configurations? Config;
        private Thread? PrivateRecvThread;
        public RobotRefbox(Robot? rob, MyLogger logger) : base(rob, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            PbHandler = new PbMessageHandlerMachineManager(MyLogger);
            Config = Configurations.GetInstance();
            if (Config == null || Config.Refbox == null) return;
            MyLogger.Log("Starting the public receive thread");
            PublicRecvThread = new Thread(() => ReceiveUdpMethod(Config.Refbox.PublicRecvPort));
            MyLogger.Log("Starting the cyan receive thread");
            PrivateRecvThread = new Thread(() => ReceiveUdpMethod(Config.Refbox.CyanRecvPort));
            //PublicSendThread = new Thread(SendUdpMethod);
            //TeamRecvThread = new Thread(() => RecvThreadMethod(Configurations.GetInstance().Refbox.CyanSendPort));
            //TeamSendThread = new Thread(() => SendThreadMethod(Configurations.RefboxCyanRecvPort));

        }

        public void ReceiveUdpMethod(int port)
        {
            MyLogger.Log("Starting the ReceiveUDPMethod!");
            if(Config?.Refbox == null)
            {
                MyLogger.Log("No Refbox Configuration is found!");
                return;
            }
            MyLogger.Log("Waiting on message on port " + port);
            var addr = IPAddress.Parse(Config.Refbox.IP);
            SendEndpoint = new IPEndPoint(IPAddress.Any, port);
            var udpServer = new UdpClient(port)
            {
                EnableBroadcast = true
            };
            MyLogger.Log("Broadcasts are = " + udpServer.EnableBroadcast);
            while (Running)
            {
                try
                {
                    MyLogger.Log("Waiting on message on port " + port);
                    var message = udpServer.Receive(ref SendEndpoint);
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
            /*if(Config.Refbox == null)
            {
                MyLogger.Log("No Refbox Configuration is found!");
                return;
            }
            var port = Config.Refbox.CyanSendPort;
            var addr_string = Config.Refbox.IP;
            MyLogger.Log("Sending message to port " + port);
            var addr = IPAddress.Parse(addr_string);
            SendEndpoint = new IPEndPoint(addr, port);
            //SendEndpoint = new IPEndPoint(IPAddress.Any, port);
            //UdpClient client = new UdpClient(SendEndpoint);
            var client = new UdpClient();
            while (Running)
            {
                try
                {
                    MyLogger.Log("Sending a message to "+ addr_string + ":" + port + "!");
                    var message = FactoryBase.CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                    if(message != null)
                    {
                        client.Send(message, message.Length,Config.Refbox.IP, port);
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                }
            }*/

        }

        public bool Connect()
        {
            //UNUSED IN THE CASE OF UDP
            return false;
        }
        public bool Close()
        {
            //UNUSED IN THE CASE OF UDP
            return false;
        }

        public bool Start()
        {
            //PublicSendThread.Start();
            PrivateRecvThread.Start();
            PublicRecvThread.Start();
            Running = true;
            return true;
        }

        public void StartSendThread()
        {
            //UNUSED IN THE CASE OF UDP
            return;
        }
        public bool Stop()
        {
            //UNUSED IN THE CASE OF UDP
            return false;
        }

        public virtual void SendThreadMethod(int port)
        {
            //UNUSED IN THE CASE OF UDP
            return;
        }

        public virtual void ReceiveThreadMethod(int port)
        {
            //UNUSED IN THE CASE OF UDP
            return;
        }


        public virtual void AddRefboxMessage(byte[] message)
        {
            Messages.Enqueue(message);
        }

    }
}
