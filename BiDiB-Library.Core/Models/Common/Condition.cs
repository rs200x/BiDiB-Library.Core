using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ConditionType")]
    public class Condition
    {
        [XmlElement("condition")]
        public Condition[] Conditions { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("operation")]
        public string Operation { get; set; }

        [XmlAttribute("cv")]
        public string Cv { get; set; }

        [XmlAttribute("indexHigh")]
        public byte IndexHigh { get; set; }

        [XmlAttribute("indexLow")]
        public byte IndexLow { get; set; }

        [XmlAttribute("selection")]
        public string Selection { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}