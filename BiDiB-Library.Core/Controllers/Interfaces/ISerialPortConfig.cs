namespace org.bidib.Net.Core.Controllers.Interfaces;

public interface ISerialPortConfig : IConnectionConfig
{
    string Comport { get; set; }

    int Baudrate { get; set; }

}