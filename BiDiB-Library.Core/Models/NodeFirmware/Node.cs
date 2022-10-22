using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    [XmlInclude(typeof(FirmwareNode))]
    [XmlInclude(typeof(DeviceNode))]
    [XmlInclude(typeof(SimpleNode))]
    [Serializable]
    [XmlType("NodeType", Namespace = Namespaces.FirmwareNamespaceUrl)]
    public abstract class Node
    {
        [XmlElement("Nodetext")]
        public NodeText[] NodeText { get; set; }

        [XmlElement("Node")]
        public Node[] Nodes { get; set; }

        [XmlAttribute]
        public string Comment { get; set; }
    }
}