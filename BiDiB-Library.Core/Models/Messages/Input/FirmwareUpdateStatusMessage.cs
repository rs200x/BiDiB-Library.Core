using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class FirmwareUpdateStatusMessage : BiDiBInputMessage
    {
        public FirmwareUpdateStatusMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_FW_UPDATE_STAT, 2)
        {
            Status = (FirmwareUpdateStatus)MessageParameters[0];

            if (Status == FirmwareUpdateStatus.BIDIB_MSG_FW_UPDATE_STAT_ERROR)
            {
                Error = (FirmwareUpdateError)MessageParameters[1];
            }
            else
            {
                Timeout = MessageParameters[1] * 10;
            }
        }

        /// <summary>
        /// Gets the timeout value in ms
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// Get the operation error
        /// </summary>
        public FirmwareUpdateError Error { get; }

        /// <summary>
        /// Gets the operation status
        /// </summary>
        public FirmwareUpdateStatus Status { get; }

        public override string ToString() => $"{base.ToString()}, {Status}, {(Status == FirmwareUpdateStatus.BIDIB_MSG_FW_UPDATE_STAT_ERROR ? Error.ToString() : $"{Timeout}ms")}";
    }
}