using FluentAssertions;
using Microsoft.Extensions.Logging;
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
            config.Setup(x => x.NetworkHostAddress).Returns("127.0.0.1");
            config.Setup(x => x.NetworkPortNumber).Returns(123);

            // Act
            var controller = Target.GetController(config.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.ConnectionName.Should().Be("127.0.0.1:123");
        }
    }
}