using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "OptionType")]
public class Option
{
    [XmlElement("description")]
    public Description[] Description { get; set; }

    [XmlAttribute("value")]
    public byte Value { get; set; }

    [XmlAttribute("reset")]
    public bool Reset { get; set; }

    [XmlAttribute("disableOther")]
    public bool DisableOther { get; set; }
}