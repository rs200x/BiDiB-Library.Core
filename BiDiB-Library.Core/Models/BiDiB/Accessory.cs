﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Accessories
{
    [XmlElement("accessory")]
    public Accessory[] Accessory { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Accessory : ModelBase
{
    private AccessoryExecutionState executionState;
    private byte activeAspect;
    private string name;


    [XmlArray("aspects")]
    [XmlArrayItem("aspect", IsNullable = false)]
    public Aspect[] Aspects { get; set; }

    [XmlAttribute("number")]
    public byte Number { get; set; }

    [XmlAttribute("name")]
    public string Name
    {
        get => name;
        set => Set(() => Name, ref name, value);
    }

    [XmlAttribute("startupState")]
    public int StartupState { get; set; }

    [XmlIgnore]
    public AccessoryExecutionState ExecutionState
    {
        get => executionState;
        set => Set(() => ExecutionState, ref executionState, value);
    }

    [XmlIgnore]
    public byte ActiveAspect
    {
        get => activeAspect;
        set => Set(() => ActiveAspect, ref activeAspect, value);
    }

    public IDictionary<AccessoryParameter, byte[]> Parameters { get; } = new Dictionary<AccessoryParameter, byte[]>();

    public override string ToString()
    {
        return $"{Number:D2}: {(!string.IsNullOrEmpty(Name) ? Name : $"Accessory{Number}")}";
    }
}