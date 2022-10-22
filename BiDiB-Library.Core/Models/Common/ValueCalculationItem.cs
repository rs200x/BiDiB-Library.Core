using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Class representing single item of value calculation
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ItemType")]
    public class ValueCalculationItem
    {
        [XmlElement("item")]
        public ValueCalculationItem[] Items { get; set; }

        [XmlAttribute("type")]
        public ValueCalculationItemType Type { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("number")]
        public ushort Number { get; set; }

        [XmlAttribute("indexLow")]
        public byte IndexLow { get; set; }

        [XmlAttribute("indexHigh")]
        public byte IndexHigh { get; set; }
    }
}
