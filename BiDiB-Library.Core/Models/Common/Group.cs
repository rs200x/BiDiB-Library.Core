using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "GroupType")]
public class Group : ICvItem
{
    public CvItemType Type => CvItemType.Group;
    
    [XmlElement("description")]
    public Description[] Descriptions { get; set; }

    [XmlElement("option")]
    public Option[] Options { get; set; }

    [XmlAttribute("number")]
    public byte Number { get; set; }
}