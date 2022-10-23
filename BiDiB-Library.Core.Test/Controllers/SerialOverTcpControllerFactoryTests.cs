using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Controllers;
using org.bidib.netbidibc.core.Controllers.Interfaces;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Controllers
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class SerialOverTcpControllerFactoryTests : TestClass<SerialOverTcpControllerFactory>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            var loggerFactory = new Mock<ILoggerFactory>();
            Target = new SerialOverTcpControllerFactory(loggerFactory.Object);
        }

        [TestMethod]
        public void ConnectionType_ShouldBeSerialOverTcp()
        {
            // Arrange

            // Act

            // Assert
            Target.ConnectionType.Should().Be(InterfaceConnectionType.SerialOverTcp);
        }

        [TestMethod]
        public void GetController_ShouldCreateAndInitController()
        {
            // Arrange
            var config = new Mock<INetConfig>();
            config.Object.NetworkHostAddress = "127.0.0.1";
            config.Object.NetworkPortNumber = 123;

            // Act
            var controller = Target.GetController(config.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.ConnectionName.Should().Be("127.0.0.1:123");
        }
    }
}