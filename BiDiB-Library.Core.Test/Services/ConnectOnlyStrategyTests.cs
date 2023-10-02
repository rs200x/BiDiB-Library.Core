using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Services.ConnectionStrategies;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Services;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class ConnectOnlyStrategyTests : TestClass<ConnectOnlyStrategy>
{
    private Mock<IConnectionService> connectionService;
    private Mock<IBiDiBMessageService> messageService;
    private Mock<IBiDiBMessageProcessor> messageProcessor;
    private Mock<IBiDiBNodesFactory> nodesFactory;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        connectionService = new Mock<IConnectionService>();
        messageService = new Mock<IBiDiBMessageService>();
        messageProcessor = new Mock<IBiDiBMessageProcessor>();
        nodesFactory = new Mock<IBiDiBNodesFactory>();

        Target = new ConnectOnlyStrategy(connectionService.Object, messageService.Object, messageProcessor.Object,
            nodesFactory.Object, NullLoggerFactory.Instance);
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldThrow_WhenConfigNull()
    {
        // Arrange

        // Act
        var action = async () => await Target.ConnectAsync(null);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'connectionConfig')");
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldOpenConnection()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.Unknown));

        // Act
        await Target.ConnectAsync(config);

        // Assert
        messageService.Verify(x => x.Activate());
        Target.ConnectionState.InterfaceState.Should().Be(InterfaceConnectionState.FullyConnected);
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldCloseConnection_WhenError()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        config.ConnectionType = InterfaceConnectionType.Unknown;
        connectionService.Setup(x => x.OpenConnectionAsync()).ThrowsAsync(new InvalidOperationException("Op"));

        // Act
        await Target.ConnectAsync(config);

        // Assert
        connectionService.Verify(x=>x.CloseConnection());
    }
}