using System;

namespace org.bidib.Net.Core.Models.Messages;

[AttributeUsage(AttributeTargets.Class)]
internal class InputMessageAttribute : Attribute
{
    public InputMessageAttribute(BiDiBMessage messageType)
    {
        MessageType = messageType;
    }

    public BiDiBMessage MessageType { get; set; }
}
