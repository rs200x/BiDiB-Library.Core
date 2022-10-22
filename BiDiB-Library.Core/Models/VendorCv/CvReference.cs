using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{
    [Serializable]
    [DebuggerStepThrough]
    public class CvReference
    {
        [XmlAttribute]
        public string Number { get; set; }

        [XmlAttribute]
        public string Values { get; set; }

    }
}