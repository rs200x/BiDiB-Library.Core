namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    public interface ISerialPortConfig : IConnectionConfig
    {
        string Comport { get; set; }

        int Baudrate { get; set; }

    }
}