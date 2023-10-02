using System.Collections.Generic;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class FirmwareUpdateOperationMessage : BiDiBOutputMessage
{
    public FirmwareUpdateOperationMessage(Node node, FirmwareUpdateOperation operation) : base(node.Address, BiDiBMessage.MSG_FW_UPDATE_OP)
    {
        Operation = operation;

        if (operation == FirmwareUpdateOperation.BIDIB_MSG_FW_UPDATE_OP_ENTER)
        {
            var parameters = new List<byte>(node.UniqueIdBytes);
            parameters.Insert(0, (byte)operation);
            Parameters = parameters.ToArray();
        }

        if (operation == FirmwareUpdateOperation.BIDIB_MSG_FW_UPDATE_OP_DONE || operation == FirmwareUpdateOperation.BIDIB_MSG_FW_UPDATE_OP_EXIT)
        {
            Parameters = new[] { (byte)operation };
        }
    }

    public FirmwareUpdateOperationMessage(Node node, FirmwareUpdateOperation operation, byte destination) : base(node.Address, BiDiBMessage.MSG_FW_UPDATE_OP)
    {
        Operation = operation;
        Parameters = new[] {(byte) operation, destination};
    }

    public FirmwareUpdateOperationMessage(Node node, FirmwareUpdateOperation operation, byte[] data) : base(node.Address, BiDiBMessage.MSG_FW_UPDATE_OP)
    {
        Operation = operation;
        var parameters = new List<byte>(data);
        parameters.Insert(0, (byte)operation);
        Parameters = parameters.ToArray();
    }

    public FirmwareUpdateOperation Operation { get; }

    public override string ToString() => $"{base.ToString()} O:{Operation} D:{Parameters.GetDataString()}";
}