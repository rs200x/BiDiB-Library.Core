using System.Collections.Generic;
using org.bidib.Net.Core.Models.Messages.Input;

namespace org.bidib.Net.Core.Message;

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