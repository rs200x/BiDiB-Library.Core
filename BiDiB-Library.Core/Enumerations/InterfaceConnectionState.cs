namespace org.bidib.netbidibc.core.Enumerations
{
    /// <summary>
    /// Enumeration that describes the connection state to the interface
    /// </summary>
    public enum InterfaceConnectionState
    {
        /// <summary>
        /// Indicates whether there is no connection to the interface
        /// </summary>
        Disconnected,

        /// <summary>
        /// Indicates that the clients doesn't know each other
        /// </summary>
        Unpaired,

        /// <summary>
        /// Indicates that a pairing with the remote client is needed
        /// </summary>
        PairingRequired,

        /// <summary>
        /// Indicates that the remote clients requests a pairing
        /// </summary>
        PairingRequested,

        /// <summary>
        /// Indicates whether there is a full control connection to the interface
        /// </summary>
        FullyConnected,

        /// <summary>
        /// Indicates whether there is a connection established but with limited control
        /// </summary>
        PartiallyConnected
    }
}