using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;

namespace org.bidib.netbidibc.core.Models.Firmware
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "CategoryType")]
    public class Category
    {
        [XmlElement("description", Namespace = Namespaces.CommonTypesNamespaceUrl)]
        public Description[] Descriptions { get; set; }

        [XmlElement("idReference", typeof(CvReference))]
        [XmlElement("category", typeof(Category))]
        public object[] Items { get; set; }
    }
}