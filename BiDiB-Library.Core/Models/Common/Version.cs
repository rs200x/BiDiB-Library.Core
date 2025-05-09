﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using org.bidib.Net.Core.Models.Xml;

namespace org.bidib.Net.Core.Models.Common;

[Serializable]
[DebuggerStepThrough]
[XmlType(Namespace = Namespaces.CommonTypesNamespaceUrl, TypeName = "VersionType")]
public class Version
{
    [XmlAttribute("createdBy", DataType = XmlDataTypes.Token)]
    public string CreatedBy { get; set; }

    [XmlAttribute("creatorLink", DataType = XmlDataTypes.AnyUri)]
    public string CreatorLink { get; set; }

    [XmlAttribute("author")]
    public string Author { get; set; }

    [XmlAttribute("lastUpdate")]
    [JsonProperty("lastUpdate")]
    public string LastUpdateString
    {
        get => LastUpdate.ToString("s", CultureInfo.CurrentCulture);
        set => LastUpdate = DateTime.Parse(value, CultureInfo.CurrentCulture);
    }

    [XmlIgnore]
    [JsonIgnore]
    public DateTime LastUpdate { get; set; }

    [XmlAttribute("created", DataType = XmlDataTypes.Date)]
    public DateTime Created { get; set; }
}