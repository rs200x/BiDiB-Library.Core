using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Extensions;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using SysMagicMessage = org.bidib.Net.Core.Models.Messages.Input.SysMagicMessage;
using SysSoftwareMessage = org.bidib.Net.Core.Models.Messages.Input.SysSoftwareMessage;
using SysProtocolMessage = org.bidib.Net.Core.Models.Messages.Input.SysProtocolMessage;
using FeatureCountMessage = org.bidib.Net.Core.Models.Messages.Input.FeatureCountMessage;
using FeatureMessage = org.bidib.Net.Core.Models.Messages.Input.FeatureMessage;
using FeatureGetAllMessage = org.bidib.Net.Core.Models.Messages.Output.FeatureGetAllMessage;

namespace org.bidib.Net.Core.Models;

public class BiDiBNode(ILogger<BiDiBNode> logger) : Node, IOccupanciesHost
{
    private string fullUserName;
    private int serialNumber;
    private bool isEnabled;
    private NodeState state;
    private string stateInfo;

    public event EventHandler<PositionPortUpdatedEventArgs> PositionPortUpdated;
    public event EventHandler<OccupanciesCollectionUpdatedEventArgs> GlobalOccupanciesCollectionChanged;
    private const int DefaultTimeout = 500;

    public BiDiBNode() : this(NullLogger<BiDiBNode>.Instance) { }

    internal BiDiBNode(IBiDiBMessageProcessor messageProcessor, ILogger<BiDiBNode> logger) : this(logger)
    {
        MessageProcessor = messageProcessor;
    }

    /// <summary>
    /// Gets the unique id in hex format
    /// </summary>
    [XmlIgnore]
    public string HexUniqueId => string.Join(".", UniqueIdBytes.Select(x => $"{x:X2}"));

    /// <summary>
    /// Gets the unique id in vendor id - product id format 
    /// </summary>
    [XmlIgnore]
    public string VendorProductId => $"V {UniqueIdBytes[2]:X2} P {UniqueIdBytes[3]:X2}{UniqueIdBytes[4]:X2}{UniqueIdBytes[5]:X2}{UniqueIdBytes[6]:X2}";

    public byte Status { get; set; }

    [XmlIgnore]
    public int SerialNumber
    {
        get => serialNumber;
        private set { Set(() => SerialNumber, ref serialNumber, value); }
    }

    public string FullUserName
    {
        get
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(fullUserName) &&
                fullUserName.StartsWith(UserName, StringComparison.CurrentCultureIgnoreCase))
            {
                return fullUserName;
            }

            if (!string.IsNullOrEmpty(UserName)) { return UserName; }
            if (!string.IsNullOrEmpty(fullUserName)) { return fullUserName; }

            var name = VendorProductId;
            if (!string.IsNullOrEmpty(ProductName)) { name += $" ({ProductName})"; }
            return name;
        }
        set { Set(() => FullUserName, ref fullUserName, value); }
    }

    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string ProductText { get; set; }
    public string Homepage { get; set; }
    public byte PackageCapacity { get; set; }

    [XmlIgnore]
    public byte[] SoftwareVersion { get; set; }

    public string SoftwareVersionString => this.GetSoftwareVersionString();

    [XmlIgnore]
    public byte[] ProtocolVersion { get; set; }

    public string ProtocolVersionString => this.GetProtocolVersionString();

    public string FullAddress => this.GetFullAddressString();

    public byte FeatureCount { get; set; }

    public string CvFileName { get; set; }

    [XmlIgnore]
    public VendorCv.VendorCv VendorCv { get; set; }

    [XmlIgnore]
    public Dictionary<ushort, PositionPort> PositionPorts { get; } = new();

    [XmlIgnore]
    public Dictionary<ushort, OccupancyInfo> GlobalOccupancies { get; } = new();

    /// <summary>
    /// Determines if node is running in simple boot loader mode
    /// </summary>
    public bool BootLoaderActive { get; set; }

    public bool IsEnabled
    {
        get => isEnabled;
        set => Set(() => IsEnabled, ref isEnabled, value);
    }

    /// <summary>
    /// Determines current state of the node
    /// </summary>
    public NodeState State
    {
        get => state;
        set => Set(() => State, ref state, value);
    }

    /// <summary>
    /// Gets or sets state related information
    /// </summary>
    public string StateInfo
    {
        get => stateInfo;
        set => Set(() => StateInfo, ref stateInfo, value);
    }

    internal IBiDiBMessageProcessor MessageProcessor { get; }

    #region Overrides of Node

    protected override void OnUpdateNodeInfo(byte[] uniqueIdBytes)
    {
        base.OnUpdateNodeInfo(uniqueIdBytes);

        if (uniqueIdBytes == null || uniqueIdBytes.Length < 7) { return; }

        BitArray pidBits = new(new[] { uniqueIdBytes[3], uniqueIdBytes[4], uniqueIdBytes[5], uniqueIdBytes[6] });
        var newSerial = 0;

        for (var i = NumberOfProductIdBits; i < pidBits.Length; i++)
        {
            if (pidBits[i])
            {
                newSerial += Convert.ToInt32(Math.Pow(2, i - NumberOfProductIdBits));
            }
        }

        SerialNumber = newSerial;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(UserName))
        {
            RaisePropertyChanged(()=>FullUserName);
        }
    }

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
        var name = !string.IsNullOrEmpty(ProductName) ? ProductName : UserName;
        return $"{FullAddress} {name}";
    }

    #endregion

    #region messaging

    public void Disable()
    {
        SendMessage(BiDiBMessage.MSG_SYS_DISABLE);
        IsEnabled = false;
    }

    public void Enable()
    {
        SendMessage(BiDiBMessage.MSG_SYS_ENABLE);
        IsEnabled = true;
    }

    public ushort GetMagic(int timeout)
    {
        var magicMessage = SendMessage<SysMagicMessage>(BiDiBMessage.MSG_SYS_GET_MAGIC, timeout);
        logger.LogDebug("Get magic processed: {Magic}", magicMessage?.Magic);

        if (magicMessage is not { MagicLow: 0x0d } || magicMessage.MagicHigh != 0xb0)
        {
            return magicMessage?.Magic ?? 0;
        }

        // node is in boot loader mode
        BootLoaderActive = true;
        FeatureCount = 1;
        Features = [new Feature { FeatureId = (int)BiDiBFeature.FEATURE_FW_UPDATE_MODE, Value = 1 }];

        return magicMessage.Magic;
    }

    public void GetProtocolVersion()
    {
        var protocolMessage = SendMessage<SysProtocolMessage>(BiDiBMessage.MSG_SYS_GET_P_VERSION);
        if (protocolMessage != null)
        {
            ProtocolVersion = protocolMessage.Protocol;
        }
    }

    public void GetSoftwareVersion()
    {
        var softwareMessage = SendMessage<SysSoftwareMessage>(BiDiBMessage.MSG_SYS_GET_SW_VERSION);
        if (softwareMessage != null)
        {
            SoftwareVersion = softwareMessage.Version;
        }
    }

    public void GetUniqueId()
    {
        var uniqueIdMessage = SendMessage<SysUniqueIdMessage>(BiDiBMessage.MSG_SYS_GET_UNIQUE_ID);
        if (uniqueIdMessage != null)
        {
            this.SetUniqueId(uniqueIdMessage.UniqueId);
        }
    }

    public void Reset()
    {
        SendMessage(BiDiBMessage.MSG_SYS_RESET);
    }


    public void GetFeatures()
    {
        // not possible to retrieve features if node in bootloader mode
        if (BootLoaderActive) { return; }

        var featureCountMessage = SendMessage<FeatureCountMessage>(new FeatureGetAllMessage(Address, false));
        if (featureCountMessage == null) { return; }

        FeatureCount = featureCountMessage.Count;
        List<BiDiBOutputMessage> nextMessages = [];
        for (var i = 0; i < featureCountMessage.Count; i++)
        {
            nextMessages.Add(new FeatureNextMessage(Address));

            if (nextMessages.Count != 4 && i != featureCountMessage.Count - 1)
            {
                continue;
            }

            var inputMessages = MessageProcessor.SendMessages<FeatureMessage>(nextMessages, DefaultTimeout);
            ProcessFeatureMessages(inputMessages.ToList());
            nextMessages.Clear();
        }
    }

    private void ProcessFeatureMessages(IEnumerable<FeatureMessage> featureMessages)
    {
        var features = Features?.ToList() ?? [];
        foreach (var featureMessage in featureMessages)
        {
            var feature = features.Find(x => x.FeatureId == featureMessage.FeatureId);
            if (feature == null)
            {
                feature = new Feature { FeatureId = featureMessage.FeatureId, Value = featureMessage.Value };
                features.Add(feature);
            }
            else
            {
                feature.Value = featureMessage.Value;
            }

            EvaluateFeature(feature);
        }

        Features = [..features.OrderBy(x => x.FeatureId)];
    }

    private void EvaluateFeature(Feature feature)
    {
        if (feature.FeatureType == BiDiBFeature.FEATURE_RELEVANT_PID_BITS)
        {
            NumberOfProductIdBits = feature.Value;
        }

        if (feature.FeatureType == BiDiBFeature.FEATURE_STRING_SIZE && feature.Value > 0)
        {
            GetNames();
        }

        if (feature.FeatureType == BiDiBFeature.FEATURE_BM_SIZE && feature.Value > 0)
        {
            GetFeedbackInfo(feature);
        }

        if (feature.FeatureType == BiDiBFeature.FEATURE_BM_ADDR_DETECT_AVAILABLE && feature.Value > 1)
        {
            SendMessage(BiDiBMessage.MSG_BM_ADDR_GET_RANGE, 0, 0);
        }
    }

    private void GetNames()
    {
        SendMessage<Messages.Input.StringMessage>( BiDiBMessage.MSG_STRING_GET, DefaultTimeout, 0, 0 );
        SendMessage<Messages.Input.StringMessage>(BiDiBMessage.MSG_STRING_GET, DefaultTimeout, 0, 1 );
    }

    private void GetFeedbackInfo(Feature feature)
    {
        var multipleMessage = SendMessage<FeedbackMultipleMessage>(BiDiBMessage.MSG_BM_GET_RANGE, DefaultTimeout, 0, feature.Value );

        var stateSize = feature.Value / 8 + (feature.Value % 8 == 0 ? 0 : 1); // extend array size if value not multiple of 8
        var feedbackStateBytes = new byte[stateSize];
        if (multipleMessage != null)
        {
            Array.Copy(multipleMessage.MessageParameters, 2, feedbackStateBytes, 0, stateSize);
        }

        List<FeedbackPort> feedbackPorts = [];

        BitArray feedbackStates = new(feedbackStateBytes);
        for (var i = 0; i < feedbackStates.Length; i++)
        {
            feedbackPorts.Add(new FeedbackPort { Number = i, IsFree = !feedbackStates[i] });
        }
        FeedbackPorts = feedbackPorts.ToArray();

        SendMessage(BiDiBMessage.MSG_BM_ADDR_GET_RANGE, 0, feature.Value);
        SendMessage(BiDiBMessage.MSG_BM_GET_CONFIDENCE);
    }

    public void GetAccessories()
    {
        var feature = this.GetFeature(BiDiBFeature.FEATURE_ACCESSORY_COUNT);
        if (feature == null || feature.Value == 0) { return; }

        var accessories = new List<Accessory>();

        for (var i = 0; i < feature.Value; i++)
        {
            var accessory = new Accessory { Number = i };
            var accessoryState = SendMessage<AccessoryStateMessage>(BiDiBMessage.MSG_ACCESSORY_GET, (byte)i);
            if (accessoryState != null)
            {
                var aspects = new List<Aspect>();
                for (var j = 0; j < accessoryState.Total; j++)
                {
                    aspects.Add(new Aspect { Number = j });
                }

                accessory.Aspects = [..aspects];
                accessory.ExecutionState = accessoryState.ExecutionState;
                accessory.ActiveAspect = accessoryState.Aspect;
            }

            var accessoryPara = SendMessage<AccessoryParaMessage>(new AccessoryParaGetMessage(Address, (byte)i, AccessoryParameter.ACCESSORY_PARA_STARTUP));
            if (accessoryPara != null)
            {
                accessory.StartupState = accessoryPara.Data?[0] ?? 255;
            }
            accessories.Add(accessory);
        }

        Accessories = accessories.ToArray();
    }


    private TResponseMessage SendMessage<TResponseMessage>(BiDiBMessage messageType, params byte[] parameters) where TResponseMessage : BiDiBInputMessage
    {
        return SendMessage<TResponseMessage>(messageType, DefaultTimeout, parameters);
    }

    private TResponseMessage SendMessage<TResponseMessage>(BiDiBMessage messageType, int timeout, params byte[] parameters) where TResponseMessage : BiDiBInputMessage
    {
        return MessageProcessor.SendMessage<TResponseMessage>(this, messageType, timeout, parameters);
    }

    private TResponseMessage SendMessage<TResponseMessage>(BiDiBOutputMessage outputMessage) where TResponseMessage : BiDiBInputMessage
    {
        return MessageProcessor.SendMessage<TResponseMessage>(outputMessage, DefaultTimeout);
    }

    private void SendMessage(BiDiBMessage messageType, params byte[] parameters)
    {
        MessageProcessor.SendMessage(this, messageType, parameters);
    }

    #endregion

    public void GetPorts()
    {

        var ports = new List<Port>();

        ports.AddRange(GetPorts(PortType.Input, BiDiBFeature.FEATURE_CTRL_INPUT_COUNT));
        ports.AddRange(GetPorts(PortType.Switch, BiDiBFeature.FEATURE_CTRL_SWITCH_COUNT));
        ports.AddRange(GetPorts(PortType.Light, BiDiBFeature.FEATURE_CTRL_LIGHT_COUNT));
        ports.AddRange(GetPorts(PortType.Servo, BiDiBFeature.FEATURE_CTRL_SERVO_COUNT));
        ports.AddRange(GetPorts(PortType.Sound, BiDiBFeature.FEATURE_CTRL_SOUND_COUNT));
        ports.AddRange(GetPorts(PortType.Motor, BiDiBFeature.FEATURE_CTRL_MOTOR_COUNT));
        ports.AddRange(GetPorts(PortType.AnalogOut, BiDiBFeature.FEATURE_CTRL_ANALOGOUT_COUNT));
        ports.AddRange(GetPorts(PortType.Backlight, BiDiBFeature.FEATURE_CTRL_BACKLIGHT_COUNT));
        ports.AddRange(GetPorts(PortType.Switchpair, BiDiBFeature.FEATURE_CTRL_SWITCH_COUNT));

        Ports = ports.ToArray();
    }

    private IEnumerable<Port> GetPorts(PortType portType, BiDiBFeature countFeature)
    {
        var feature = this.GetFeature(countFeature);
        return feature?.Value > 0 ? MessageProcessor.GetPorts(this, portType, feature.Value) : new List<Port>();
    }

    public bool SetFeature(Feature feature)
    {
        if (feature == null) { return false; }


        var featureMessage = MessageProcessor.SendMessage<FeatureMessage>(this, BiDiBMessage.MSG_FEATURE_SET, feature.FeatureId, feature.Value);

        return featureMessage != null && featureMessage.Value == feature.Value;
    }

    public void UpdatePosition(PositionPort port)
    {
        if (port == null) { return; }
        PositionPorts[port.Position] = port;
        OnPositionPortUpdated(port);
    }

    private void OnPositionPortUpdated(PositionPort port)
    {
        PositionPortUpdated?.Invoke(this, new PositionPortUpdatedEventArgs(port));
    }

    #region IOccupancyProvider

    public OccupancyInfo GetOccupancy(ushort address)
    {
        if (GlobalOccupancies.TryGetValue(address, out var occupancy))
        {
            return occupancy;
        }

        var newOccupancy = new OccupancyInfo { Address = address };
        AddOccupancy(newOccupancy);
        return newOccupancy;
    }

    public ICollection<OccupancyInfo> GetOccupanciesByFilter(Func<OccupancyInfo, bool> filter)
    {
        return GlobalOccupancies.Values.Where(filter).ToList();
    }

    public void AddOccupancy(OccupancyInfo occupancy)
    {
        if (occupancy == null) { return; }

        lock (GlobalOccupancies)
        {
            GlobalOccupancies.Add(occupancy.Address, occupancy);
            GlobalOccupanciesCollectionChanged?.Invoke(this, new OccupanciesCollectionUpdatedEventArgs(occupancy, null));
        }
    }

    public void RemoveOccupancy(OccupancyInfo occupancy)
    {
        if (occupancy == null) { return; }

        lock (GlobalOccupancies)
        {
            GlobalOccupancies.Remove(occupancy.Address);
            GlobalOccupanciesCollectionChanged?.Invoke(this, new OccupanciesCollectionUpdatedEventArgs(null, occupancy));
            occupancy.Dispose();
        }
    }

    public void ClearOccupancies()
    {
        lock (GlobalOccupancies)
        {
            foreach (var occupancy in GlobalOccupancies.Select(x=>x.Value))
            {
                GlobalOccupanciesCollectionChanged?.Invoke(this, new OccupanciesCollectionUpdatedEventArgs(null, occupancy));
                occupancy.Dispose();
            }

            GlobalOccupancies.Clear();
        }
    }

    #endregion


}