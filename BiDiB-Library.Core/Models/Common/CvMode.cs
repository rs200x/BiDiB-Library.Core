using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVModeType")]
public enum CvMode
{
    [XmlEnum("rw")]
    ReadWrite,
    [XmlEnum("ro")]
    ReadOnly,
    [XmlEnum("wo")]
    WriteOnly
}