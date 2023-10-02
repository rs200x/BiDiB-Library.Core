using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.VendorCvNamespaceUrl, TypeName = "CVType")]
public class Cv : CvBase<string>
{
    [XmlElement("Description")]
    public CvDescription[] Description { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlElement("Bitdescription")]
    public CvBitDescription[] BitDescription { get; set; }

    [XmlElement("Bit")]
    public CvBitDescription[] Bit { get; set; }

    [XmlElement]
    public CvRadioDescription[] Radio { get; set; }

    [XmlAttribute]
    public string Number { get; set; }

    [XmlAttribute]
    public CvDataType Type { get; set; }

    [XmlAttribute]
    public string Min { get; set; }

    [XmlAttribute]
    public string Max { get; set; }

    [XmlAttribute]
    public string Low { get; set; }

    [XmlAttribute]
    public string High { get; set; }

    [XmlAttribute]
    public string Next { get; set; }

    [XmlAttribute]
    public string Values { get; set; }

    [XmlAttribute]
    public CvAccessMode Mode { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlAttribute("Rebootneeded")]
    public bool RebootNeeded { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlAttribute("Radiovalues")]
    public string RadioValues { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlAttribute("Radiobits")]
    public int RadioBits { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlAttribute("Lowbits")]
    public int LowBits { get; set; }

    // ReSharper disable once StringLiteralTypo
    [XmlAttribute("Highbits")]
    public int HighBits { get; set; }

    [XmlAttribute]
    public string RadioGroups { get; set; }

    [XmlAttribute]
    public string Keyword { get; set; }

    [XmlAttribute]
    public string DefaultValue { get; set; }

    [XmlAttribute]
    public string Regex { get; set; }

    [XmlIgnore]
    public bool HasChanges => !Value.Equals(NewValue, StringComparison.CurrentCultureIgnoreCase);

    public override string ToString() => $"N:{Number} T:{Type} V:{Value} NV:{NewValue} K:{Keyword}";
}