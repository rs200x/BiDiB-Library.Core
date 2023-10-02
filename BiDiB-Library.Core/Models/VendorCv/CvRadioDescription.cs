using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
public class CvRadioDescription
{
    public static readonly string DefaultGroupName = "G1";

    [XmlAttribute]
    public byte Value { get; set; }

    [XmlAttribute]
    public string Group { get; set; }

    [XmlElement("Description")]
    public CvDescription[] Description { get; set; }
}