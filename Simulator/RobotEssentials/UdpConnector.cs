using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    /// <summary>
    /// Class <c>UdpConnector</c> is used for communication with the Refbox.
    /// </summary>
    class UdpConnector : ConnectorBase
    {
        private bool OnlySending;
        private UdpClient Client;
        
        private string IpString;
        public UdpConnector(Configurations config, string ip, int port, Robot rob, MyLogger logger, bool onlySend) : base(config, ip, port, rob, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            OnlySending = onlySend;
            PbHandler = new PBMessageHandlerRobot(Config, rob, MyLogger);

            IpString = ip;

            RecvThread = new Thread(() => ReceiveUdpMethod());
            RecvThread.Name = "Robot" + rob.JerseyNumber + "_UDP_ReceiveThread";
            SendThread = new Thread(() => SendUdpMethod());
            SendThread.Name = "Robot" + rob.JerseyNumber + "_UDP_ReceiveThread";
            PbFactory = Owner != null ? new PBMessageFactoryRobot(Config, Owner, MyLogger) : new PBMessageFactoryBase(Config, MyLogger);
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public UdpConnector(Configurations config, string ip, int port, MpsManager mpsManager, MyLogger logger) : base(config, ip, port, null, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            MyLogger.Log("Starting UdpConnector without a robot!");

            PbHandler = new PBMessageHandlerMachineManager(Config, mpsManager, MyLogger);

            IpString = ip;
            RecvThread = new Thread(() => ReceiveUdpMethod());
            RecvThread.Name = "mpsManager_UDP_ReceiveThread";
            //SendThread = new Thread(() => SendUdpMethod());

            PbFactory = Owner != null ? new PBMessageFactoryRobot(Config, Owner, MyLogger) : new PBMessageFactoryBase(Config, MyLogger);
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public void ReceiveUdpMethod()
        {
            MyLogger.Log("Starting the ReceiveUDPMethod!");
            ResolveIpAddress(IpString);
            var recvEndpoint = new IPEndPoint(IPAddress.Any, 0);
            MyLogger.Log("Broadcasts are = " + Client.EnableBroadcast);

            while (Running)
            {
                try
                {
                    MyLogger.Log("Waiting on message on port " + Port);
                    var message = Client.Receive(ref recvEndpoint);
                    var payload = PbHandler.CheckMessageHeader(message);
                    MyLogger.Log("Received " + message.Length + " bytes and decoded the payload as being " + payload);
                    PbHandler.HandleMessage(message);
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the ReceiveThread!");
                    Thread.Sleep(1000);
                }
            }
        }

        public void SendUdpMethod()
        {
            MyLogger.Log("Sending message to port " + Port);

            while (Running)
            {
                try
                {
                    byte[] message;
                    if(Messages.Count == 0)
                    {
                        MyLogger.Log("Sending a message to " + Address + ":" + Port + "!");
                        message = ((PBMessageFactoryRobot)PbFactory).CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                        if (Owner != null)
                        {
                            message = ((PBMessageFactoryRobot)PbFactory).CreateMessage((PBMessageFactoryBase
                                .MessageTypes.BeaconSignal));
                        }
                        else
                        {
                            message = PbFactory.CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                        }
                    }
                    else
                    {
                        MyLogger.Log("Sending Queue message to " + Address + ":" + Port + "!");
                        message = Messages.Dequeue();
                    }
                    if(message != Array.Empty<byte>())
                    {
                        Client.Send(message, message.Length,Address.ToString(), Port);
                    }
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the SendThread!");
                }
                Thread.Sleep(1000);
            }

        }
        
        public bool Start()
        {
            Running = true;
            //PublicSendThread.Start();
            if(Owner == null)
            {
                RecvThread.Start();
            }
            else
            {
                SendThread.Start();
                Thread.Sleep(400);
                if (!OnlySending)
                {
                    RecvThread.Start();
                }
            }
            return true;
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
