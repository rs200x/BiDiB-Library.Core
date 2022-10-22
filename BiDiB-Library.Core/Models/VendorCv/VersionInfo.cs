using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public class VersionInfo
    {
    [XmlAttribute]
        public string ReleaseDate { get; set; }

        [XmlAttribute]
        public string Created { get; set; }
    
        [XmlAttribute]
        public string Firmware { get; set; }
    
        [XmlAttribute]
        public string Categorie { get; set; }
    
        [XmlAttribute]
        public string CreatorLink { get; set; }
    
        [XmlAttribute]
        public string Createdby { get; set; }
    
        [XmlAttribute]
        public string Copyright { get; set; }
    
        [XmlAttribute]
        public string Vendor { get; set; }
    
        [XmlAttribute]
        public string Description { get; set; }
    
        [XmlAttribute]
        public string Version { get; set; }
    
        [XmlAttribute]
        public string Lastupdate { get; set; }

        public DateTime LastUpdateDate => !string.IsNullOrEmpty(Lastupdate)
            ? DateTime.ParseExact(Lastupdate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)
            : DateTime.MinValue;
    
        [XmlAttribute]
        public string Author { get; set; }
    
        [XmlAttribute]
        public string Pid { get; set; }
    
        [XmlAttribute]
        public string Url { get; set; }
    
        [XmlAttribute]
        public string POM { get; set; }
    }
}