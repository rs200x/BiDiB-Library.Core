using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "BitType")]
public class Bit
{
    [XmlElement("description")]
    public Description[] Description { get; set; }

    [XmlAttribute("number")]
    public byte Number { get; set; }
}