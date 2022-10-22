using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public class CvNodeText
    {
        private string helpField;

        private string langField;

        private string textField;

    
        [XmlAttribute]
        public string Lang
        {
            get { return langField; }
            set { langField = value; }
        }

    
        [XmlAttribute]
        public string Text
        {
            get { return textField; }
            set { textField = value; }
        }

    
        [XmlAttribute]
        public string Help
        {
            get { return helpField; }
            set { helpField = value; }
        }
    }
}