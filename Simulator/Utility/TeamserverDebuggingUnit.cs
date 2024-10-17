using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Simulator.RobotEssentials;

namespace Simulator.Utility {
    class TeamserverDebuggingUnit {
        // GRIPS: Fix your compile warnings
        // PBMessageFactoryRobot Factory;
        // PBMessageHandlerRobot Handler;
        Configurations Config;
        MyLogger MyLogger;
        public IPEndPoint? SendEndpoint;
        private Thread? PublicRecvThread;
        private Thread? PrivateRecvThread;
        private bool Running;
        public TeamserverDebuggingUnit(Configurations config) {
            MyLogger = new MyLogger("Teamserver", true);
            // GRIPS: Fix your compile warnings
            // Factory = new PBMessageFactoryRobot(Config, null, MyLogger);
            // Handler = new PBMessageHandlerRobot(Config, null, MyLogger);
            Config = config;
            if (Config == null || Config.Refbox == null) {
                throw new Exception("No Refbox Configuration is found!");
            }
            Running = true;

            MyLogger.Log("Starting the public receive thread");
            PublicRecvThread = new Thread(() => ReceiveUdpMethod(Config.Refbox.PublicSendPort, "public"));
            PublicRecvThread.Start();
            MyLogger.Log("Starting the cyan receive thread");
            PrivateRecvThread = new Thread(() => ReceiveUdpMethod(Config.Refbox.CyanSendPort, "private"));
            PrivateRecvThread.Start();


        }
        public void ReceiveUdpMethod(int port, string prefix) {
            MyLogger.Log("Starting the " + prefix + " ReceiveUDPMethod!");
            if (Config?.Refbox == null) {
                MyLogger.Log("No Refbox Configuration is found!");
                return;
            }
            MyLogger.Log(prefix + " Waiting on message on port " + port);
            var addr = IPAddress.Parse(Config.Refbox.IP);
            SendEndpoint = new IPEndPoint(IPAddress.Any, port);
            var udpServer = new UdpClient(port) {
                EnableBroadcast = true
            };
            MyLogger.Log("Broadcasts are = " + udpServer.EnableBroadcast);
            while (Running) {
                try {
                    MyLogger.Log(prefix + "Waiting on message on port " + port);
                    var message = udpServer.Receive(ref SendEndpoint);
                    // GRIPS: Fix your compile warnings
                    // var payload = Handler.CheckMessageHeader(message);
                    // MyLogger.Log(prefix + "Received " + message.Length + " bytes and decoded the payload as being " + payload);
                    // Handler.HandleMessage(message);
                }
                catch (Exception e) {
                    MyLogger.Log(e + " - Something went wrong with the" + prefix + " ReceiveThread!");
                }
            }

        }
    }
}
