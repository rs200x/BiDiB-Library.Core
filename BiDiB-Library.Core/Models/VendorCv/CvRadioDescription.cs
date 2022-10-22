using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [DebuggerStepThrough]
    public class CvRadioDescription
    {
        public static string DefaultGroupName = "G1";

    
        [XmlAttribute]
        public byte Value { get; set; }

        [XmlAttribute]
        public string Group { get; set; }

        [XmlElement("Description")]
        public CvDescription[] Description { get; set; }
    }
}