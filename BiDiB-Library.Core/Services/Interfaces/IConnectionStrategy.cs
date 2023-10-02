using System.Threading.Tasks;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models;

namespace org.bidib.Net.Core.Services.Interfaces;

public interface IConnectionStrategy
{
    ConnectionStrategyType Type { get; }

    ConnectionStateInfo ConnectionState { get; }

    Task ConnectAsync(IConnectionConfig connectionConfig);

    void Disconnect();
}