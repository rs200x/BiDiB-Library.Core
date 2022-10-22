using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    [Serializable]
    [XmlType("FirmwareDefinitionType", Namespace = Namespaces.FirmwareNamespaceUrl)]
    public class FirmwareDefinition
    {
        [XmlAttribute("Version")]
        public string Version { get; set; }
        
        [XmlAttribute("ProtocolVersion")]
        public string ProtocolVersion { get; set; }
        
        [XmlAttribute("RequiredMinVersion")]
        public string RequiredMinVersion { get; set; }

        [XmlAttribute("Status")]
        public FirmwareStatus Status { get; set; }

        [XmlElement("Node")]
        public Node[] Nodes { get; set; }

        [XmlElement("CvFilename")]
        public string[] CvFilename { get; set; }

        [XmlElement("Changelog")]
        public string Changelog { get; set; }
    }
}