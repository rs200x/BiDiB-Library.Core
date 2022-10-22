using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    [Serializable]
    [DebuggerStepThrough]
    public class CvBitDescription
    {
        [XmlAttribute]
        public int Bitnum { get; set; }

        [XmlAttribute]
        public int Number { get; set; }

        [XmlAttribute]
        public string Group { get; set; }

        [XmlAttribute]
        public string Lang { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string Help { get; set; }

        [XmlElement("Description")]
        public CvDescription[] Description { get; set; }
    }
}