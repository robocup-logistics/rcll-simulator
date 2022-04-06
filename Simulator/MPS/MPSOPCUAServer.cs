using System;
using System.Threading;
using Opc.UaFx;
using Opc.UaFx.Server;
using Opc.UaFx.Services;
using Simulator.Utility;

namespace Simulator.MPS
{
    public class MPSOPCUAServer
    {
        private readonly MyLogger MyLogger;
        private readonly string Name;
        private readonly int Port;
        private readonly string URL;
        private readonly OpcServer server;
        private readonly MPSNodeManager m1;
        private readonly ManualResetEvent WriteEvent;
        //private readonly static Dictionary<OpcNodeId, int> nodesPerSession = new Dictionary<OpcNodeId, int>();

        public MPSOPCUAServer(string name, int port, ManualResetEvent mre, MyLogger log)
        {
            MyLogger = log;
            Name = name;
            Port = port;
            URL = "opc.tcp://localhost:" + Convert.ToString(Port) + "/";
            MyLogger.Info("Created Mps " + Name + " with URL = " + URL);
            WriteEvent = mre;
            string[] Namespaces =
{
                "urn:DESKTOP-7TJKAL0:3S%20-%20Smart%20Software%20Solutions%20GmbH:CODESYS%20Control%20Win%20V3:OPCUA:Server",
                "http://PLCopen.org/OpcUa/IEC61131-3/",
                "CODESYSSPV3/3S/IecVarAccess"
            };
            m1 = new MPSNodeManager(Namespaces[1], Namespaces, MyLogger);
            server = new OpcServer(URL, m1);
            try
            {
                server.Address = new Uri(URL);
                server.Start();
            }
            catch (Exception e)
            {
                MyLogger.Log("Port already in use?" + e.ToString());
            }
        }

        public void Start()
        {
            //TODO change the start of the Mps OPCUA server to either not use the event handlers or use them differently
            
            //server.RequestProcessing += HandleRequestProcessing;
            // /*server.RequestProcessing += HandleRequestProcessing;
            // server.RequestValidating += HandleRequestValidating;
            // server.RequestValidated += HandleRequestValidated;
            // server.RequestProcessed += HandleRequestProcessed;*/
            server.RequestValidating += HandleRequestValidating;
            //server.RequestProcessed += HandleRequestProcessed;
            while (true)
            {
                //MyLogger.Info("Doing stuff with the Refbox!");
                //MyLogger.Info("The Refbox connector is still running!");

                Thread.Sleep(500);
            }
        }

        public NodeCollection GetNodeCollection(bool In)
        {
            return In ? m1.InNodes : m1.BasicNodes;
        }
        public void UpdateChanges(OpcNode node)
        {
            node.UpdateChanges(server.SystemContext, OpcNodeChanges.Value);
        }

        #region MessageHandler
        private void HandleRequestProcessing(object sender, OpcRequestProcessingEventArgs e)
        {
            const string prefix = "[HRP] ";
            //MyLogger.Log("------------------------------------------------");
            MyLogger.Log(prefix + "Processing: " + e.Request + " type = [" + e.Request.GetType().FullName + "]");

            switch (e.Request.ToString())
            {
                case "Read":
                    MyLogger.Log(prefix + "Trying to Read from the Server!");
                    break;
                case "Write":
                    MyLogger.Log(prefix + "This is a Write Request");
                    break;
                case "Publish":
                    MyLogger.Log(prefix + "This is a Publish Request");
                    break;
                case "CreateSessionRequest":
                    MyLogger.Log(prefix + "This is a CreateSessionRequest");
                    break;
                case "GetEndpointsRequest":
                    MyLogger.Log(prefix + "This is a GetEndpointsRequest");
                    break;
                case "BrowseRequest":
                    MyLogger.Log(prefix + "This is a BrowseRequest");
                    break;
                case "ActivateSessionRequest":
                    MyLogger.Log(prefix + "This is a ActivateSessionRequest");
                    break;
                case "ReadRequest":
                    MyLogger.Log(prefix + "This is a ReadRequest for the node ");
                    break;
                case "WriteRequest":
                    MyLogger.Log(prefix + "This is a WriteRequest");
                    break;
                default:
                    MyLogger.Log(prefix + "Not a known Request! {" + e.Request + "}");
                    break;
            }

        }

        /// <summary>
        /// Function to wake up sleeping machines when an update from the refbox is sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRequestValidating(object sender, OpcRequestValidatingEventArgs e)
        {
            if (e.RequestType != OpcRequestType.Write)
            {
                return;
            }
            var Request = (OpcWriteNodesRequest)e.Request; 
            //MyLogger.Log("We got a write for [" + Request.Commands[0].NodeId + "] -> [" + Request.Commands[0].Value + "] on the port [" + Port + "] an we wake up the corresponding machine!");
            var nodeName = Request.Commands[0].NodeId.ToString();
            var nodeValue = Request.Commands[0].Value.ToString();
            if (!nodeName.Contains("Enable") || !nodeValue.Equals("True")) return;
            MyLogger.Log("Got a Enable with the following data : AiD[" + m1.InNodes.ActionId.Value + "] D0[" + m1.InNodes.Data0.Value + "] D1[" + m1.InNodes.Data1.Value + "]");
            WriteEvent.Set();
        }

        void HandleRequestValidated(object sender, OpcRequestValidatedEventArgs e)
        {
            const string prefix = "[HRValidated] ";
            MyLogger.Log(prefix + e + e.Request);
            if (e.RequestType.Equals(OpcRequestType.GetEndpoints))
            {
                MyLogger.Log(prefix + "Validating a Get Endpoints request!");
            }
            MyLogger.Log(prefix + " -> Validated");
        }


        private void HandleRequestProcessed(object sender, OpcRequestProcessedEventArgs e)
        {
            /*if (e.Request != OpcRequestType.Write)
            {
                return;
            }*/
            bool v = e.Request.GetType() == typeof(OpcWriteNodesRequest);
            if (!v) return;
            MyLogger.Log(e.Request.ToString());
            WriteEvent.Set();

            //WriteEvent.Set();

        }
#endregion
    }
}
