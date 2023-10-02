using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.BiDiB.Base;

namespace org.bidib.Net.Core.Models.BiDiB.Labels;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.Labels10NamespaceUrl)]
public class AccessoryLabel : BaseLabel
{
    [XmlElement("aspectLabel")]
    public BaseLabel[] AspectLabels { get; set; }
}