using System;

namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    public interface ISocketController : IConnectionController<INetConfig>, IDisposable
    {
    }
}