using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public class ErrorCode
    {
        private CvDescription[] descriptionField;

        private int errnumField;

    
        [XmlElement("Description")]
        public CvDescription[] Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

    
        [XmlAttribute]
        public int Errnum
        {
            get { return errnumField; }
            set { errnumField = value; }
        }
    }
}