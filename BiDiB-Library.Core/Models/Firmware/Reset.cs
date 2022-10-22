using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;

namespace org.bidib.netbidibc.core.Models.Firmware
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "ResetType")]
    public class Reset
    {
        [XmlElement("Description")]
        public Description[] Description { get; set; }

        [XmlAttribute("CV")]
        public ushort Cv { get; set; }

        [XmlAttribute]
        public byte Value { get; set; }
    }
}