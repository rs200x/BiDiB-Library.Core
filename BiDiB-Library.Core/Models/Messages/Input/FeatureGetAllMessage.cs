namespace org.bidib.Net.Core.Models.Messages.Input;

[InputMessage(BiDiBMessage.MSG_FEATURE_GETALL)]
public class FeatureGetAllMessage : BiDiBInputMessage
{
    public FeatureGetAllMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_FEATURE_GETALL, 0)
    {

        UseStreaming = MessageParameters.Length > 0 && MessageParameters[0] == 0x01;
    }
    public bool UseStreaming {get;}

    public override string ToString() => $"{base.ToString()}, Streaming: {UseStreaming}";
}