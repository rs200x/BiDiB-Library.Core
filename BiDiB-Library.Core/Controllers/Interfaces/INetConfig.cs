namespace org.bidib.Net.Core.Controllers.Interfaces;

public interface INetConfig : IConnectionConfig
{
    string NetworkHostAddress { get; set; }

    int NetworkPortNumber { get; set; }
}