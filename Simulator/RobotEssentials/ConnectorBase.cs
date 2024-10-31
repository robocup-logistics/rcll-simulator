using System.Net;
using Simulator.Utility;

namespace Simulator.RobotEssentials;
abstract class ConnectorBase {
    public bool Running = true;
    public IPEndPoint Endpoint;
    public MyLogger MyLogger;
    public Queue<byte[]> ReportMessages;
    public IPAddress Address = IPAddress.Any;
    public PBMessageFactoryRobot? PbFactory;
    public PBMessageHandlerBase? PbHandler;
    public readonly Configurations Config;
    public string IP;
    public int Port;

    protected ConnectorBase(Configurations config, string ip, int port, MyLogger logger) {
        ResolveIpAddress(ip);
        ReportMessages = new Queue<byte[]>();
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
                MyLogger.Warn("Not able to get DNS? Retrying");
                Address = IPAddress.Any;
                Thread.Sleep(1000);
                return false;
            }
        }
        return true;
    }

    protected void MessageReceived(byte[] message) {
        if (PbHandler == null) {
            throw new Exception("PbHandler is null");
        }
        PbHandler.HandleMessage(message);
    }

    public virtual void Stop() {
        Running = true;
    }

    public void AppendMachineReport(LlsfMsgs.MachineReport report) {
        if (PbFactory == null) {
            throw new Exception("PbFactory is null");
        }
        lock(ReportMessages){
            ReportMessages.Enqueue(PbFactory.CreateMachineReport(report));
        }
    }
}
