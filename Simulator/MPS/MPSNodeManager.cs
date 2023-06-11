using System.Collections.Generic;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using Opc.UaFx.Server;
using Simulator.Utility;

namespace Simulator.MPS
{
    class MPSNodeManager : OpcNodeManager
    {
        private readonly MyLogger MyLogger;
        public MPSNodeManager(string defaultnamespace, string[] namespaces, MyLogger Logger)
            : base(defaultnamespace, namespaces)
        {
            MyLogger = Logger;
        }

        private readonly string[] bits = { "Busy", "Ready", "Error", "Enable", "unused0", "unused1", "inSensor", "outSensor" };
        private readonly string[] proto = { "ActionId", "BarCode", "Data", "Error", "SlideCnt", "Status" };
        private readonly int[] type = { 0, 0, 0, 1, 0, 2 }; /// 0 = ushort, 1 = byte 2 = folder
        public NodeCollection InNodes;
        public NodeCollection BasicNodes;


        //MyLogger.Log("Starting with Organizing the Nodes of our server for level!");
        // Define custom root node.
        /*
         * "Objects",
         * "2:DeviceSet",
         * "4:CPX-E-CEC-C1-PN",
         * "4:Resources",
         * "4:Application",
         * "3:GlobalVars",
         * "4:G",
         * "4:In"
         *
         *
         * "ACTION_ID_IN",
         * "BARCODE_IN",
         * "DATA_IN",
         * "ERROR_IN",
         * "SLIDECOUNT_IN",
         * "STATUS_BUSY_IN",
         * "STATUS_ENABLE_IN",
         * "STATUS_ERROR_IN",
         * "STATUS_READY_IN",
         *
         */
        protected override IEnumerable<IOpcNode> CreateNodes(OpcNodeReferenceCollection references)
        {
            var objectsNode = OpcObjectTypes.ObjectsFolder;
            var ns = DefaultNamespaceIndex;
            // Creating the basic structure of the OPC UA communication with the refbox
            var nodeId = new OpcNodeId(5001, ns);
            var deviceNode = new OpcObjectNode(new OpcName("DeviceSet",this.DefaultNamespace),nodeId);
            references.Add(deviceNode, objectsNode);
            ns = Namespaces[2].Index;
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN", ns);
            var CPXNode = new OpcObjectNode(deviceNode, new OpcName("CPX-E-CEC-C1-PN", ns), nodeId); //changed between "CPX-E-CEC-C1-PN" and "CODESYS Control Win V3" "CODESYS Control Win V3 x64"
            nodeId = new OpcNodeId(1001, ns);
            var resNode = new OpcObjectNode(CPXNode, new OpcName("Resources", ns),nodeId);
            
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application", ns);
            var AppNode = new OpcObjectNode(resNode, new OpcName("Application", ns),nodeId);
            
            ns = Namespaces[1].Index;
            nodeId = new OpcNodeId("|appo|CPX-E-CEC-C1-PN.Application.GlobalVars", ns);
            var GlobNode = new OpcObjectNode(AppNode, new OpcName("GlobalVars", ns), nodeId);
            
            ns = Namespaces[2].Index;
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application.G", ns);
            var GNode = new OpcFolderNode(GlobNode, new OpcName("G", ns),nodeId);
            
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application.G.In", ns);
            var In = new OpcObjectNode(GNode, new OpcName("In", ns), nodeId);
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application.G.Basic", ns);
            var Basic = new OpcObjectNode(GNode, new OpcName("Basic", ns), nodeId);

            //create the local variables for the communication
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application.G.In.p", ns);
            var In_p = new OpcFolderNode(In, new OpcName("p", ns), nodeId);
            nodeId = new OpcNodeId("|var|CPX-E-CEC-C1-PN.Application.G.Basic.p", ns);
            var Ba_p = new OpcFolderNode(Basic, new OpcName("p", ns), nodeId);
            BasicNodes = new NodeCollection(Ba_p, ns, "Basic", references);
            InNodes = new NodeCollection(In_p, ns, "In", references);
            
            // Yielding of the nodes
            yield return GlobNode;
            yield return deviceNode;
            yield return CPXNode;
            yield return resNode;
            yield return AppNode;
            yield return GNode;
            yield return In;
            yield return Basic;
            yield return In_p;
            yield return Ba_p;
            // yield the status nodes of in
            yield return InNodes.StatusNodes.busy;
            yield return InNodes.StatusNodes.ready;
            yield return InNodes.StatusNodes.error;
            yield return InNodes.StatusNodes.enable;
            yield return InNodes.StatusNodes.unused0;
            yield return InNodes.StatusNodes.unused1;
            yield return InNodes.StatusNodes.inSensor;
            yield return InNodes.StatusNodes.outSensor;
            // yield the rest of in
            yield return InNodes.ActionId;
            yield return InNodes.BarCode;
            yield return InNodes.Data0;
            yield return InNodes.Data1;
            yield return InNodes.Data;
            yield return InNodes.SlideCnt;
            yield return InNodes.ByteError;
            yield return InNodes.Status;
            // yield the status nodes of in
            yield return BasicNodes.StatusNodes.busy;
            yield return BasicNodes.StatusNodes.ready;
            yield return BasicNodes.StatusNodes.error;
            yield return BasicNodes.StatusNodes.enable;
            yield return BasicNodes.StatusNodes.unused0;
            yield return BasicNodes.StatusNodes.unused1;
            yield return BasicNodes.StatusNodes.inSensor;
            yield return BasicNodes.StatusNodes.outSensor;
            // yield the rest of in
            yield return BasicNodes.ActionId;
            yield return BasicNodes.BarCode;
            yield return BasicNodes.Data0;
            yield return BasicNodes.Data1;
            yield return BasicNodes.Data;
            yield return BasicNodes.SlideCnt;
            yield return BasicNodes.ByteError;
            yield return BasicNodes.Status;
        }
    }

    public class NodeCollection
    {
        public OpcDataVariableNode<ushort> ActionId;
        public OpcDataVariableNode<uint> BarCode;
        public OpcDataVariableNode<ushort> Data;
        public OpcDataVariableNode<ushort> Data0;
        public OpcDataVariableNode<ushort> Data1;
        public OpcDataVariableNode<ushort> SlideCnt;
        public OpcDataVariableNode<byte> ByteError;
        public OpcFolderNode ParentNode;
        public OpcFolderNode Status;
        public Status StatusNodes;
        public NodeCollection(OpcFolderNode Parent, int ns, string prefix, OpcNodeReferenceCollection reference)
        {
            //ActionId = new OpcNode();
            ParentNode = Parent;
            var path = "|var|CPX-E-CEC-C1-PN.Application.G." + prefix + ".p.";
            var name = "ActionId";
            var nodeId = new OpcNodeId(path + name, ns);
            ActionId = new OpcDataVariableNode<ushort>(ParentNode,new OpcName(name, ns), nodeId ,0);
            //reference.Add(ActionId, ParentNode.Id);
            //ActionId 
            //OpcText()
            name = "BarCode";
            nodeId = new OpcNodeId(path + name, ns);
            BarCode = new OpcDataVariableNode<uint>(ParentNode, new OpcName(name, ns), nodeId, (uint)0);
            name = "Data";
            nodeId = new OpcNodeId(path + name, ns);
            Data = new OpcDataVariableNode<ushort>(ParentNode, new OpcName(name, ns), nodeId, 0);
            name = "Data[0]";
            nodeId = new OpcNodeId(path + name, ns);
            Data0 = new OpcDataVariableNode<ushort>(Data, new OpcName(name, ns), nodeId, 0);
            name = "Data[1]";
            nodeId = new OpcNodeId(path + name, ns);
            Data1 = new OpcDataVariableNode<ushort>(Data, new OpcName(name, ns), nodeId,0);
            name = "SlideCnt";
            nodeId = new OpcNodeId(path + name, ns);
            SlideCnt = new OpcDataVariableNode<ushort>(ParentNode,  new OpcName(name, ns), nodeId, 0);
            name = "Error";
            nodeId = new OpcNodeId(path + name, ns);
            ByteError = new OpcDataVariableNode<byte>(ParentNode,  new OpcName(name, ns), nodeId, 0);
            name = "Status";
            nodeId = new OpcNodeId(path + name, ns);
            Status = new OpcFolderNode(ParentNode,  new OpcName(name, ns), nodeId);
            StatusNodes = new Status(Status, ns, prefix, reference);
        }
    }
    public class Status
    {
        public OpcDataVariableNode<bool> busy;
        public OpcDataVariableNode<bool> ready;
        public OpcDataVariableNode<bool> error;
        public OpcDataVariableNode<bool> enable;
        public OpcDataVariableNode<bool> unused0;
        public OpcDataVariableNode<bool> unused1;
        public OpcDataVariableNode<bool> inSensor;
        public OpcDataVariableNode<bool> outSensor;

        public Status(OpcFolderNode Parent, int ns, string prefix, OpcNodeReferenceCollection reference)
        {
            var path = "|var|CPX-E-CEC-C1-PN.Application.G." + prefix + ".p.Status.";
            var name = "Busy";
            var nodeId = new OpcNodeId(path + name, ns);
            busy = new OpcDataVariableNode<bool>(Parent, new OpcName(name, ns), nodeId, false);
            name = "Ready";
            nodeId = new OpcNodeId(path + name, ns);
            ready = new OpcDataVariableNode<bool>(Parent, new OpcName(name, ns), nodeId, false);
            name = "Error";
            nodeId = new OpcNodeId(path + name, ns);
            error = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            name = "Enable";
            nodeId = new OpcNodeId(path + name, ns);
            enable = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            name = "unused0";
            nodeId = new OpcNodeId(path + name, ns);
            unused0 = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            name = "unused1";
            nodeId = new OpcNodeId(path + name, ns);
            unused1 = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            name = "inSensor";
            nodeId = new OpcNodeId(path + name, ns);
            inSensor = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            name = "outSensor";
            nodeId = new OpcNodeId(path + name, ns);
            outSensor = new OpcDataVariableNode<bool>(Parent,  new OpcName(name, ns),nodeId, false);
            
        }
    }
}
