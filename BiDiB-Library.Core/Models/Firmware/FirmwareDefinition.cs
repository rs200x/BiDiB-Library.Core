using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;

namespace org.bidib.Net.Core.Models.Firmware;

[Serializable]
[DebuggerStepThrough]
[XmlType(AnonymousType = true, Namespace = Namespaces.DecoderFirmwareNamespaceUrl)]
[XmlRoot("decoderFirmwareDefinition", Namespace = Namespaces.DecoderFirmwareNamespaceUrl, IsNullable = false)]
public class FirmwareDefinition : ItemWithVersion, IFileSourceItem
{
    [XmlElement("firmware")]
    public Firmware Firmware { get; set; }

    #region Overrides of Object

    public override string ToString()
    {
        return Firmware.ToString();
    }

    #endregion

    public string SourceFile { get; set; }
}