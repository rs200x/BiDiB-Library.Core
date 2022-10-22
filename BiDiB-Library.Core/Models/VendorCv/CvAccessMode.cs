using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.VendorCv
{

    [Serializable]
    [XmlType(Namespace = Namespaces.VendorCvNamespaceUrl)]
    public enum CvAccessMode
    {
        [XmlEnum("rw")]
        ReadWrite,
        [XmlEnum("ro")]
        ReadOnly,
        [XmlEnum("wo")]
        WriteOnly,
        [XmlEnum("w")]
        Write,
        [XmlEnum("h")]
        Hidden
    }
}