using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message;

[TestClass, TestCategory(TestCategory.UnitTest)]
public class MainMessageReceiverTests : TestClass<MainMessageReceiver>
{
    private Mock<IBiDiBMessageService> messageService;
    private Mock<IBiDiBNodesFactory> nodesFactory;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        messageService = new Mock<IBiDiBMessageService>();
        nodesFactory = new Mock<IBiDiBNodesFactory>();

        Target = new MainMessageReceiver(messageService.Object, nodesFactory.Object, NullLoggerFactory.Instance);
    }

    [TestMethod]
    public void HandleNodeNew_ShouldSendNodeChangeAck()
    {
        // Arrange
        var bytes = GetBytes("0C-00-27-8D-04-01-45-00-0D-79-00-01-EC-3E-FE");
        var message = new NodeNewMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(x =>
            x.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion));
    }

    [TestMethod]
    public async Task HandleNodeNew_ShouldTriggerCreateNewNode()
    {
        // Arrange
        var bytes = GetBytes("0C-00-27-8D-04-01-45-00-0D-79-00-01-EC-3E-FE");
        var message = new NodeNewMessage(bytes);

        // Act
        Target.ProcessMessage(message);
        await Task.Delay(200);

        // Assert
        nodesFactory.Verify(x => x.CreateNode(message.NodeAddress, message.UniqueId));
    }

    [TestMethod]
    public void HandleNodeLost_ShouldSendNodeChangeAck()
    {
        // Arrange
        var bytes = GetBytes("0C-00-28-8C-05-01-45-00-0D-79-00-01-EC-B3-FE");
        var message = new NodeLostMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(x =>
            x.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion));
    }

    [TestMethod]
    public void HandleNodeLost_ShouldResetAndRemoveNode()
    {
        // Arrange
        var bytes = GetBytes("0C-00-28-8C-05-01-45-00-0D-79-00-01-EC-B3-FE");
        var message = new NodeLostMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        nodesFactory.Verify(x => x.RemoveNode(message.NodeAddress));
    }

    [TestMethod]
    public void HandleFeedbackMultiple_ShouldMirrorMessage()
    {
        // Arrange
        var bytes = GetBytes("06-00-25-A2-00-08-A0-D3-FE");
        var message = new FeedbackMultipleMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(x =>
            x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_MULTIPLE, message.Address, message.MessageParameters));
    }


    [TestMethod]
    public void HandleFeedbackMultiple_ShouldUpdateOccupancies()
    {
        // Arrange
        var bytes = GetBytes("06-00-25-A2-00-08-A0-D3-FE");
        var message = new FeedbackMultipleMessage(bytes);

        FeedbackPort[] ports =
        {
            new FeedbackPort { Number = 0, IsFree = false },
            new FeedbackPort { Number = 1, IsFree = false },
            new FeedbackPort { Number = 2, IsFree = false },
            new FeedbackPort { Number = 3, IsFree = false },
            new FeedbackPort { Number = 4, IsFree = false },
            new FeedbackPort { Number = 5, IsFree = true },
            new FeedbackPort { Number = 6, IsFree = false },
            // set up only 7 ports to check out of range handling
        };

        var node = new BiDiBNode { FeedbackPorts = ports };

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        ports[0].IsFree.Should().BeTrue();
        ports[1].IsFree.Should().BeTrue();
        ports[2].IsFree.Should().BeTrue();
        ports[3].IsFree.Should().BeTrue();
        ports[4].IsFree.Should().BeTrue();
        ports[5].IsFree.Should().BeFalse();
        ports[6].IsFree.Should().BeTrue();
    }

    [TestMethod]
    public void HandleFeedbackMessage_ShouldMirrorMessage_ForFree()
    {
        // Arrange
        var bytes = GetBytes("04-00-D6-A1-06-20-FE");
        var message = new FeedbackMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(x =>
            x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_FREE, message.Address, message.MessageParameters));
    }

    [TestMethod]
    public void HandleFeedbackMessage_ShouldSetIsFreeTrueAndClearOccupanciesForPort_WhenFree()
    {
        // Arrange
        var bytes = GetBytes("04-00-D6-A1-06-20-FE");
        var message = new FeedbackMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
        var feedbackPort = new FeedbackPort { Number = 6, IsFree = false };
        feedbackPort.AddOccupancy(new OccupancyInfo());
        node.FeedbackPorts[6] = feedbackPort;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        feedbackPort.IsFree.Should().BeTrue();
        feedbackPort.Occupancies.Should().BeNull();
    }

    [TestMethod]
    public void HandleFeedbackMessage_ShouldMirrorMessage_ForOcc()
    {
        // Arrange
        var bytes = GetBytes("04-00-F2-A0-05-0C-FE");
        var message = new FeedbackOccupiedMessage(bytes);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(x =>
            x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_OCC, message.Address, message.MessageParameters));
    }

    [TestMethod]
    public void HandleFeedbackMessage_ShouldSetIsFreeFalseForPort_WhenOcc()
    {
        // Arrange
        var bytes = GetBytes("04-00-F2-A0-05-0C-FE");
        var message = new FeedbackOccupiedMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
        var feedbackPort = new FeedbackPort { Number = 5, IsFree = true };
        node.FeedbackPorts[5] = feedbackPort;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        feedbackPort.IsFree.Should().BeFalse();
    }

    [TestMethod]
    public void HandleFeedbackAddressMessage_ShouldAddOccupancyInfoForAddresses()
    {
        // Arrange
        var bytes = GetBytes("08-00-18-A3-06-04-80-03-00-95-FE");
        var addressMessage = new FeedbackAddressMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
        var feedbackPort = new FeedbackPort { Number = 6 };
        node.FeedbackPorts[6] = feedbackPort;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(addressMessage);

        // Assert
        feedbackPort.Occupancies.Should().HaveCount(2);
        feedbackPort.Occupancies[0].Address.Should().Be(4);
        feedbackPort.Occupancies[0].Direction.Should().Be(DecoderDirection.BackwardDirection);
        feedbackPort.Occupancies[1].Address.Should().Be(3);
        feedbackPort.Occupancies[1].Direction.Should().Be(DecoderDirection.ForwardDirection);
    }

    [TestMethod]
    public void HandleFeedbackAddressMessage_ShouldClearOccupancyInfos_WhenAddressZero()
    {
        // Arrange
        var bytes = GetBytes("06-00-18-A3-06-00-00-95-FE");
        var addressMessage = new FeedbackAddressMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
        var feedbackPort = new FeedbackPort { Number = 6 };
        feedbackPort.AddOccupancy(new OccupancyInfo { Address = 1 });
        node.FeedbackPorts[6] = feedbackPort;


        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(addressMessage);

        // Assert
        feedbackPort.Occupancies.Should().BeNull();
    }

    [TestMethod]
    public void HandleFeedbackAddressMessage_ShouldRemoveOccupancyInfo_WhenAddressNotContained()
    {
        // Arrange
        var bytes = GetBytes("06-00-18-A3-06-02-00-95-FE");
        var addressMessage = new FeedbackAddressMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
        var feedbackPort = new FeedbackPort { Number = 6 };
        feedbackPort.AddOccupancy(new OccupancyInfo { Address = 1 });
        feedbackPort.AddOccupancy(new OccupancyInfo { Address = 2 });
        node.FeedbackPorts[6] = feedbackPort;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(addressMessage);

        // Assert
        feedbackPort.Occupancies.Should().HaveCount(1);
        feedbackPort.Occupancies[0].Address.Should().Be(2);
    }

    [TestMethod]
    public void HandleFeedbackSpeedMessage_ShouldSetSpeedForAllOccupanciesMatchDecoderAddress()
    {
        // Arrange
        var bytes = GetBytes("07-00-0A-A6-EC-A7-09-00-8A-FE");
        var message = new FeedbackSpeedMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[2] };
        var feedbackPort1 = new FeedbackPort { Number = 3 };
        feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 1, Speed = 1 });
        feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 10220, Speed = 1 });
        var feedbackPort2 = new FeedbackPort { Number = 6 };
        feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 1, Speed = 1 });
        feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 10220, Speed = 1 });
        node.FeedbackPorts[0] = feedbackPort1;
        node.FeedbackPorts[1] = feedbackPort2;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        feedbackPort1.Occupancies[0].Speed.Should().Be(1);
        feedbackPort1.Occupancies[1].Speed.Should().Be(9);
        feedbackPort1.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
        feedbackPort2.Occupancies[0].Speed.Should().Be(1);
        feedbackPort2.Occupancies[1].Speed.Should().Be(9);
        feedbackPort2.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
    }
    
    [TestMethod]
    public void HandleFeedbackSpeedMessage_ShouldSetSpeedForGlobalOccupancy()
    {
        // Arrange
        var bytes = GetBytes("07-00-0A-A6-EC-A7-09-00-8A-FE");
        var message = new FeedbackSpeedMessage(bytes);

        var node = new BiDiBNode();

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        node.GlobalOccupancies[10220].Speed.Should().Be(9);
        node.GlobalOccupancies[10220].Direction.Should().Be(DecoderDirection.BackwardDirection); 
    }

    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldDynValueForAllOccupanciesMatchDecoderAddress()
    {
        // Arrange
        var bytes = GetBytes("08-00-00-AA-06-03-80-02-E3-C9-FE");
        var message = new FeedbackDynStateMessage(bytes);

        var node = new BiDiBNode { FeedbackPorts = new FeedbackPort[2] };
        var feedbackPort1 = new FeedbackPort { Number = 3 };
        feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 1, Temperature = 0 });
        feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 3, Temperature = 0 });
        var feedbackPort2 = new FeedbackPort { Number = 6 };
        feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 1, Temperature = 0 });
        feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 3, Temperature = 0 });
        node.FeedbackPorts[0] = feedbackPort1;
        node.FeedbackPorts[1] = feedbackPort2;

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        feedbackPort1.Occupancies[0].Temperature.Should().Be(0);
        feedbackPort1.Occupancies[1].Temperature.Should().Be(-29);
        feedbackPort1.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
        feedbackPort2.Occupancies[0].Temperature.Should().Be(0);
        feedbackPort2.Occupancies[1].Temperature.Should().Be(-29);
        feedbackPort2.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForSignalQuality()
    {
        HandleFeedbackDynStateMessage(
            DynState.SignalQuality, 
            o => o.Quality ?? 0, 
            227);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForTemperature()
    {
        HandleFeedbackDynStateMessage(
            DynState.Temperature, 
            o => o.Temperature ?? 0, 
            -29);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForContainer1()
    {
        HandleFeedbackDynStateMessage(
            DynState.Container1, 
            o => o.Container1 ?? 0,
            227);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForContainer2()
    {
        HandleFeedbackDynStateMessage(
            DynState.Container2, 
            o => o.Container2 ?? 0,
            227);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForContainer3()
    {
        HandleFeedbackDynStateMessage(
            DynState.Container3, 
            o => o.Container3 ?? 0,
            227);
    }
    
    [TestMethod]
    public void HandleFeedbackDynStateMessage_ShouldSetDynValue_ForTrackVoltage()
    {
        HandleFeedbackDynStateMessage(
            DynState.TrackVoltage, 
            o => o.TrackVoltage ?? 0,
            22.7);
    }
    
    [TestMethod]
    [DataRow(0, NodeState.Ok)]
    [DataRow(1, NodeState.Identifying)]
    public void HandleIdentifyState_ShouldSetNodeState(int stateValue, NodeState state)
    {
        // Arrange
        var bytes = GetBytes($"04-00-01-87-0{stateValue}");
        var message = new SysIdentifyStateMessage(bytes);

        var node = new BiDiBNode();

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        node.State.Should().Be(state);
    }

    [TestMethod]
    public void HandleIdentifyState_ShouldNotSetState_WhenMessageNull()
    {
        // Arrange
        var bytes = GetBytes($"04-00-01-87-01");
        var message = new BiDiBInputMessage(bytes);

        var node = new BiDiBNode { State = NodeState.Unavailable };

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        node.State.Should().Be(NodeState.Unavailable);
    }

    [TestMethod]
    public void HandleIdentifyState_ShouldNotSetState_WhenNodeNull()
    {
        // Arrange
        var bytes = GetBytes("04-00-01-87-01");
        var message = new SysIdentifyStateMessage(bytes);

        var node = new BiDiBNode { State = NodeState.Unavailable };

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns((BiDiBNode) null);

        // Act
        Target.ProcessMessage(message);

        // Assert
        node.State.Should().Be(NodeState.Unavailable);
    }

    [TestMethod]
    public void HandleFeedbackPositionMessage_ShouldNotSetPosition_WhenMessageNull()
    {
        // Arrange
        var bytes = GetBytes("0D-01-00-1D-B8-00-03-14-01-DB-01-69-02-14-05-01-00-1E-A1-04-30-FE");
        var message = new BiDiBInputMessage(bytes);
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns((BiDiBNode) null);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(
            x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_POSITION, It.IsAny<byte[]>(), It.IsAny<byte[]>()),
            Times.Never);
    }

    [TestMethod]
    public void HandleFeedbackPositionMessage_ShouldNotSetPosition_WhenNodeNull()
    {
        // Arrange
        var bytes = GetBytes("09-01-00-2D-AC-01-00-00-03-00-22-FE");
        var message = new FeedbackPositionMessage(bytes);
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns((BiDiBNode) null);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(
            x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_POSITION, It.IsAny<byte[]>(), It.IsAny<byte[]>()),
            Times.Once);
    }

    [TestMethod]
    public void HandleFeedbackPositionMessage_ShouldSetPositionOccupancy()
    {
        // Arrange
        var bytes = GetBytes("09-01-00-2D-AC-01-00-00-03-00-22-FE");
        var message = new FeedbackPositionMessage(bytes);

        var node = new BiDiBNode();
        var positionPort = new PositionPort(10);
        positionPort.AddOccupancy(new OccupancyInfo { Address = 1 });
        node.UpdatePosition(positionPort);

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        messageService.Verify(
            x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_POSITION, It.IsAny<byte[]>(), It.IsAny<byte[]>()),
            Times.Once);
        node.PositionPorts.Should().HaveCount(2);
        node.PositionPorts[10].Position.Should().Be(10);
        node.PositionPorts[10].Occupancies.Should().BeNull();
        node.PositionPorts[3].Position.Should().Be(3);
        node.PositionPorts[3].Occupancies.Should().HaveCount(1);
        node.PositionPorts[3].Occupancies[0].Address.Should().Be(1);
    }
    
    private void HandleFeedbackDynStateMessage(DynState dynState, Func<OccupancyInfo, double> propertyExpression, double expectedValue)
    {
        // Arrange
        var bytes = GetBytes($"08-00-00-AA-06-03-80-{(int)dynState:X2}-E3-C9-FE");
        var message = new FeedbackDynStateMessage(bytes);

        var node = new BiDiBNode();

        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // Act
        Target.ProcessMessage(message);

        // Assert
        var propValue = propertyExpression(node.GlobalOccupancies[3]);
        (Math.Abs(propValue - expectedValue) < 0.1).Should().BeTrue();
        node.GlobalOccupancies[3].Direction.Should().Be(DecoderDirection.BackwardDirection);
    }
}