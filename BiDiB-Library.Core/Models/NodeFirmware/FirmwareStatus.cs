using System;
using System.Xml.Serialization;

namespace org.bidib.netbidibc.core.Models.NodeFirmware
{
    [Serializable]
    [XmlType("FirmwareStatusType", Namespace = Namespaces.FirmwareNamespaceUrl)]
    public enum FirmwareStatus
    {
        [XmlEnum("none")]
        None,

        [XmlEnum("beta")]
        Beta,

        [XmlEnum("stable")]
        Stable,

        [XmlEnum("mandatory")]
        Mandatory
    }
}
