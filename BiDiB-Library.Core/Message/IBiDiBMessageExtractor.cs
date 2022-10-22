using System.Collections.Generic;
using org.bidib.netbidibc.core.Models.Messages.Input;

namespace org.bidib.netbidibc.core.Message
{
    public interface IBiDiBMessageExtractor
    {
        /// <summary>
        /// Extracts the bidib input messages out of the array of data bytes
        /// </summary>
        /// <param name="messageBytes">The message bytes</param>
        /// <param name="checkCrc">Check crc protection of messages</param>
        /// <returns>Collection of extracted messages</returns>
        IEnumerable<BiDiBInputMessage> ExtractMessage(byte[] messageBytes, bool checkCrc);
    }
}