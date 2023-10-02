using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType("NodeTextType",Namespace = Namespaces.FirmwareNamespaceUrl)]
public class NodeText
{
    [XmlAttribute]
    public string Lang { get; set; }

    [XmlAttribute]
    public string Text { get; set; }
}