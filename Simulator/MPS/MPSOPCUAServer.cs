using System;
using System.Threading;
using Opc.Ua;
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
        /// <summary></summary>
        private readonly MPSNodeManager NodeManager;
        private readonly ManualResetEvent BasicEvent;
        private readonly ManualResetEvent InEvent;
        //private readonly static Dictionary<OpcNodeId, int> nodesPerSession = new Dictionary<OpcNodeId, int>();
        private readonly string Prefix;
        private bool isMonitored;
        private bool Active;
        public bool inEnabled;
        
        public MPSOPCUAServer(string name, int port, ManualResetEvent basicEvent, ManualResetEvent inEvent, MyLogger log)
        {
            MyLogger = log;
            Name = name;
            Port = port;
            URL = "opc.tcp://localhost:" + Convert.ToString(Port) + "/";
            MyLogger.Info("Created Mps " + Name + " with URL = " + URL);
            BasicEvent = basicEvent;
            InEvent = inEvent;
            isMonitored = false;
            inEnabled = false;
            Prefix = String.Format("HRP on {0,-6}|", Name);
            string[] Namespaces =
            {
                "urn:DESKTOP-7TJKAL0:3S%20-%20Smart%20Software%20Solutions%20GmbH:CODESYS%20Control%20Win%20V3:OPCUA:Server",
                "http://PLCopen.org/OpcUa/IEC61131-3/",
                "CODESYSSPV3/3S/IecVarAccess"
            };
            NodeManager = new MPSNodeManager(Namespaces[1], Namespaces, MyLogger);
            //server.Configurations
            server = new OpcServer(URL, NodeManager);
            try
            {
                server.Address = new Uri(URL);
                Active = true;
                var config = server.Configuration.ServerConfiguration;
                
                server.Start();
            }
            catch (Exception e)
            {
                MyLogger.Log("Port already in use?" + e.ToString());
            }
        }

        ~MPSOPCUAServer()
        {
            MyLogger.Log("Closing the OPCUA Server for " + Name);
            server.Stop();
        }

        public void Start()
        {
            //TODO change the start of the Mps OPCUA server to either not use the event handlers or use them differently
            Console.WriteLine("Server " + Port  + " is Setup!");
            //server.RequestProcessing += HandleRequestProcessing;
            // /*server.RequestProcessing += HandleRequestProcessing;
            // server.RequestValidating += HandleRequestValidating;
            // server.RequestValidated += HandleRequestValidated;
            // server.RequestProcessed += HandleRequestProcessed;*/
            server.RequestValidating += HandleRequestValidating;
            //server.RequestProcessed += HandleRequestProcessed;

            while (Active)
            {
                /*if(NodeManager.InNodes.StatusNodes.busy.Value == true)
                {
                    NodeManager.InNodes.StatusNodes.busy.Value = true;
                    ApplyChanges(NodeManager.InNodes.StatusNodes.busy);
                }
                else
                {
                    NodeManager.InNodes.StatusNodes.busy.Value = false;
                    ApplyChanges(NodeManager.InNodes.StatusNodes.busy);
                }*/
                Thread.Sleep(10);
            }
        }

        public NodeCollection GetNodeCollection(bool In)
        {
            return In ? NodeManager.InNodes : NodeManager.BasicNodes;
        }
        public void ApplyChanges(OpcNode node)
        {
            node.ApplyChanges(server.SystemContext);
        }

        public void Stop()
        {
            Active = false;
        }

        #region MessageHandler
        private void HandleRequestProcessing(object sender, OpcRequestProcessingEventArgs e)
        {

          
            //MyLogger.Log("------------------------------------------------");
            //MyLogger.Log(prefix + "Processing: " + e.Request + " type = [" + e.Request.GetType().FullName + "]");
            switch (e.Request.ToString())
            {
                case "Read":
                    //MyLogger.Log(prefix + "Trying to Read from the Server!");
                    break;
                case "Write":
                    //MyLogger.Log(prefix + "This is a WriteRequest");
                    break;
                case "Publish":
                    //MyLogger.Log(prefix + "This is a Publish Request");
                    break;
                case "CreateSessionRequest":
                    Console.WriteLine(Prefix + "This is a CreateSessionRequest");
                    break;
                case "GetEndpointsRequest":
                    //MyLogger.Log(prefix + "This is a GetEndpointsRequest");
                    break;
                case "BrowseRequest":
                    //MyLogger.Log(prefix + "This is a BrowseRequest");
                    break;
                case "ActivateSessionRequest":
                    //MyLogger.Log(prefix + "This is a ActivateSessionRequest");
                    break;
                case "ReadRequest":
                    {
                        Console.WriteLine(Prefix + "This is a ReadRequest");
                        /*foreach(var com in Request.Commands)
                        {
                            Console.WriteLine(Prefix + com.ToString());
                        }*/
                    }

                    
                    break;
                case "WriteRequest":
                {
                    Console.WriteLine(Prefix + "WriteRequest on " + Name + "!");
                    var Request = (OpcWriteNodesRequest)e.Request;
                    Console.WriteLine(Prefix + Request.Header);
                    foreach (var com in Request.Commands)
                    {
                        Console.WriteLine(Prefix + com.ToString());
                    }
                }
                    break;
                case "TranslateBrowsePathsToNodeIdsRequest":
                    break;
                case "CreateMonitoredItemsRequest":
                {
                    Console.WriteLine(Prefix + " CreateMonitoredItemsRequest");
                    isMonitored = true;
                }
                    break;
                case "DeleteMonitoredItemsRequest":
                isMonitored = false;
                    break;
                case "CreateSubscriptionRequest":
                    break;
                case "PublishRequest":
                    break;
                case "CloseSessionRequest":
                    break;
                default:
                    Console.WriteLine(Prefix + "Not a known Request! {" + e.Request + "}");
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
            var parts = nodeName.Split("/");
            var nodeValue = Request.Commands[0].Value.ToString();
            switch (parts.Last())
            {
                case "Enable":
                    if (nodeValue.ToLower().Equals("true"))
                    {
                        MyLogger.Log(nodeName);
                        if (nodeName.ToLower().Contains("basic"))
                        {
                            MyLogger.Log("Got a Basic-Enable with the following data : AiD[" + NodeManager.BasicNodes.ActionId.Value + "] D0[" + NodeManager.BasicNodes.Data0.Value + "] D1[" + NodeManager.InNodes.Data1.Value + "]");
                            BasicEvent.Set();
                        }
                        if(nodeName.ToLower().Contains("in"))
                        {
                            MyLogger.Log("Got a In-Enable with the following data : AiD[" + NodeManager.InNodes.ActionId.Value + "] D0[" + NodeManager.InNodes.Data0.Value + "] D1[" + NodeManager.InNodes.Data1.Value + "]");
                            inEnabled = true;
                            InEvent.Set();
                        }
                    }
                    break;
                case "ActionId":
                    if(parts[^3].Equals("In"))
                        MyLogger.Log("Got a new Value for " + parts.Last() + " [" + nodeValue + "]");
                    break;
                default:
                    return;
            }
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
            //MyLogger.Log(e.Request.ToString());
            InEvent.Set();

            //WriteEvent.Set();

        }
#endregion
    }
}
