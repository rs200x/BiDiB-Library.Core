using System;
using System.IO.Ports;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Controllers;
using org.bidib.netbidibc.core.Controllers.Interfaces;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class SerialPortControllerTests
    {
        private SerialPortController target;
        private Mock<ISerialPort> serialPortMock;

        [TestInitialize]
        public void TestInitialize()
        {
            serialPortMock = new Mock<ISerialPort>();

            target = new SerialPortController(NullLoggerFactory.Instance);

            target.GetType().GetProperty("serialPort", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, serialPortMock.Object);
        }

        [TestMethod]
        public void Ctor_ShouldInitializeSerialPortWithDefaultConfig()
        {
            // Arrange

            // Act
            target = new SerialPortController(NullLoggerFactory.Instance);

            // Assert
            ISerialPort serialPort = target.GetType().GetProperty("serialPort", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target) as ISerialPort;

            serialPort.ReadBufferSize.Should().Be(8192);
            serialPort.WriteBufferSize.Should().Be(4096);
            serialPort.Parity.Should().Be(Parity.None);
            serialPort.StopBits.Should().Be(StopBits.One);
            serialPort.DataBits.Should().Be(8);
            serialPort.Handshake.Should().Be(Handshake.None);
            serialPort.WriteTimeout.Should().Be(200);
        }

        [TestMethod]
        public void Initialize_ShouldSetPortAndBaudRate()
        {
            // Arrange
            var portConfig = new Mock<ISerialPortConfig>();
            portConfig.Object.Baudrate = 115200;
            portConfig.Object.Comport = "COM1";

            // Act
            target.Initialize(portConfig.Object);

            // Assert
            serialPortMock.Object.PortName.Should().Be("COM1");
            serialPortMock.Object.BaudRate.Should().Be(115200);
        }

        [TestMethod]
        public void Initialize_ShouldCloseIfPortIsOpen()
        {
            // Arrange
            serialPortMock.Setup(x => x.IsOpen).Returns(true);
            var portConfig = new Mock<ISerialPortConfig>();
            portConfig.Object.Baudrate = 115200;
            portConfig.Object.Comport = "COM1";

            // Act
            target.Initialize(portConfig.Object);

            // Assert
            serialPortMock.Verify(x => x.DiscardInBuffer());
            serialPortMock.Verify(x => x.DiscardOutBuffer());
            serialPortMock.Verify(x => x.Close());
        }

        [TestMethod]
        public void Initialize_ShouldThrowIfWrongBaudRate()
        {
            // Arrange
            serialPortMock.Setup(x => x.IsOpen).Returns(true);
            var portConfig = new Mock<ISerialPortConfig>();
            portConfig.Object.Baudrate = 11;
            portConfig.Object.Comport = "COM1";

            // Act
            Action action = () => target.Initialize(portConfig.Object);

            // Assert
            action.Should().Throw<InvalidOperationException>().WithMessage("%%ERROR: Please define COM-Port and baud rate !!!!\n");
        }
    }
}