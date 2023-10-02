using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType("VersionInfoType", Namespace = Namespaces.FirmwareNamespaceUrl)]
public class VersionInfo
{
    [XmlAnyElement]
    public XmlElement Any { get; set; }

    [XmlAttribute]
    public string Version { get; set; }

    [XmlAttribute]
    public string Lastupdate { get; set; }

    public DateTime LastUpdateDate => !string.IsNullOrEmpty(Lastupdate)
        ? DateTime.ParseExact(Lastupdate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)
        : DateTime.MinValue;

    [XmlAttribute]
    public string Author { get; set; }

    [XmlAttribute]
    public string Pid { get; set; }

    [XmlAttribute]
    public string Vendor { get; set; }

    [XmlAttribute]
    public string Description { get; set; }
}