using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Common
{
    /// <summary>
    /// Detection type class
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "DetectionType")]
    public class Detection
    {
        [XmlElement("cv", typeof(Cv))]
        [XmlElement("cvGroup", typeof(CvGroup))]
        [XmlElement("conditions", typeof(Conditions))]
        public object[] Items { get; set; }

        [XmlAttribute("type")]
        public DetectionType Type { get; set; }

        [XmlAttribute(DataType = XmlDataTypes.Token)]
        public string DisplayFormat { get; set; }
    }
}
