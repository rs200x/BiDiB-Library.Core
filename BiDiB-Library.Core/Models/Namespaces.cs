using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "XML Namespaces")]
public static class Namespaces
{
    public const string BiDiB10NamespaceUrl = "http://www.bidib.org/schema/bidib";

    public const string BiDiB20NamespaceUrl = "http://www.bidib.org/schema/bidib/2.0";

    public const string BiDiBBase10NamespaceUrl = "http://www.bidib.org/schema/bidibbase/1.0";

    public const string VendorCvNamespaceUrl = "http://www.bidib.org/schema/vendorcv";

    public const string Labels10NamespaceUrl = "http://www.bidib.org/schema/nodeLabels/1.0";

    #region decoder cv

    public const string DecoderCvNamespaceUrl = "http://www.bidib.org/schema/decoderCv/1.1";

    #endregion

    public const string SimulationNamespaceUrl = "http://www.bidib.org/jbidibc/simulation/nodes";

    public const string FirmwareNamespaceUrl = "http://www.bidib.org/schema/firmware";


    #region userDevices
     
    public const string UserDevices10NamespaceUrl = "http://www.bidib.org/schema/userDevices/1.0";
    public const string UserDevices11NamespaceUrl = "http://www.bidib.org/schema/userDevices/1.1";
    public const string UserDevices12NamespaceUrl = "http://www.bidib.org/schema/userDevices/1.2";
    public const string UserDevices13NamespaceUrl = "http://www.bidib.org/schema/userDevices/1.3";
    public const string UserDevices14NamespaceUrl = "http://www.bidib.org/schema/userDevices/1.4";

    /// <summary>
    /// The current/latest UserDevices namespace url
    /// </summary>
    public const string UserDevicesNamespaceUrl = UserDevices14NamespaceUrl;

    /// <summary>
    /// The current/latest UserDevices schema
    /// </summary>
    public static readonly XmlNamespace UserDevices = new("ud", UserDevicesNamespaceUrl, "userDevices.xsd");

    #endregion

    #region manufacturers

    /// <summary>
    /// The current manufacturers namespace url
    /// </summary>
    public const string ManufacturersNamespaceUrl = "http://www.decoderdb.de/schema/manufacturers/1.1";

    #endregion

    #region decoder firmware

    /// <summary>
    /// The current decoder firmware namespace url
    /// </summary>
    public const string DecoderFirmwareNamespaceUrl = "http://www.decoderdb.de/schema/decoderFirmware/1.2";


    #endregion

    #region common types

    /// <summary>
    /// The current commonTypes namespace url
    /// </summary>
    public const string CommonTypesNamespaceUrl = "http://www.decoderdb.de/schema/commonTypes/1.2";

    public static XmlNamespace CommonTypes { get; } = new("ct", CommonTypesNamespaceUrl, "commonTypes.xsd");

    #endregion

    #region decoder

    /// <summary>
    /// The current Decoder namespace url
    /// </summary>
    public const string DecoderNamespaceUrl = "http://www.decoderdb.de/schema/decoder/1.3";

    public static readonly XmlNamespace Decoder = new("dc", DecoderNamespaceUrl, "decoder.xsd");

    #endregion

    #region decoder detection

    /// <summary>
    /// The current decoder detection namespace url
    /// </summary>
    public const string DecoderDetectionNamespaceUrl = "http://www.decoderdb.de/schema/decoderDetection/1.1";

    #endregion
}