using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Org.BouncyCastle.Math.EC;
using Simulator.Utility;

namespace Simulator.RobotEssentials {
    abstract class ConnectorBase {
        public Robot? Owner;
        public bool Running;
        public IPEndPoint Endpoint;
        public MyLogger MyLogger;
        public Queue<byte[]> Messages;
        public Thread? SendThread;
        public Thread? RecvThread;
        public IPAddress Address = IPAddress.Any;
        public PBMessageFactoryBase? PbFactory;
        public PBMessageHandlerBase? PbHandler;
        public readonly Configurations Config;
        public string IP;
        public int Port;

        protected ConnectorBase(Configurations config, string ip, int port, Robot? rob, MyLogger logger) {
            ResolveIpAddress(ip);
            Messages = new Queue<byte[]>();
            MyLogger = logger;
            IP = ip;
            Port = port;
            Endpoint = new IPEndPoint(Address, Port);
            Owner = rob;
            Config = config;
        }

        public bool ResolveIpAddress(string ip) {
            // MyLogger.Log("Starting the ResolveIpFunction");
            while (Address.Equals(IPAddress.Any)) {
                try {
                    Address = Dns.GetHostAddresses(ip)[0];
                }
                catch (Exception) {
                    MyLogger.Log("Not able to get DNS? Retrying");
                    Address = IPAddress.Any;
                    Thread.Sleep(1000);
                    return false;
                }
            }
            return true;
        }
        public void AddMessage(byte[] msg) {
            MyLogger.Log("Added a Message to the List!");
            Messages.Enqueue(msg);
            //WaitSend.Set();
        }
        public byte[] CreateMessage(PBMessageFactoryBase.MessageTypes type) {
            if (PbFactory == null) {
                throw new Exception("PbFactory is null");
            }
            return PbFactory.CreateMessage(type);
        }
    }
}
