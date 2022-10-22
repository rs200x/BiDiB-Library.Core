using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;

namespace org.bidib.netbidibc.core.Models.Decoder
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderDetectionNamespaceUrl, TypeName = "DecoderDetectionProtocolType")]
    public class DecoderDetectionProtocol : Protocol
    {
        [XmlArray("default")]
        [XmlArrayItem("detection", IsNullable = false)]
        public Detection[] Default { get; set; }

        [XmlElement("manufacturer")]
        public Manufacturer[] Manufacturer { get; set; }
    }
}