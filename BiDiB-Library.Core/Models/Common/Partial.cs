using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

/// <summary>
/// Class representing partial cv value object
/// </summary>
[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "PartialType")]
public class Partial : ICvItem
{
    public CvItemType Type => CvItemType.Partial;
    
    [XmlElement("valueCalculation")]
    public ValueCalculation ValueCalculation { get; set; }

    [XmlElement("description")]
    public Description[] Descriptions { get; set; }

    [XmlAttribute("number")]
    public ushort Number { get; set; }

    [XmlAttribute("possibleValues")]
    public string PossibleValues { get; set; }

    [XmlAttribute("reset")]
    public bool Reset { get; set; }

    [XmlAttribute("disableOther")]
    public bool DisableOther { get; set; }

    [XmlAttribute("multiply")]
    public byte Multiply { get; set; }
}