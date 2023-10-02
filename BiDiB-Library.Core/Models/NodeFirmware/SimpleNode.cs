using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType(Namespace = Namespaces.FirmwareNamespaceUrl)]
public class SimpleNode : Node { }