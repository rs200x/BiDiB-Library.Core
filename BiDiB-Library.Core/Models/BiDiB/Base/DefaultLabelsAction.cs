using System;
using System.Xml.Serialization;

namespace org.bidib.Net.Core.Models.BiDiB.Base;

[Serializable]
[XmlType("DefaultLabelsActionType", Namespace = Namespaces.BiDiBBase10NamespaceUrl)]
public enum DefaultLabelsAction
{
    [XmlEnum("UNKNOWN")]
    Unknown,

    [XmlEnum("APPLIED")]
    Applied,

    [XmlEnum("IGNORED")]
    Ignored
}