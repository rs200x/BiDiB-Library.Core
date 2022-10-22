using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Xml;

namespace org.bidib.netbidibc.core.Models.Firmware
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.DecoderFirmwareNamespaceUrl, TypeName = "FirmwareProtocolType")]
    public class FirmwareProtocol : Protocol
    {
        [XmlElement("cvChangelog")]
        public CvChangelog CvChangelog { get; set; }

        [XmlArray("decoderDetection")]
        [XmlArrayItem("detection", IsNullable = false)]
        public Detection[] DecoderDetection { get; set; }

        [XmlArray("resets")]
        [XmlArrayItem("reset", IsNullable = false)]
        public Reset[] Resets { get; set; }

        [XmlArray("cvs")]
        [XmlArrayItem("cv", typeof(Cv), IsNullable = false, Namespace = Namespaces.CommonTypesNamespaceUrl)]
        [XmlArrayItem("cvGroup", typeof(CvGroup), IsNullable = false, Namespace = Namespaces.CommonTypesNamespaceUrl)]
        public CvBase[] CVs { get; set; }

        [XmlArray("cvStructure")]
        [XmlArrayItem("category", IsNullable = false)]
        public Category[] CvStructure { get; set; }

        [XmlAttribute("speedSteps", DataType = XmlDataTypes.Token)]
        public string SpeedSteps { get; set; }

        [XmlAttribute("addresses", DataType = XmlDataTypes.Token)]
        public string Addresses { get; set; }

        [XmlAttribute("functions")]
        public byte Functions { get; set; }

        [XmlAttribute("progModes", DataType = XmlDataTypes.Token)]
        public string ProgModes { get; set; }

        [XmlAttribute("options", DataType = XmlDataTypes.Token)]
        public string Options { get; set; }

        [XmlAttribute("railCom", DataType = XmlDataTypes.Token)]
        public string RailCom { get; set; }

        public ICollection<Cv> GetAllCvs()
        {
            if (CVs == null)
            {
                return new List<Cv>();
            }

            List<Cv> allCvs = CVs.OfType<Cv>().ToList();
            allCvs.AddRange(CVs.OfType<CvGroup>().SelectMany(x => x.Cvs));
            return allCvs.OrderBy(x => x.Number).ToList();
        }

        public override string ToString() => $"FirmwareProtocol {Type} {CVs?.Length ?? 0} Cvs";
    }
}