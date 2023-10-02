using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVTypeType")]
public enum CvType
{
    [XmlEnum("byte")]
    Byte,

    [XmlEnum("select")]
    Select,

    [XmlEnum("signedByte")]
    SignedByte
}