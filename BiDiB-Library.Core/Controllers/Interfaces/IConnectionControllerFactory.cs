using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Controllers.Interfaces;

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