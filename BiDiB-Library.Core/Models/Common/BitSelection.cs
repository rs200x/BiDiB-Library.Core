using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "BitSelectionType")]
public class BitSelection : CvItem
{
    public override CvItemType Type => CvItemType.BitSelection;
    
    [XmlElement("option")]
    public Option[] Options { get; set; }

    [XmlAttribute("number")]
    public byte Number { get; set; }
}