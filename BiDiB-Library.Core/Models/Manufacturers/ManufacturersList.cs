using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Manufacturers
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.ManufacturersNamespaceUrl)]
    [XmlRoot("manufacturersList", Namespace = Namespaces.ManufacturersNamespaceUrl, IsNullable = false)]
    public class ManufacturersList
    {
        [XmlElement("version")]
        public ManufacturersListVersion Version { get; set; }

        [XmlArray("manufacturers")]
        [XmlArrayItem("manufacturer", IsNullable = false)]
        public Manufacturer[] Manufacturers { get; set; }

        [XmlAttribute("decoderDBLink", DataType = XmlDataTypes.AnyUri)]
        public string DecoderDbLink { get; set; }
    }
}