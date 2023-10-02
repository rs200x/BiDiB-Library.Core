using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.BiDiB.Base;

namespace org.bidib.Net.Core.Models.BiDiB.Labels;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.Labels10NamespaceUrl)]
public class PortLabel : BaseLabel
{
    [XmlAttribute("type")]
    public PortType Type { get; set; }
}