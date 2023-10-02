using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
public class Repeater
{
    [XmlElement("CV")]
    public Cv[] Cvs { get; set; }

    [XmlElement("CVRef")]
    public CvReference[] CvReferences { get; set; }
    
    [XmlAttribute]
    public int Offset { get; set; }
    
    [XmlAttribute]
    public int Count { get; set; }
    
    [XmlAttribute]
    public int Next { get; set; }

    [XmlAttribute]
    public string Comment { get; set; }

    [XmlAttribute]
    public int Start { get; set; }
}