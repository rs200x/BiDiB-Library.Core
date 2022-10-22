using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Enum for option values of CvGroup
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVGroupTypeOptions")]
    public enum CvGroupTypeOptions
    {
        [XmlEnum("bitList")]
        BitList,

        [XmlEnum("bitListSort")]
        BitListSort,

        [XmlEnum("greater")]
        Greater,

        [XmlEnum("greaterEqual")]
        GreaterEqual,

        [XmlEnum("less")]
        Less,

        [XmlEnum("lessEqual")]
        LessEqual,

        [XmlEnum("switchRowCol")]
        SwitchRowCol,

        [XmlEnum("reverseRow")]
        ReverseRow,

        [XmlEnum("reverseCol")]
        ReverseCol,
    }
}
