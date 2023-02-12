using Opc.UaFx.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulatortests
{
    public class OPCTestHelper
    {
        public string NodePath = "ns=4;s=DeviceSet/CPX-E-CEC-C1-PN/Resources/Application/GlobalVars/G/In/p/";
        private readonly int Port;
        private readonly OpcClient Client;

        public OPCTestHelper(int port)
        {
            Port = port;
            Client = new OpcClient("opc.tcp://localhost:" + Port + "/");

        }
        public bool CreateConnection()
        {
            try
            {
                Client.Connect();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void SendTask(ushort TaskId, ushort data0 = 0, ushort data1 = 0)
        {
            Client.WriteNode(NodePath + "ActionId", TaskId);
            Client.WriteNode(NodePath + "Data/Data[0]", data0);
            Client.WriteNode(NodePath + "Data/Data[1]", data1);
            EnableTask();
        }
        public void EnableTask()
        {
            Client.WriteNode(NodePath + "Status/Enable", true);
        }

        public bool CloseConnection()
        {

            try
            {
                Client.Disconnect();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
