using System;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ProtocolTypeType")]
public enum ProtocolType
{
    [XmlEnum("dcc")]
    DCC,
    [XmlEnum("mm")]
    MM,
    [XmlEnum("mfx")]
    MFX,
    [XmlEnum("sx")]
    SX,
    [XmlEnum("sx2")]
    SX2,
    [XmlEnum("susi")]
    Susi
}