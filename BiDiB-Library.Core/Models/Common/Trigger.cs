using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "TriggerType")]
    public class Trigger
    {
        [XmlElement("condition")]
        public Condition[] Conditions { get; set; }
     
        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("target")]
        public string Target { get; set; }
    }
}