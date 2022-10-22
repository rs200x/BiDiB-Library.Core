using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;
using Version = org.bidib.netbidibc.core.Models.Common.Version;

namespace org.bidib.netbidibc.core.Models.Manufacturers
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = Namespaces.ManufacturersNamespaceUrl, TypeName = "ManufacturersListVersionType")]
    [XmlRoot(Namespace = Namespaces.ManufacturersNamespaceUrl, IsNullable = false, ElementName = "Version")]
    public class ManufacturersListVersion : Version
    {
        [XmlAttribute("nmraListDate", DataType = XmlDataTypes.Date)]
        public DateTime ListDate { get; set; }
    }
}