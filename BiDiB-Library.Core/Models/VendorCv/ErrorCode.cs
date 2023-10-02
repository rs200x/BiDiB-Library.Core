using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.VendorCv;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
public class ErrorCode
{
    [XmlElement("Description")]
    public CvDescription[] Description { get; set; }

    [XmlAttribute("Errnum")]
    public int ErrorNumber { get; set; }
}