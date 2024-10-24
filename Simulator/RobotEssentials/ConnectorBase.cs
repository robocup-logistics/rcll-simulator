using System.Net;
using Simulator.Utility;

namespace Simulator.RobotEssentials {
    abstract class ConnectorBase {
        public bool Running = true;
        public IPEndPoint Endpoint;
        public MyLogger MyLogger;
        public Queue<byte[]> Messages;
        public IPAddress Address = IPAddress.Any;
        public PBMessageFactoryRobot? PbFactory;
        public PBMessageHandlerBase? PbHandler;
        public readonly Configurations Config;
        public string IP;
        public int Port;

        protected ConnectorBase(Configurations config, string ip, int port, MyLogger logger) {
            ResolveIpAddress(ip);
            Messages = new Queue<byte[]>();
            MyLogger = logger;
            IP = ip;
            Port = port;
            Endpoint = new IPEndPoint(Address, Port);
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

        protected void MessageReceived(byte[] message) {
            if (PbHandler == null) {
                //TODO CHANGE TO ERROR MESSAGE
                throw new Exception("PbHandler is null");
            }
            PbHandler.HandleMessage(message);
        }

        public virtual void Stop() {
            Running = true;
        }
    }
}
