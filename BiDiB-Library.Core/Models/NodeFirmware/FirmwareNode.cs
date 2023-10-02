using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType(Namespace = Namespaces.FirmwareNamespaceUrl)]
public class FirmwareNode : Node
{
    public string Filename { get; set; }

    [XmlAttribute]
    public int DestinationNumber { get; set; }

    [XmlIgnore]
    public List<byte[]> Data { get; set; }
}