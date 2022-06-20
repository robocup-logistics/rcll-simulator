using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    class ConnectorBase
    {
        public Robot? Owner;
        public bool Running;
        public IPEndPoint Endpoint;
        public MyLogger MyLogger;
        public Queue<byte[]> Messages;
        public Thread SendThread;
        public Thread RecvThread;
        public IPAddress Address;
        public PBMessageFactoryBase PbFactory;
        protected PBMessageHandlerRobot HandlerRobot;

        public string IP;
        public int Port;

        //public UdpClient UdpSender;
        //public UdpClient UdpReciever;
        public ConnectorBase(string ip, int port, Robot? rob, MyLogger logger)
        {
            Messages = new Queue<byte[]>();
            MyLogger = logger;
            IP = ip;
            Port = port;
            this.Owner = rob;
            Address = IPAddress.Any;
            //PbFactory = Owner != null ? new PBMessageFactoryRobot(Owner, MyLogger) : new PBMessageFactoryBase(MyLogger);
        }

        public bool ResolveIpAddress(string ip)
        {
            while (Address.Equals(IPAddress.Any))
            {
                try
                {
                    Address = Dns.GetHostAddresses(ip)[0];
                }
                catch (Exception)
                {
                    MyLogger.Log("Not able to get DNS? Retrying");
                    Thread.Sleep(1000);
                }
            }
            return true;
        }
        public void AddMessage(byte[] msg)
        {
            MyLogger.Log("Added a Message to the List!");
            Messages.Enqueue(msg);
            //WaitSend.Set();
        }
        public byte[] CreateMessage(PBMessageFactoryBase.MessageTypes type)
        {
            return PbFactory.CreateMessage(type);
        }
    }
}
