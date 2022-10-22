using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    [Serializable]
    [DebuggerStepThrough]
    public class CvNode
    {
        // ReSharper disable once StringLiteralTypo
        [XmlElement("Nodetext")]
        public CvNodeText[] NodeText { get; set; }

        [XmlElement("Description")]
        public CvDescription[] Description { get; set; }

        [XmlElement("CV", typeof(Cv))]
        [XmlElement("Node", typeof(CvNode))]
        [XmlElement("Repeater", typeof(Repeater))]
        [XmlElement("CVRef", typeof(CvReference))]
        public object[] Items { get; set; }

        [XmlAttribute]
        public int Offset { get; set; }

        [XmlAttribute]
        public string Template { get; set; }

        [XmlAttribute]
        public int Count { get; set; }

        [XmlAttribute]
        public int Next { get; set; }

        [XmlAttribute]
        public string Comment { get; set; }

        //[XmlIgnore]
        public CvNode[] Nodes { get; set; } = Array.Empty<CvNode>();

        //[XmlIgnore]
        public Cv[] CVs { get; set; } = Array.Empty<Cv>();

        [XmlIgnore]
        public CvReference[] CvReferences { get; set; }
    }
}