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
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.ConnectionStrategies;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Services;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class DefaultStrategyTests : TestClass<DefaultStrategy>
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

        Target = new DefaultStrategy(connectionService.Object, messageService.Object, messageProcessor.Object,
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
    public async Task ConnectAsync_ShouldNotContinue_WhenNotConnected()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.Disconnected, InterfaceConnectionType.Unknown));

        // Act
        await Target.ConnectAsync(config);

        // Assert
        messageService.Verify(x => x.Activate());
        connectionService.Verify(x=>x.CloseConnection());
        messageProcessor.Verify(x=>x.SendMessage(It.IsAny<BiDiBNode>(), It.IsAny<BiDiBMessage>(), It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldNotContinue_WhenPartlyConnected()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.PartiallyConnected, InterfaceConnectionType.NetBiDiB));

        // Act
        await Target.ConnectAsync(config);

        // Assert
        messageService.Verify(x=>x.Activate());
        connectionService.Verify(x => x.CloseConnection(), Times.Never);
        messageProcessor.Verify(x => x.SendMessage(It.IsAny<BiDiBNode>(), It.IsAny<BiDiBMessage>(), It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldNotDisable_WhenSerialOverTcp()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialOverTcp));

        messageProcessor
            .Setup(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>())).Returns((SysMagicMessage)null);

        // Act
        await Target.ConnectAsync(config);

        // Assert
        connectionService.Verify(x => x.CloseConnection());
        messageService.Verify(x => x.Deactivate());
        Target.ConnectionState.InterfaceState.Should().Be(InterfaceConnectionState.Disconnected);
        messageProcessor
            .Verify(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>()));
        messageProcessor
            .Verify(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_DISABLE,
                It.IsAny<int>(), It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public async Task ConnectAsync_ShouldClose_WhenMagicNull()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.Unknown));

        messageProcessor
            .Setup(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>())).Returns((SysMagicMessage)null);

        // Act
        await Target.ConnectAsync(config);

        // Assert
        connectionService.Verify(x => x.CloseConnection());
        messageService.Verify(x=>x.Deactivate());
        Target.ConnectionState.InterfaceState.Should().Be(InterfaceConnectionState.Disconnected);
        messageProcessor
            .Verify(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>()));
        messageProcessor
            .Verify(x => x.SendMessage(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_DISABLE, It.IsAny<byte[]>()));
    }
    
    [TestMethod]
    public async Task ConnectAsync_ShouldEnableRoot()
    {
        // Arrange
        var config = Mock.Of<IConnectionConfig>();
        connectionService.Setup(x => x.OpenConnectionAsync()).ReturnsAsync(
            new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.Unknown));

        byte[] bytes = GetBytes("05-00-00-81-FE-AF-89-FE");
        messageProcessor
            .Setup(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>())).Returns(new SysMagicMessage(bytes));

        var rootNode = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);
        nodesFactory.Setup(x => x.GetRootNode()).Returns(rootNode);

        connectionService.Setup(x => x.GetConnectionName()).Returns("CON");

        // Act
        await Target.ConnectAsync(config);

        // Assert
        messageService.Verify(x=>x.Cleanup());
        nodesFactory.Verify(x=>x.Reset());
        Target.ConnectionState.InterfaceState.Should().Be(InterfaceConnectionState.FullyConnected);
        Target.ConnectionState.ConnectionName.Should().Be("CON");
        messageProcessor
            .Verify(x => x.SendMessage<SysMagicMessage>(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_GET_MAGIC,
                It.IsAny<int>(), It.IsAny<byte[]>()));
        messageProcessor
            .Verify(x => x.SendMessage(It.IsAny<BiDiBNode>(), BiDiBMessage.MSG_SYS_DISABLE, It.IsAny<byte[]>()));
        messageProcessor
            .Verify(x => x.SendMessage(rootNode, BiDiBMessage.MSG_SYS_ENABLE, It.IsAny<byte[]>()));
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

    [TestMethod]
    public void Disconnect_ShouldResetNodes()
    {
        // Arrange

        // Act
        Target.Disconnect();

        // Assert

        nodesFactory.Verify(x => x.Reset());
    }
}