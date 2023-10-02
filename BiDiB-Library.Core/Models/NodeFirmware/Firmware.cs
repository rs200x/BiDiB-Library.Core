using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.NodeFirmware;

[Serializable]
[XmlType(AnonymousType = true, Namespace = Namespaces.FirmwareNamespaceUrl)]
[XmlRoot(Namespace = Namespaces.FirmwareNamespaceUrl, IsNullable = false)]
public class Firmware
{
    public Firmware()
    {
        Id = Guid.NewGuid();
    }

    [XmlIgnore]
    public Guid Id { get; }

    public VersionInfo Version { get; set; }

    public FirmwareDefinition FirmwareDefinition { get; set; }

    [XmlIgnore]
    public string FileName { get; set; }

    [XmlIgnore]
    public string Sha1 { get; set; }

    public override string ToString()
    {
        return $"{Version.Vendor} {Version.Pid} {FirmwareDefinition.Version} {FirmwareDefinition.Status}";

    }
}