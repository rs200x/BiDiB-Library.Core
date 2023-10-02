using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "SpecialValueType")]
[XmlRoot("specialValue", Namespace = Namespaces.CommonTypesNamespaceUrl, IsNullable = false)]
public class SpecialValue
{
    [XmlElement("description")]
    public Description[] Descriptions { get; set; }

    [XmlAttribute("value")]
    public long Value { get; set; }

    [XmlAttribute("substitute")]
    public string Substitute { get; set; }
}