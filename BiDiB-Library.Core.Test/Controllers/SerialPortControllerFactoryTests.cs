using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Controllers;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Controllers
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class SerialPortControllerFactoryTests : TestClass<SerialPortControllerFactory>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new SerialPortControllerFactory(NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void ConnectionType_ShouldBeSerialPort()
        {
            // Arrange

            // Act

            // Assert
            Target.ConnectionType.Should().Be(InterfaceConnectionType.SerialPort);
        }

        [TestMethod]
        public void GetController_ShouldCreateAndInitController()
        {
            // Arrange
            var config = new Mock<ISerialPortConfig>();
            config.Setup(x=>x.Comport).Returns("COM1");
            config.Setup(x => x.Baudrate).Returns(115200);

            // Act
            var controller = Target.GetController(config.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.ConnectionName.Should().Be("COM1");
        }
    }
}