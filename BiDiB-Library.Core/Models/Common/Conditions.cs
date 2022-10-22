using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ConditionsType")]
    public class Conditions
    {
        [XmlElement("trigger")]
        public Trigger[] Triggers { get; set; }
    }
}