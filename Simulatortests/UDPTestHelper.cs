using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Simulatortests
{
    public class UDPTestHelper
    {
        private readonly int Port;
        private UdpClient Client;
        public UDPTestHelper(int port)
        {
            Port = port;
            Client = new UdpClient(port);
        }
        
        public void SendTask(ushort TaskId, ushort data0 = 0, ushort data1 = 0)
        {

            EnableTask();
        }
        public void EnableTask()
        {

        }

        public bool CloseConnection()
        {

            try
            {
                Client.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
