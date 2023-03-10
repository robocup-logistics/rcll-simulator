using System.Collections.Generic;
using Opc.UaFx;
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
            var deviceNode = new OpcFolderNode(new OpcName("DeviceSet", ns));
            references.Add(deviceNode, objectsNode);
            ns = Namespaces[2].Index;
            var CPXNode = new OpcFolderNode(deviceNode, new OpcName("CPX-E-CEC-C1-PN", ns)); //changed between "CPX-E-CEC-C1-PN" and "CODESYS Control Win V3" "CODESYS Control Win V3 x64"
            var resNode = new OpcFolderNode(CPXNode, new OpcName("Resources", ns));
            var AppNode = new OpcFolderNode(resNode, new OpcName("Application", ns));
            ns = Namespaces[1].Index;
            var GlobNode = new OpcFolderNode(AppNode, new OpcName("GlobalVars", ns));
            ns = Namespaces[2].Index;
            var GNode = new OpcFolderNode(GlobNode, new OpcName("G", ns));
            var In = new OpcFolderNode(GNode, new OpcName("In", ns));
            var Basic = new OpcFolderNode(GNode, new OpcName("Basic", ns));
            
            //Adding the references to correct the naming convention
            references.Add(CPXNode, deviceNode.Id);
            references.Add(resNode, CPXNode.Id);
            references.Add(AppNode, resNode.Id);
            references.Add(GlobNode, AppNode.Id);
            references.Add(GNode, GlobNode.Id);
            references.Add(In, GNode.Id);
            references.Add(Basic, GNode.Id);

            //create the local variables for the communication
            var In_p = new OpcFolderNode(In, new OpcName("p", ns));
            var Ba_p = new OpcFolderNode(Basic, new OpcName("p", ns));
            references.Add(In_p, In.Id);
            references.Add(Ba_p, Basic.Id);
            BasicNodes = new NodeCollection(Ba_p, ns, references);
            InNodes = new NodeCollection(In_p, ns, references);
            
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
        public NodeCollection(OpcFolderNode Parent, int ns, OpcNodeReferenceCollection reference)
        {
            //ActionId = new OpcNode();
            ParentNode = Parent;
            ActionId = new OpcDataVariableNode<ushort>(Parent, new OpcName("ActionId", ns), 0);
            
            //ActionId 
            //OpcText()
            BarCode = new OpcDataVariableNode<uint>(Parent, new OpcName("BarCode", ns), (uint)0);
            Data = new OpcDataVariableNode<ushort>(Parent, new OpcName("Data", ns), 0);
            Data0 = new OpcDataVariableNode<ushort>(Data, new OpcName("Data[0]", ns), 0);
            Data1 = new OpcDataVariableNode<ushort>(Data, new OpcName("Data[1]", ns), 0);
            
            SlideCnt = new OpcDataVariableNode<ushort>(Parent, new OpcName("SlideCnt", ns), 0);
            ByteError = new OpcDataVariableNode<byte>(Parent, new OpcName("Error", ns), 0);
            Status = new OpcFolderNode(Parent, new OpcName("Status", ns));
            // todo add Dimensions with value 1 (uint32); add IndexMax with value 1 (uint32) and IndexMin with value 0 (uint32) 
            reference.Add(ActionId, Parent.Id);
            reference.Add(BarCode, Parent.Id);
            reference.Add(Data, Parent.Id);
            reference.Add(Data0, Data.Id);
            reference.Add(Data1, Data.Id);
            reference.Add(SlideCnt, Parent.Id);
            reference.Add(ByteError, Parent.Id);
            reference.Add(Status, Parent.Id);
            StatusNodes = new Status(Status, ns, reference);
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

        public Status(OpcFolderNode Parent, int ns, OpcNodeReferenceCollection reference)
        {
            busy = new OpcDataVariableNode<bool>(Parent, new OpcName("Busy", ns), false);
            ready = new OpcDataVariableNode<bool>(Parent, new OpcName("Ready", ns), false);
            error = new OpcDataVariableNode<bool>(Parent, new OpcName("Error", ns), false);
            enable = new OpcDataVariableNode<bool>(Parent, new OpcName("Enable", ns), false);
            unused0 = new OpcDataVariableNode<bool>(Parent, new OpcName("unused0", ns), false);
            unused1 = new OpcDataVariableNode<bool>(Parent, new OpcName("unused1", ns), false);
            inSensor = new OpcDataVariableNode<bool>(Parent, new OpcName("inSensor", ns), false);
            outSensor = new OpcDataVariableNode<bool>(Parent, new OpcName("outSensor", ns), false);
            reference.Add(busy, Parent.Id);
            reference.Add(ready, Parent.Id);
            reference.Add(error, Parent.Id);
            reference.Add(enable, Parent.Id);
            reference.Add(unused0, Parent.Id);
            reference.Add(unused1, Parent.Id);
            reference.Add(inSensor, Parent.Id);
            reference.Add(outSensor, Parent.Id);
        }
    }
}
