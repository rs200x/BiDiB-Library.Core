using System;
using System.Diagnostics;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Product;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.DecoderNamespaceUrl, TypeName = "DecoderType")]
public class Decoder
{
    [XmlElement("description")]
    public Description[] Description { get; set; }

    [XmlElement("specifications")]
    public Specifications Specifications { get; set; }
        
    [XmlArray("images", Namespace = Namespaces.CommonTypesNamespaceUrl)]
    [XmlArrayItem("image", IsNullable = false)]
    public Image[] Images { get; set; }

    [XmlAttribute("name", DataType = XmlDataTypes.Token)]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public DecoderType Type { get; set; }

    [XmlAttribute("manufacturerId")]
    public byte ManufacturerId { get; set; }

    [XmlAttribute("manufacturerExtendedId")]
    public ushort ManufacturerExtendedId { get; set; }

    [XmlAttribute("manufacturerName", DataType = XmlDataTypes.Token)]
    public string ManufacturerName { get; set; }

    [XmlAttribute("manufacturerShortName", DataType = XmlDataTypes.Token)]
    public string ManufacturerShortName { get; set; }

    [XmlAttribute("manufacturerUrl", DataType = XmlDataTypes.AnyUri)]
    public string ManufacturerUrl { get; set; }

    [XmlAttribute("typeIds", DataType = XmlDataTypes.Token)]
    public string TypeIds { get; set; }

    [XmlAttribute("articleNumbers", DataType = XmlDataTypes.Token)]
    public string ArticleNumbers { get; set; }

    [XmlAttribute("producedFrom", DataType = XmlDataTypes.Year)]
    public string ProducedFrom { get; set; }

    [XmlAttribute("producedTill", DataType = XmlDataTypes.Year)]
    public string ProducedTill { get; set; }

    [XmlAttribute("decoderDBLink", DataType = XmlDataTypes.AnyUri)]
    public string DecoderDbLink { get; set; }

    #region Overrides of Object

    public override string ToString()
    {
        return Name;
    }

    #endregion
}