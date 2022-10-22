using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "SpecialValueType")]
    [XmlRoot("specialValue", Namespace = Namespaces.CommonTypesNamespaceUrl, IsNullable = false)]
    public class SpecialValue
    {
        [XmlAttribute("value")]
        public long Value { get; set; }

        [XmlAttribute("substitute")]
        public string Substitute { get; set; }
    }
}