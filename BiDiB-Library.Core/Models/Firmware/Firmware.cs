using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "FirmwareType")]
public class Firmware
{
    [XmlArray("decoders")]
    [XmlArrayItem("decoder", IsNullable = false)]
    public Decoder[] Decoders { get; set; }

    [XmlArray("manuals")]
    [XmlArrayItem("manual", IsNullable = false)]
    public Manual[] Manuals { get; set; }

    [XmlArray("images", Namespace = Namespaces.CommonTypesNamespaceUrl)]
    [XmlArrayItem("image", IsNullable = false)]
    public Image[] Images { get; set; }

    [XmlArray("protocols")]
    [XmlArrayItem("protocol", IsNullable = false)]
    public FirmwareProtocol[] Protocols { get; set; }

    [XmlAttribute("version", DataType = XmlDataTypes.Token)]
    public string Version { get; set; }

    [XmlAttribute("versionExtension", DataType = XmlDataTypes.Token)]
    public string VersionExtension { get; set; }

    [XmlIgnore]
    public string FullVersionString => string.IsNullOrEmpty(VersionExtension) ? Version : $"{Version} ({VersionExtension})";

    [XmlAttribute("releaseDate", DataType = XmlDataTypes.Date)]
    public DateTime ReleaseDate { get; set; }

    [XmlAttribute("manufacturerId")]
    public byte ManufacturerId { get; set; }

    [XmlAttribute("manufacturerExtendedId")]
    public ushort ManufacturerExtendedId { get; set; }

    [XmlAttribute("manufacturerName", DataType = XmlDataTypes.Token)]
    public string ManufacturerName { get; set; }

    [XmlAttribute("manufacturerShortName", DataType = XmlDataTypes.Token)]
    public string ManufacturerShortName { get; set; }

    [XmlAttribute("manufacturerUrl", DataType = XmlDataTypes.AnyUri)]
    public string ManufacturerUrl { get; set; }

    [XmlAttribute("decoderDBLink", DataType = XmlDataTypes.AnyUri)]
    public string DecoderDbLink { get; set; }

    [XmlAttribute("options", DataType = XmlDataTypes.Token)]
    public string Options { get; set; }

    public override string ToString()
    {
        return $"{ManufacturerId}.{ManufacturerExtendedId} {FullVersionString}";
    }
}