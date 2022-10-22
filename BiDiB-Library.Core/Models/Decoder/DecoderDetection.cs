using System;
using System.Diagnostics;
using System.Xml.Serialization;
using Version = org.bidib.netbidibc.core.Models.Common.Version;

namespace org.bidib.netbidibc.core.Models.Decoder
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.DecoderDetectionNamespaceUrl)]
    [XmlRoot("decoderDetection",Namespace = Namespaces.DecoderDetectionNamespaceUrl, IsNullable = false)]
    public class DecoderDetection
    {
        [XmlElement("version")]
        public Version Version { get; set; }

        [XmlArray("protocols")]
        [XmlArrayItem("protocol", IsNullable = false)]
        public DecoderDetectionProtocol[] Protocols { get; set; }
    }
}