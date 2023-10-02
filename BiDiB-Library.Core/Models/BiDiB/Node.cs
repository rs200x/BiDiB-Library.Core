using System;
using System.Collections;
using System.Xml.Serialization;
using org.bidib.Net.Core.Models.BiDiB.Extensions;

namespace org.bidib.Net.Core.Models.BiDiB;

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Nodes
{
    [XmlElement("node")]
    public Node[] Items { get; set; }
}

[Serializable]
[XmlType(Namespace = Namespaces.BiDiB20NamespaceUrl)]
public class Node : ModelBase
{
    private int numberOfProductIdBits;
    private long uniqueId;
    private string userName;
    private Accessory[] accessories;
    private Feature[] features;

    protected Node()
    {
        Address = new byte[] { 0 };
        UniqueIdBytes = new byte[7];
        NumberOfProductIdBits = 16;
    }

    #region properties defined by BiDiB.xsd

    [XmlArray("features")]
    [XmlArrayItem("feature", IsNullable = false)]
    public Feature[] Features
    {
        get => features;
        set { Set(() => Features, ref features, value); }
    }

    [XmlArray("ports")]
    [XmlArrayItem("port", IsNullable = false)]
    public Port[] Ports { get; set; }

    [XmlArray("feedbackPorts")]
    [XmlArrayItem("feedbackPort", IsNullable = false)]
    public FeedbackPort[] FeedbackPorts { get; set; }

    [XmlArray("macros")]
    [XmlArrayItem("macro", IsNullable = false)]
    public Macro[] Macros { get; set; }

    [XmlArray("accessories")]
    [XmlArrayItem("accessory", IsNullable = false)]
    public Accessory[] Accessories
    {
        get => accessories;
        set { Set(() => Accessories, ref accessories, value); }
    }

    [XmlAttribute("manufacturerId")]
    public int ManufacturerId { get; set; }

    [XmlAttribute("productId")]
    public int ProductId { get; set; }

    [XmlAttribute("userName")]
    public string UserName
    {
        get => userName;
        set { Set(() => UserName, ref userName, value); }
    }

    [XmlAttribute("numberOfProductIdBits")]
    public int NumberOfProductIdBits
    {
        get => numberOfProductIdBits;
        set
        {
            Set(() => NumberOfProductIdBits, ref numberOfProductIdBits, value);
            UpdateNodeInfo();
        }
    }

    [XmlAttribute("uniqueId")]
    public long UniqueId
    {
        get => uniqueId;
        set
        {
            Set(() => UniqueId, ref uniqueId, value);
            UpdateNodeInfo();
        }
    }

    #endregion

    #region additional properties

    /// <summary>
    /// Gets or sets the unique id bytes
    /// </summary>
    [XmlIgnore]
    public byte[] UniqueIdBytes { get; set; }

    /// <summary>
    /// Gets or sets the address bytes
    /// </summary>
    [XmlIgnore]
    public byte[] Address { get; set; }

    /// <summary>
    /// This is a bit field, which indicates the class of this node.
    /// This bit field is a quick reference for the host which shows the available features on this specific node. 
    /// If a node has implemented commands of a particular class, the appropriate class bit must be set too. 
    /// A node can also belong to several classes at once.
    /// </summary>
    [XmlIgnore]
    public byte ClassId { get; set; }

    /// <summary>
    /// ClassID Extension; this byte is reserved and must be coded with 0.
    /// </summary>
    [XmlIgnore]
    public byte ClassIdExtended { get; set; }

    /// <summary>
    /// Indicates if node contains switching functions, e.g. light control
    /// </summary>
    [XmlIgnore]
    public bool HasSwitchFunctions => (ClassId & 1) == 1;

    /// <summary>
    /// Indicates if node contains booster functions
    /// </summary>
    [XmlIgnore]
    public bool HasBoosterFunctions => (ClassId & 2) == 2;

    /// <summary>
    /// Indicates if node contains accessory functions
    /// </summary>
    [XmlIgnore]
    public bool HasAccessoryFunctions => (ClassId & 4) == 4;

    /// <summary>
    /// Indicates if node contains DCC signal generator for programming
    /// </summary>
    [XmlIgnore]
    public bool HasCommandStationProgrammingFunctions => (ClassId & 8) == 8;

    /// <summary>
    /// Indicates if node contains DCC signal generator for driving, switching
    /// </summary>
    [XmlIgnore]
    public bool HasCommandStationFunctions => (ClassId & 16) == 16;

    /// <summary>
    /// Indicates if node contains guest functions
    /// </summary>
    [XmlIgnore]
    public bool HasGuestFunctions => (ClassId & 32) == 32;

    /// <summary>
    /// Indicates if node contains occupancy detection functions
    /// </summary>
    [XmlIgnore]
    public bool HasFeedbackFunctions => (ClassId & 64) == 64;

    /// <summary>
    /// Indicates if node contains sub-nodes (is an interface itself)
    /// </summary>
    [XmlIgnore]
    public bool HasSubNodesFunctions => (ClassId & 128) == 128;

    /// <summary>
    /// Indicates if node contains position detection functions
    /// </summary>
    [XmlIgnore]
    public bool HasPositionFunctions => ProductId == 32770; // currently only supported by RF Basis Node

    #endregion

    #region internal methods

    private void UpdateNodeInfo()
    {
        UniqueIdBytes = this.GetUniqueIdBytes();

        ClassId = UniqueIdBytes[0];
        ClassIdExtended = UniqueIdBytes[1];
        ManufacturerId = UniqueIdBytes[2];

        var pidBits = new BitArray(new[] { UniqueIdBytes[3], UniqueIdBytes[4], UniqueIdBytes[5], UniqueIdBytes[6] });

        var newPid = 0;

        for (var i = 0; i < NumberOfProductIdBits; i++)
        {
            if (pidBits[i])
            {
                newPid += Convert.ToInt32(Math.Pow(2, i));
            }
        }

        ProductId = newPid;

        OnUpdateNodeInfo(UniqueIdBytes);
    }

    protected virtual void OnUpdateNodeInfo(byte[] uniqueIdBytes) { }

    #endregion
}