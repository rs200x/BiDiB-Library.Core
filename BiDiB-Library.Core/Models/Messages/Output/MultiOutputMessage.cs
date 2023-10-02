using System.Collections.Generic;
using System.Text;

namespace org.bidib.Net.Core.Models.Messages.Output;

public class MultiOutputMessage : BiDiBOutputMessage
{
    public MultiOutputMessage(ICollection<BiDiBOutputMessage> messages) : base(new byte[] { 0 }, 0)
    {
        Messages = messages;
    }

    public ICollection<BiDiBOutputMessage> Messages { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{BiDiBConstants.OutMessagePrefix} {"MULTI_MESSAGE",-25} c:{Messages.Count}");
        foreach (var message in Messages)
        {
            sb.AppendLine(message.ToString());
        }

        return sb.ToString();
    }
}