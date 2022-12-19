﻿using System;
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
        private readonly PBMessageHandlerBase PbHandler;
        private Configurations Config;
        private bool OnlySending;
        private UdpClient Client;
        
        private string IpString;
        public UdpConnector(string ip, int port, Robot? rob, MyLogger logger, bool onlySend) : base(ip, port, rob, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            PbHandler = new PBMessageHandlerRobot(rob, MyLogger);
            Config = Configurations.GetInstance();
            
            ResolveIpAddress(ip);
            Endpoint = new IPEndPoint(Address, Port);
            RecvThread = new Thread(() => ReceiveUdpMethod());
            SendThread = new Thread(() => SendUdpMethod());

            PbFactory = Owner != null ? new PBMessageFactoryRobot(Owner, MyLogger) : new PBMessageFactoryBase(MyLogger);
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public UdpConnector(string ip, int port, MyLogger logger) : base(ip, port, null, logger)
        {
            //address = System.Net.IPAddress.Parse(Configurations.GetInstance().Refbox.IP);
            MyLogger.Log("Starting UdpConnector without a robot!");
            PbHandler = new PBMessageHandlerMachineManager(MyLogger);
            Config = Configurations.GetInstance();
            IpString = ip;
            RecvThread = new Thread(() => ReceiveUdpMethod());
            //SendThread = new Thread(() => SendUdpMethod());

            PbFactory = Owner != null ? new PBMessageFactoryRobot(Owner, MyLogger) : new PBMessageFactoryBase(MyLogger);
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            //WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public void ReceiveUdpMethod()
        {
            MyLogger.Log("Starting the ReceiveUDPMethod!");
            ResolveIpAddress(IpString);
            Endpoint = new IPEndPoint(Address, Port);
            MyLogger.Log("Broadcasts are = " + Client.EnableBroadcast);
            Client.Connect(Endpoint);

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
            ResolveIpAddress(IpString);
            Endpoint = new IPEndPoint(Address, Port);
            //SendEndpoint = new IPEndPoint(IPAddress.Any, port);
            //UdpClient client = new UdpClient(SendEndpoint);
            
            while (Running)
            {
                try
                {
                    MyLogger.Log("Sending a message to " + Address + ":" + Port + "!");
                    byte[] message;
                    if(Owner != null)
                    {   
                        message = ((PBMessageFactoryRobot)PbFactory).CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                    }
                    else
                    {
                        message = PbFactory.CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
                    }
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
