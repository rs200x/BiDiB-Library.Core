using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ConditionsType")]
public class Conditions
{
    [XmlElement("trigger")]
    public Trigger[] Triggers { get; set; }
}