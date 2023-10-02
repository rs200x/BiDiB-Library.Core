using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
public class CvDescription
{
    [XmlAttribute]
    public string Lang { get; set; }


    [XmlAttribute]
    public string Text { get; set; }


    [XmlAttribute]
    public string Help { get; set; }

    [XmlAttribute("Genericterm")]
    public string GenericTerm { get; set; }
}
