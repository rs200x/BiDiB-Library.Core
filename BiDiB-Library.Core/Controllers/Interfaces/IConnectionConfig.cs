using org.bidib.Net.Core.Enumerations;

namespace org.bidib.Net.Core.Controllers.Interfaces;

public interface IConnectionConfig
{
    string Id { get; set; }

    string Name { get; set; }

    string ApplicationName { get; set; }

    InterfaceConnectionType ConnectionType { get; set; }

    ConnectionStrategyType ConnectionStrategyType { get; set; }
}