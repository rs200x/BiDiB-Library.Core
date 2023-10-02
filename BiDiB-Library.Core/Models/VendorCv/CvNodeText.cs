using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
public class CvNodeText
{
    [XmlAttribute]
    public string Lang { get; set; }


    [XmlAttribute]
    public string Text { get; set; }


    [XmlAttribute]
    public string Help { get; set; }
}
