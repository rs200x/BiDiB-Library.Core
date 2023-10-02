using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Manufacturers;

[Serializable]
[DebuggerStepThrough]
[XmlType(AnonymousType = true, Namespace = Namespaces.ManufacturersNamespaceUrl, TypeName = "ManufacturersListVersionType")]
[XmlRoot(Namespace = Namespaces.ManufacturersNamespaceUrl, IsNullable = false, ElementName = "Version")]
public class ManufacturersListVersion : Common.Version
{
    [XmlAttribute("nmraListDate", DataType = XmlDataTypes.Date)]
    public DateTime ListDate { get; set; }
}