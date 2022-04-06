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
        //public UdpClient UdpSender;
        //public UdpClient UdpReciever;
        public ConnectorBase(Robot? rob, MyLogger logger)
        {
            Messages = new Queue<byte[]>();
            
            MyLogger = logger;
            this.Owner = rob;
        }
        
    }
}
