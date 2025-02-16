using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Common;

/// <summary>
/// Class representing image objects
/// </summary>
[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "ImageType")]
public class Image
{
    [XmlAttribute("name", DataType = XmlDataTypes.Token)]
    public string Name { get; set; }

    [XmlAttribute("src", DataType = XmlDataTypes.AnyUri)]
    public string Source { get; set; }
        
    [XmlAttribute("lastModified")]
    [JsonProperty("lastModified")]
    public string LastModifiedString
    {
        get => LastModified.ToString("s", CultureInfo.InvariantCulture);
        set => LastModified = DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    [XmlIgnore]
    [JsonIgnore]
    public DateTime LastModified { get; set; }

    [XmlAttribute("copyright")]
    public string Copyright { get; set; }
}