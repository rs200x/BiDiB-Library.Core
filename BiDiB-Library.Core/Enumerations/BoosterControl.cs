namespace org.bidib.netbidibc.core.Enumerations
{
    /// <summary>
    /// Enum defining states of booster dcc control
    /// </summary>
    public enum BoosterControl
    {
        /// <summary>
        /// Booster is controlled by the connected BiDiB bus
        /// </summary>
        Bus = 0,

        /// <summary>
        /// Booster is using its own DCC generator
        /// </summary>
        Local = 1,

        /// <summary>
        /// Booster is controlled by a remote dcc generator
        /// </summary>
        Remote = 2
    }
}