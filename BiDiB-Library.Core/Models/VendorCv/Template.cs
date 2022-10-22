using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public class Template
    {
        [XmlElement("CV", typeof(Cv))]
        [XmlElement("Node", typeof(CvNode))]
        [XmlElement("Repeater", typeof(Repeater))]
        public object[] Items { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool SkipOnTimeout { get; set; }
    }
}