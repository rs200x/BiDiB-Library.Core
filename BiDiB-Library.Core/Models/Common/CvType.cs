using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
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
}