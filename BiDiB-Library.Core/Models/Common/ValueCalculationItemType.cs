using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Enum defining value calculation type
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ValueCalculationItemType")]
    public enum ValueCalculationItemType
    {
        [XmlEnum("self")]
        Self,

        [XmlEnum("operator")]
        Operator,

        [XmlEnum("constant")]
        Constant,

        [XmlEnum("cv")]
        CvValue,

        [XmlEnum("bracket")]
        Bracket
    }
}
