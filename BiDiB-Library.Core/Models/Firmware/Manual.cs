using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Firmware
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "ManualType")]
    public class Manual
    {
        [XmlAttribute("name", DataType = XmlDataTypes.Token)]
        public string Name { get; set; }

        [XmlAttribute("src", DataType = XmlDataTypes.AnyUri)]
        public string Source { get; set; }

        [XmlAttribute("lastModified")]
        public string LastModifiedString
        {
            get => LastModified.ToString("s");
            set => LastModified = DateTime.Parse(value);
        }

        [XmlIgnore]
        public DateTime LastModified { get; set; }

        [XmlAttribute("copyright")]
        public string Copyright { get; set; }

        [XmlAttribute("sha1", DataType = XmlDataTypes.Token)]
        public string Sha1 { get; set; }

        [XmlAttribute("fileSize")]
        public int FileSize { get; set; }
    }
}