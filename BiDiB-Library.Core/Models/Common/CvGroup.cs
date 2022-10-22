using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.Common
{
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "CVGroupType")]
    public class CvGroup : CvBase
    {
        [XmlElement("cv")]
        public Cv[] Cvs { get; set; }

        [XmlIgnore]
        public CvGroupType Type
        {
            get => Enum.TryParse(TypeString, true, out CvGroupType t) ? t : CvGroupType.List;
            set => TypeString = value.ToString();
        }

        [XmlAttribute("stringEncoding")]
        public string StringEncoding { get; set; }

        [XmlAttribute("options")]
        public CvGroupTypeOptions Options { get; set; }

        [XmlElement("conditions")]
        public Conditions Conditions { get; set; }

        [XmlIgnore]
        public string TypeShortName
        {
            get
            {
                switch (Type)
                {
                    case CvGroupType.DccLongAddr:
                        return "DLA";
                    case CvGroupType.DccSpeedCurve:
                        return "DSC";
                    case CvGroupType.DccAccAddr:
                        return "DAA";
                    case CvGroupType.DccAddrRg:
                        return "DAR";
                    case CvGroupType.Int:
                        return "INT";
                    case CvGroupType.Long:
                        return "LNG";
                    case CvGroupType.String:
                        return "STR";
                    case CvGroupType.Matrix:
                        return "MTX";
                    case CvGroupType.DccLongConsist:
                        return "DLC";
                    case CvGroupType.RgbColor:
                        return "RGB";
                    case CvGroupType.CentesimalInt:
                        return "CET";
                    default:
                        return "CVG";
                }
            }
        }
    }
}