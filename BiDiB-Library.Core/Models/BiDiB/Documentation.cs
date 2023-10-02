using System;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType("DocumentationType", Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Documentation
{
    
    [XmlAttribute("language", DataType = XmlDataTypes.Language)]
    public string Language { get; set; }

    
    [XmlAttribute("text")]
    public string Text { get; set; }

    
    [XmlAttribute("description")]
    public string Description { get; set; }

    
    [XmlAttribute("url", DataType = XmlDataTypes.AnyUri)]
    public string Url { get; set; }
}