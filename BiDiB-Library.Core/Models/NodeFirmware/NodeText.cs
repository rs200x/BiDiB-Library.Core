using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{ 
    [Serializable]
    [XmlType("NodeTextType",Namespace = Namespaces.FirmwareNamespaceUrl)]
    public class NodeText
    {
        [XmlAttribute]
        public string Lang { get; set; }

        [XmlAttribute]
        public string Text { get; set; }
    }
}