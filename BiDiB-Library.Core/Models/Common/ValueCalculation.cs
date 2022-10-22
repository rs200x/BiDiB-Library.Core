using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Class representing value calculation object
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ValueCalculationType")]
    [XmlRoot("valueCalculation", Namespace = Namespaces.CommonTypesNamespaceUrl, IsNullable = false)]
    public class ValueCalculation
    {
        [XmlAttribute("unit")]
        public UnitType Unit { get; set; }

        [XmlAttribute("digits")]
        public int Digits { get; set; }

        [XmlElement("item")]
        public ValueCalculationItem[] Items { get; set; }

        [XmlElement("specialValue")]
        public SpecialValue[] SpecialValues { get; set; }
    }
}
