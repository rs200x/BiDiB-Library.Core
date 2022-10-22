using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    public interface IConnectionConfig
    {
        string Id { get; set; }

        string Name { get; set; }

        InterfaceConnectionType ConnectionType { get; set; }
    }
}