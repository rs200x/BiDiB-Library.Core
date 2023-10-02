using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "DetectionTypeType")]
public enum DetectionType
{
    [XmlEnum("manufacturerId")]
    ManufacturerId,
    [XmlEnum("manufacturerExtendedId")]
    ManufacturerExtendedId,
    [XmlEnum("decoderId")]
    DecoderId,
    [XmlEnum("firmwareVersion")]
    FirmwareVersion,
    [XmlEnum("serialNumber")]
    SerialNumber
}