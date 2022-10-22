namespace org.bidib.netbidibc.core.Enumerations;

public enum SubscriptionResult
{
    SubscriptionEstablished = 0x00,
    SubscriptionEstablishedNodeNotAvailable = 0x01,
    SubscriptionsNotSupported = 0x80,
    SubscriptionNotEstablishedNoNode = 0x81,
    SubscriptionNotEstablishedSubSelf = 0x82,
    TargetModeNotSupported = 0xFF
}
