using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{ 
    [Serializable]
    [XmlType("FilenameType", Namespace = Namespaces.FirmwareNamespaceUrl)]
    public class DefaultLabelFile
    {
        [XmlAttribute("Lang")]
        public string Lang { get; set; }

        [XmlAttribute("Filename")]
        public string Filename { get; set; }
    }
}