using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
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
}