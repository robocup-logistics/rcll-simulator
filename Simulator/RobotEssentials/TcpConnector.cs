using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    /// <summary>
    /// Class <c>TcpConnector</c> is used for communication with the Teamserver.
    /// </summary>
    class TcpConnector : ConnectorBase
    {
        //TODO add second condition to assign team 2
        PBMessageFactoryRobot PbFactory;
        private PBMessageHandlerRobot HandlerRobot;
        IPAddress Address;
        private EventWaitHandle WaitSend;
        private ManualResetEvent WakePeerUpEvent;
        public TcpConnector(Robot rob, MyLogger logger) : base(rob, logger)
        {
            MyLogger.Log("Starting connection TcpConnector!");
            if (Owner == null)
            {
                MyLogger.Log("Starting connection to "+ Configurations.GetInstance().Teams[0].Name);
                Address = IPAddress.Parse(Configurations.GetInstance().Teams[0].Ip);
                SendEndpoint = new IPEndPoint(Address, Configurations.GetInstance().Teams[0].Port);
                Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                TeamRecvThread = new Thread(() => ReceiveThreadMethod(Configurations.GetInstance().Teams[0].Port));
                TeamSendThread = new Thread(() => SendThreadMethod(Configurations.GetInstance().Teams[0].Port));
            }
            else
            {
                if (Owner.TeamColor.Equals(LlsfMsgs.Team.Cyan))
                {
                    IPAddress ip = IPAddress.Any;
                    while(ip == IPAddress.Any)
                    {
                        try{
                            ip = Dns.GetHostAddresses(Configurations.GetInstance().Teams[0].Ip)[0];
                        }
                        catch(Exception e)
                        {
                            MyLogger.Log("Not able to get DNS? Retrying");
                            Thread.Sleep(1000);
                        }
                    }
                   
                    //Address = IPAddress.Parse(ip);
                    SendEndpoint = new IPEndPoint(ip, Configurations.GetInstance().Teams[0].Port);
                    Socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    TeamRecvThread = new Thread(() => ReceiveThreadMethod(Configurations.GetInstance().Teams[0].Port));
                    TeamSendThread = new Thread(() => SendThreadMethod(Configurations.GetInstance().Teams[0].Port));
                }
                else
                {
                    Address = IPAddress.Parse(Configurations.GetInstance().Teams[1].Ip);
                    SendEndpoint = new IPEndPoint(Address, Configurations.GetInstance().Teams[1].Port);
                    Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    TeamRecvThread = new Thread(() => ReceiveThreadMethod(Configurations.GetInstance().Teams[1].Port));
                    TeamSendThread = new Thread(() => SendThreadMethod(Configurations.GetInstance().Teams[1].Port));
                }
            }

            PbFactory = new PBMessageFactoryRobot(Owner, MyLogger);
            WaitSend = new EventWaitHandle(false, EventResetMode.AutoReset);
            HandlerRobot = new PBMessageHandlerRobot(Owner, MyLogger);
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
                    Socket.Connect(SendEndpoint);
                    MyLogger.Log(".... connected!");
                }
                catch (SocketException)
                {
                    MyLogger.Log("Wasn't able to CONNECT to the teamserver retrying in a few seconds!");
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
                    TeamRecvThread.Start();
                    TeamSendThread.Start();
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
        public void AddMessage(byte[] msg)
        {
            MyLogger.Log("Added a Message to the List!");
            Messages.Enqueue(msg);
            //WaitSend.Set();
        }
        public void SendThreadMethod(int port)
        {
            MyLogger.Log("Starting the SendThread!");
            if(Socket==null)
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
                    while(true)
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
                        if(msg!= null)
                        {
                            Socket.Send(msg);
                        }

                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    MyLogger.Log(e + " - Something went wrong with the sending to the Teamserver!");
                    if(!Socket.Connected)
                    {
                        MyLogger.Log("the connection is lost retry to connect!");
                        while(!Socket.Connected)
                        {
                            Socket.Connect(SendEndpoint);
                        }
                    }
                }
            }
        }
        public byte[]? GetTestMessage()
        {
            return PbFactory.CreateMessage(PBMessageFactoryBase.MessageTypes.BeaconSignal);
        }
        public byte[]? CreateMessage(PBMessageFactoryBase.MessageTypes type)
        {
            return PbFactory.CreateMessage(type);  
        }

        public void ReceiveThreadMethod(int port)
        {
            MyLogger.Log("Starting the ReceiveThread!");
            while (Running)
            {
                try
                {
                    if(Socket.Available == 0)
                    {
                        continue;
                    }
                    //MyLogger.Log("Waiting for a message!");
                    var buffer = new byte[4096];
                    var message = Socket.Receive(buffer,0,buffer.Length,SocketFlags.None);
                    var payload = HandlerRobot.CheckMessageHeader(buffer);
                    if(payload == -1)
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
                    HandlerRobot.HandleMessage(buffer);
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
