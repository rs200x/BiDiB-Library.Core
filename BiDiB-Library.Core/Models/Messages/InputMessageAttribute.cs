using System;

namespace org.bidib.netbidibc.core.Models.Messages;

[AttributeUsage(AttributeTargets.Class)]
internal class InputMessageAttribute : Attribute
{
    public InputMessageAttribute(BiDiBMessage messageType)
    {
        MessageType = messageType;
    }

    public BiDiBMessage MessageType { get; set; }
}
