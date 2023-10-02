using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVGroupTypeType")]
public enum CvGroupType
{
    // fallback value if unknown type defined
    [XmlEnum("list")]
    List,

    [XmlEnum("dccLongAddr")]
    DccLongAddr,
    [XmlEnum("dccSpeedCurve")]
    DccSpeedCurve,
    [XmlEnum("dccAccAddr")]
    DccAccAddr,
    [XmlEnum("dccAddrRG")]
    DccAddrRg,
    [XmlEnum("dccLongConsist")]
    DccLongConsist,
    [XmlEnum("int")]
    Int,
    [XmlEnum("long")]
    Long,
    [XmlEnum("string")]
    String,
    [XmlEnum("matrix")]
    Matrix,
    [XmlEnum("rgbColor")]
    RgbColor,
    [XmlEnum("centesimalInt")]
    CentesimalInt,
}