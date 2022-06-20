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
        public Socket? Socket;
        public IPEndPoint? SendEndpoint;
        public IPEndPoint? RecvEndpoint;
        public MyLogger MyLogger;
        public Queue<byte[]> Messages;
        public Thread? PublicRecvThread;
        public Thread? PublicSendThread;
        public Thread? TeamRecvThread;
        public Thread? TeamSendThread;
        public IPAddress Address;
        public PBMessageFactoryBase PbFactory;
        //public UdpClient UdpSender;
        //public UdpClient UdpReciever;
        public ConnectorBase(Robot? rob, MyLogger logger)
        {
            Messages = new Queue<byte[]>();
            MyLogger = logger;
            this.Owner = rob;
            Address = IPAddress.Any;
            PbFactory = Owner != null ? new PBMessageFactoryRobot(Owner, MyLogger) : new PBMessageFactoryBase(MyLogger);
        }

        public bool ResolveIpAddress()
        {
            while (Address.Equals(IPAddress.Any))
            {
                try
                {
                    Address = Dns.GetHostAddresses(Configurations.GetInstance().Teams[0].Ip)[0];
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
