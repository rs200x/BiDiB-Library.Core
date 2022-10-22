using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Decoder
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderDetectionNamespaceUrl, TypeName = "ManufacturerType")]
    public class Manufacturer
    {
        [XmlElement("detection")]
        public Detection[] Detection { get; set; }

        [XmlAttribute("id")]
        public byte Id { get; set; }

        [XmlAttribute("extendedId")]
        public ushort ExtendedId { get; set; }

        [XmlAttribute("name", DataType = XmlDataTypes.Token)]
        public string Name { get; set; }

        [XmlAttribute("shortName", DataType = XmlDataTypes.Token)]
        public string ShortName { get; set; }
    }
}