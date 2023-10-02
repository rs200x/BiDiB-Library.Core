using System;
using org.bidib.Net.Core;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_NODETAB)]
public class NodeTabMessage : BiDiBInputMessage
{
    public NodeTabMessage(byte[] messageBytes) : this(messageBytes, BiDiBMessage.MSG_NODETAB)
    {
    }

    public NodeTabMessage(byte[] messageBytes, BiDiBMessage expectedMessageType) 
        : base(messageBytes, expectedMessageType, 9)
    {
        TableVersion = MessageParameters[0];
        LocalNodeAddress = MessageParameters[1];

        ClassId1 = MessageParameters[2];
        ClassId2 = MessageParameters[3];
        VendorId = MessageParameters[4];
        ProducerId1 = MessageParameters[5];
        ProducerId2 = MessageParameters[6];
        ProducerId3 = MessageParameters[7];
        ProducerId4 = MessageParameters[8];

        UniqueId = new byte[7];
        Array.Copy(MessageParameters, 2, UniqueId, 0, UniqueId.Length);

        HexUid = $"{ClassId1:X2}.{ClassId2:X2}.{VendorId:X2}.{ProducerId1:X2}.{ProducerId2:X2}.{ProducerId3:X2}.{ProducerId4:X2}";
        VendorProductId = $"V {VendorId:X2} P {ProducerId1:X2}{ProducerId2:X2}{ProducerId3:X2}{ProducerId4:X2}";

        DefineNodeAddress();
    }

    private void DefineNodeAddress()
    {
        var addr = new byte[Address.Length + 1];

        if (Address.Length == 1 && Address[0] == 0)
        {
            // the parent is the interface node
            addr = new byte[1];
            addr[0] = LocalNodeAddress;
        }
        else if (LocalNodeAddress == 0)
        {
            // we have the node itself
            addr = new byte[Address.Length];
            Array.Copy(Address, 0, addr, 0, Address.Length);
        }
        else
        {
            // add the local address of the sub node to the parent address
            Array.Copy(Address, 0, addr, 0, Address.Length);
            addr[Address.Length] = LocalNodeAddress;
        }

        NodeAddress = addr;
    }

    public byte ClassId1 { get; set; }

    public byte ClassId2 { get; set; }

    public byte VendorId { get; set; }

    public byte ProducerId1 { get; set; }

    public byte ProducerId2 { get; set; }

    public byte ProducerId3 { get; set; }

    public byte ProducerId4 { get; set; }

    public string HexUid { get; set; }

    public string VendorProductId { get; set; }

    public byte TableVersion { get; }

    public byte LocalNodeAddress { get;}

    public byte[] UniqueId { get; }

    public byte[] NodeAddress { get; private set; }

    #region Overrides of BiDiBInputMessage

    public override string ToString()
    {
        return $"{base.ToString()}, TabVer:{TableVersion}, LocAddr: {LocalNodeAddress}, ID: {HexUid}";
    }

    #endregion
}