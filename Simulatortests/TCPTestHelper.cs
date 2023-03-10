using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulator.RobotEssentials;
using Simulator.Utility;

namespace Simulatortests
{
    public class TCPTestHelper
    {
        private readonly int Port;
        private readonly Socket Client;
        private readonly TcpListener server;
        private readonly EndPoint Endpoint;
        private MyLogger Logger;
        private Thread SendThread;
        private Thread ReceiveThread;
        private bool Running;
        public TCPTestHelper(int port)
        {
            Logger = new MyLogger("TCPTest", true);
            Port = port;

            if (!IPAddress.TryParse("127.0.0.1", out var address))
            {
                Logger.Log("Couldn't parse local address?");
                return;
            }


            //Endpoint = new IPEndPoint(address, Port);
            server = new TcpListener(address, port);
            //Client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //SendThread = new Thread(SendMethod);
            ReceiveThread = new Thread(RecvMethod);
        }
        public bool CreateConnection()
        {
            try
            {
                Logger.Log("Starting Connection?");
                //Client.Bind(Endpoint);
                server.Start();
                //Client.Connect(Endpoint);
                Running = true;
                //SendThread.Start();
                ReceiveThread.Start();
            }
            catch(Exception ex)
            {
                Logger.Log("Caught exception in CreateConnection!" + ex.Message);
                return false;
            }
            return true;
        }

        public bool CloseConnection()
        {

            try
            {
                Running = false;
                Client.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SendMethod()
        {
            while (Running)
            {

            }
        }

        private void RecvMethod()
        {
            Logger.Log("Starting the RecvMethod!");
            //Logger.Log(Client.LocalEndPoint.ToString());
            while (Running)
            {
                Thread.Sleep(200);
                try
                {
                    Logger.Log("Something to Recv?");
                    if (Client.Available == 0)
                    {
                        Logger.Log("Nope!");
                        continue;
                    }
                    //MyLogger.Log("Waiting for a message!");
                    var buffer = new byte[4096];
                    var message = Client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    Logger.Log("Received:");
                    Logger.Log(buffer);
                    /*var payload = PbHandler.CheckMessageHeader(buffer);
                    if (payload == -1)
                    {
                        continue;
                    }
                    //MyLogger.Log("Lines Receive " + message);
                    if (payload > message)
                    {
                        message = Client.Receive(buffer, message, payload + 16 - message, SocketFlags.None);

                    }*/

                }
                catch (Exception e)
                {
                    Logger.Log(e + " - Something went wrong with the ReceiveThread!");
                }
            }
        }
    }
}
