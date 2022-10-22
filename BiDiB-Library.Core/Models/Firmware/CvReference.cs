using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Firmware
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "IdReferenceType")]
    public class CvReference
    {
        [XmlAttribute("id", DataType = XmlDataTypes.Token)]
        public string Id { get; set; }

        [XmlAttribute("activeItems", DataType = XmlDataTypes.Token)]
        public string ActiveItems { get; set; }

        [XmlIgnore]
        public CvBase CvItem { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
            return $"ID: {Id}, AI: {ActiveItems}";
        }

        #endregion
    }
}