using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Manufacturers
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.ManufacturersNamespaceUrl, TypeName = "ManufacturerType")]
    public class Manufacturer
    {
        [XmlAttribute("id")]
        public byte Id { get; set; }

        [XmlAttribute("extendedId")]
        public ushort ExtendedId { get; set; }

        [XmlAttribute("name",DataType = XmlDataTypes.Token)]
        public string Name { get; set; }

        [XmlAttribute("shortName",DataType = XmlDataTypes.Token)]
        public string ShortName { get; set; }

        [XmlAttribute("url", DataType = XmlDataTypes.AnyUri)]
        public string Url { get; set; }

        [XmlAttribute("decoderDBLink", DataType = XmlDataTypes.AnyUri)]
        public string DecoderDbLink { get; set; }
        
        [XmlAttribute("country", DataType = XmlDataTypes.Token)]
        public string Country { get; set; }

        [XmlIgnore]
        public string CombinedId => ExtendedId != 0 ? $"{Id}.{ExtendedId}" : Id.ToString(CultureInfo.CurrentCulture);

        public override string ToString()
        {
            return $"{CombinedId} - {ShortName}";
        }
    }
}