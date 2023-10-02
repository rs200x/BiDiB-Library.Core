using System;
using System.IO.Ports;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Controllers;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
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

            target.GetType().GetField("serialPort", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, serialPortMock.Object);
        }
        
        [TestMethod]
        public void Ctor_ShouldThrow_WhenLoggerFactoryNull()
        {
            // Arrange

            // Act
            var action = () => new SerialPortController(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'loggerFactory')");
        }

        [TestMethod]
        public void Ctor_ShouldInitializeSerialPortWithDefaultConfig()
        {
            // Arrange

            // Act
            target = new SerialPortController(NullLoggerFactory.Instance);

            // Assert
            var serialPort = target.GetType().GetField("serialPort", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target) as ISerialPort;

            serialPort.ReadBufferSize.Should().Be(8192);
            serialPort.WriteBufferSize.Should().Be(4096);
            serialPort.Parity.Should().Be(Parity.None);
            serialPort.StopBits.Should().Be(StopBits.One);
            serialPort.DataBits.Should().Be(8);
            serialPort.Handshake.Should().Be(Handshake.None);
            serialPort.WriteTimeout.Should().Be(200);
        }
        
        [TestMethod]
        public void Initialize_ShouldThrow_WhenConfigNull()
        {
            // Arrange

            // Act
            var action = () => target.Initialize(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'config')");
        }

        [TestMethod]
        public void Initialize_ShouldSetPortAndBaudRate()
        {
            // Arrange
            var portConfig = new Mock<ISerialPortConfig>();
            portConfig.Setup(x => x.Baudrate).Returns(115200);
            portConfig.Setup(x => x.Comport).Returns("COM1");

            // Act
            target.Initialize(portConfig.Object);

            // Assert
            serialPortMock.VerifySet(x=>x.PortName = "COM1");
            serialPortMock.VerifySet(x=>x.BaudRate = 115200);
        }

        [TestMethod]
        public void Initialize_ShouldCloseIfPortIsOpen()
        {
            // Arrange
            serialPortMock.Setup(x => x.IsOpen).Returns(true);
            var portConfig = new Mock<ISerialPortConfig>();
            portConfig.Setup(x => x.Baudrate).Returns(115200);
            portConfig.Setup(x => x.Comport).Returns("COM1");

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
            portConfig.Setup(x => x.Baudrate).Returns(11);
            portConfig.Setup(x => x.Comport).Returns("COM1");

            // Act
            var action = () => target.Initialize(portConfig.Object);

            // Assert
            action.Should().Throw<InvalidOperationException>().WithMessage("Invalid baud rate configuration 11! Valid values are 19.200 or 115.200");
        }
    }
}