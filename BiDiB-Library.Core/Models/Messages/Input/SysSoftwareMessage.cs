using System.Collections.Generic;
using System.Linq;
using org.bidib.Net.Core.Utils;

namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_SYS_SW_VERSION)]
public class SysSoftwareMessage : BiDiBInputMessage
{
    public SysSoftwareMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_SW_VERSION, 3)
    {
        Version = new[] { MessageParameters[2], MessageParameters[1], MessageParameters[0] };

        var subVersions = new List<byte[]>();

        if (MessageParameters.Length > 3 && MessageParameters.Length % 3 == 0)
        {
            var subParameters = MessageParameters.Skip(3).ToList();
            for (int i = 0; i < subParameters.Count / 3; i++)
            {
                var subIndex = i * 3;
                subVersions.Add(new[] { subParameters[subIndex+2], subParameters[subIndex+1], subParameters[subIndex] });
            }
        }

        SubVersions = subVersions;
    }

    public byte[] Version { get; }
    
    public IEnumerable<byte[]> SubVersions { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, V: {Version.GetSoftwareVersionString()}, {string.Join(',', SubVersions.Select(x=>x.GetSoftwareVersionString()))}";
    }
}