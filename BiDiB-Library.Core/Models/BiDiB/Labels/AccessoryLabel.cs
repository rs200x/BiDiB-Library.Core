using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.BiDiB.Base;

namespace org.bidib.netbidibc.core.Models.BiDiB.Labels
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.Labels10NamespaceUrl)]
    public class AccessoryLabel : BaseLabel
    {
        [XmlElement("aspectLabel")]
        public BaseLabel[] AspectLabels { get; set; }
    }
}