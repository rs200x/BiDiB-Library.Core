using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Product
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderNamespaceUrl, TypeName = "DimensionsType")]
    public class Dimensions
    {
        [XmlAttribute("length")]
        public decimal Length { get; set; }

        [XmlAttribute("width")]
        public decimal Width { get; set; }

        [XmlAttribute("height")]
        public decimal Height { get; set; }
    }
}