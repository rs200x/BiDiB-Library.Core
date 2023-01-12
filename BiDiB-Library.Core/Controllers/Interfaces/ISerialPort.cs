using System.IO.Ports;

namespace org.bidib.netbidibc.core.Controllers.Interfaces
{
    /// <summary>
    /// Wrapping interface for SerialPort - used for testing
    /// </summary>
    public interface ISerialPort
    {
        bool IsOpen { get; }
        string PortName { get; set; }
        int BaudRate { get; set; }
        int BytesToRead { get;  }
        int ReadBufferSize { get; set; }
        int WriteTimeout { get; set; }
        Handshake Handshake { get; set; }
        int DataBits { get; set; }
        StopBits StopBits { get; set; }
        Parity Parity { get; set; }
        int WriteBufferSize { get; set; }
        bool CtsHolding { get; }
        bool RtsEnable { get; set; }
        void Open();
        void Close();

        event SerialDataReceivedEventHandler DataReceived;
        event SerialPinChangedEventHandler PinChanged;

        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);

        void DiscardInBuffer();
        void DiscardOutBuffer();
    }
}