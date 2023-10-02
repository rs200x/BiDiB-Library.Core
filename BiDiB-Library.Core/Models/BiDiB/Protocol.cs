using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl, TypeName = "protocol")]
public class Protocol
{
    [XmlArray("messageTypes")]
    [XmlArrayItem("messageType", IsNullable = false)]
    public MessageType[] MessageTypes { get; set; }

    [XmlArray("featureCodes")]
    [XmlArrayItem("featureCode", IsNullable = false)]
    public FeatureCode[] FeatureCodes { get; set; }

    [XmlAttribute("version")]
    public decimal Version { get; set; }
}