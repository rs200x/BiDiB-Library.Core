using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    [Serializable]
    [XmlType(Namespace = Namespaces.FirmwareNamespaceUrl)]
    public class SimpleNode : Node { }
}