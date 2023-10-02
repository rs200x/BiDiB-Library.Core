using System;

namespace org.bidib.Net.Core.Controllers.Interfaces;

public interface ISocketController<in TConfig> : IConnectionController<TConfig>, IDisposable where TConfig : INetConfig
{
}