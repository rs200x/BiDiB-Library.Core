using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Product;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderNamespaceUrl, TypeName = "SpecificationsType")]
public class Specifications
{
    [XmlElement("dimensions")]
    public Dimensions Dimensions { get; set; }

    [XmlElement("electrical")]
    public Electrical Electrical { get; set; }

    [XmlElement("functionConnectors")]
    public FunctionConnectors FunctionConnectors { get; set; }

    [XmlElement("connectors")]
    public Connectors Connectors { get; set; }
}