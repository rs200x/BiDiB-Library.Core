using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Controllers;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;

namespace org.bidib.netbidibc.core.Test.Controllers
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
            config.Object.Comport = "COM1";
            config.Object.Baudrate = 115200;

            // Act
            var controller = Target.GetController(config.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.ConnectionName.Should().Be("COM1");
        }
    }
}