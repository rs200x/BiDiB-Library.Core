using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    public interface IConnectionControllerFactory
    {
        /// <summary>
        /// Gets the supported connection type
        /// </summary>
        InterfaceConnectionType ConnectionType { get; }

        /// <summary>
        /// Gets an connection controller instance for the supported connection type
        /// </summary>
        /// <param name="connectionConfig">The connection configuration</param>
        /// <returns></returns>
        IConnectionController GetController(IConnectionConfig connectionConfig);
    }
}