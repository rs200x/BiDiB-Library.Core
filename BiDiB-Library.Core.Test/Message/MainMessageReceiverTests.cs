using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Services.Interfaces;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace org.bidib.netbidibc.core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class MainMessageReceiverTests : TestClass
    {

        private MainMessageReceiver target;
        private Mock<IBiDiBMessageService> messageService;
        private Mock<IBiDiBNodesFactory> nodesFactory;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            messageService = new Mock<IBiDiBMessageService>();
            nodesFactory = new Mock<IBiDiBNodesFactory>();

            target = new MainMessageReceiver(messageService.Object, nodesFactory.Object, NullLogger<MainMessageReceiver>.Instance);
        }

        [TestMethod]
        public void HandleNodeNew_ShouldSendNodeChangeAck()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-00-27-8D-04-01-45-00-0D-79-00-01-EC-3E-FE");
            NodeNewMessage message = new NodeNewMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            messageService.Verify(x => x.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion));
        }

        [TestMethod]
        public async Task HandleNodeNew_ShouldTriggerCreateNewNode()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-00-27-8D-04-01-45-00-0D-79-00-01-EC-3E-FE");
            NodeNewMessage message = new NodeNewMessage(bytes);

            // Act
            target.ProcessMessage(message);
            await Task.Delay(200);

            // Assert
            nodesFactory.Verify(x => x.CreateNode(message.NodeAddress, message.UniqueId));
        }

        [TestMethod]
        public void HandleNodeLost_ShouldSendNodeChangeAck()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-00-28-8C-05-01-45-00-0D-79-00-01-EC-B3-FE");
            NodeLostMessage message = new NodeLostMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            messageService.Verify(x => x.SendMessage(BiDiBMessage.MSG_NODE_CHANGED_ACK, message.Address, message.TableVersion));
        }

        [TestMethod]
        public void HandleNodeLost_ShouldResetAndRemoveNode()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-00-28-8C-05-01-45-00-0D-79-00-01-EC-B3-FE");
            NodeLostMessage message = new NodeLostMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            nodesFactory.Verify(x => x.RemoveNode(message.NodeAddress));
        }

        [TestMethod]
        public void HandleFeedbackMultiple_ShouldMirrorMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("06-00-25-A2-00-08-A0-D3-FE");
            FeedbackMultipleMessage message = new FeedbackMultipleMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            messageService.Verify(x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_MULTIPLE, message.Address, message.MessageParameters));

        }


        [TestMethod]
        public void HandleFeedbackMultiple_ShouldUpdateOccupancies()
        {
            // Arrange
            byte[] bytes = GetBytes("06-00-25-A2-00-08-A0-D3-FE");
            FeedbackMultipleMessage message = new FeedbackMultipleMessage(bytes);

            FeedbackPort[] ports = {
                new FeedbackPort {Number = 0, IsFree = false},
                new FeedbackPort {Number = 1, IsFree = false},
                new FeedbackPort {Number = 2, IsFree = false},
                new FeedbackPort {Number = 3, IsFree = false},
                new FeedbackPort {Number = 4, IsFree = false},
                new FeedbackPort {Number = 5, IsFree = true},
                new FeedbackPort {Number = 6, IsFree = false},
                // set up only 7 ports to check out of range handling
            };

            BiDiBNode node = new BiDiBNode { FeedbackPorts = ports };

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(message);

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
            byte[] bytes = GetBytes("04-00-D6-A1-06-20-FE");
            FeedbackMessage message = new FeedbackMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            messageService.Verify(x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_FREE, message.Address, message.MessageParameters));
        }

        [TestMethod]
        public void HandleFeedbackMessage_ShouldSetIsFreeTrueAndClearOccupanciesForPort_WhenFree()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-D6-A1-06-20-FE");
            FeedbackMessage message = new FeedbackMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
            FeedbackPort feedbackPort = new FeedbackPort { Number = 6, IsFree = false };
            feedbackPort.AddOccupancy(new OccupancyInfo());
            node.FeedbackPorts[6] = feedbackPort;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(message);

            // Assert
            feedbackPort.IsFree.Should().BeTrue();
            feedbackPort.Occupancies.Should().BeNull();
        }

        [TestMethod]
        public void HandleFeedbackMessage_ShouldMirrorMessage_ForOcc()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-F2-A0-05-0C-FE");
            FeedbackOccupiedMessage message = new FeedbackOccupiedMessage(bytes);

            // Act
            target.ProcessMessage(message);

            // Assert
            messageService.Verify(x => x.SendMessage(BiDiBMessage.MSG_BM_MIRROR_OCC, message.Address, message.MessageParameters));
        }

        [TestMethod]
        public void HandleFeedbackMessage_ShouldSetIsFreeFalseForPort_WhenOcc()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-F2-A0-05-0C-FE");
            FeedbackOccupiedMessage message = new FeedbackOccupiedMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
            FeedbackPort feedbackPort = new FeedbackPort { Number = 5, IsFree = true };
            node.FeedbackPorts[5] = feedbackPort;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(message);

            // Assert
            feedbackPort.IsFree.Should().BeFalse();
        }

        [TestMethod]
        public void HandleFeedbackAddressMessage_ShouldAddOccupancyInfoForAddresses()
        {
            // Arrange
            byte[] bytes = GetBytes("08-00-18-A3-06-04-80-03-00-95-FE");
            FeedbackAddressMessage addressMessage = new FeedbackAddressMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
            FeedbackPort feedbackPort = new FeedbackPort { Number = 6 };
            node.FeedbackPorts[6] = feedbackPort;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(addressMessage);

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
            byte[] bytes = GetBytes("06-00-18-A3-06-00-00-95-FE");
            FeedbackAddressMessage addressMessage = new FeedbackAddressMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
            FeedbackPort feedbackPort = new FeedbackPort { Number = 6 };
            feedbackPort.AddOccupancy(new OccupancyInfo { Address = 1 });
            node.FeedbackPorts[6] = feedbackPort;


            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(addressMessage);

            // Assert
            feedbackPort.Occupancies.Should().BeNull();
        }

        [TestMethod]
        public void HandleFeedbackAddressMessage_ShouldRemoveOccupancyInfo_WhenAddressNotContained()
        {
            // Arrange
            byte[] bytes = GetBytes("06-00-18-A3-06-02-00-95-FE");
            FeedbackAddressMessage addressMessage = new FeedbackAddressMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[7] };
            FeedbackPort feedbackPort = new FeedbackPort { Number = 6 };
            feedbackPort.AddOccupancy(new OccupancyInfo { Address = 1 });
            feedbackPort.AddOccupancy(new OccupancyInfo { Address = 2 });
            node.FeedbackPorts[6] = feedbackPort;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(addressMessage);

            // Assert
            feedbackPort.Occupancies.Should().HaveCount(1);
            feedbackPort.Occupancies[0].Address.Should().Be(2);
        }

        [TestMethod]
        public void HandleFeedbackSpeedMessage_ShouldSetSpeedForAllOccupanciesMatchDecoderAddress()
        {
            // Arrange
            byte[] bytes = GetBytes("07-00-0A-A6-EC-A7-09-00-8A-FE");
            FeedbackSpeedMessage message = new FeedbackSpeedMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[2] };
            FeedbackPort feedbackPort1 = new FeedbackPort { Number = 3 };
            feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 1, Speed = 1 });
            feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 10220, Speed = 1 });
            FeedbackPort feedbackPort2 = new FeedbackPort { Number = 6 };
            feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 1, Speed = 1 });
            feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 10220, Speed = 1 });
            node.FeedbackPorts[0] = feedbackPort1;
            node.FeedbackPorts[1] = feedbackPort2;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(message);

            // Assert
            feedbackPort1.Occupancies[0].Speed.Should().Be(1);
            feedbackPort1.Occupancies[1].Speed.Should().Be(9);
            feedbackPort1.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
            feedbackPort2.Occupancies[0].Speed.Should().Be(1);
            feedbackPort2.Occupancies[1].Speed.Should().Be(9);
            feedbackPort2.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
        }

        [TestMethod]
        public void HandleFeedbackDynStateMessage_ShouldDynValueForAllOccupanciesMatchDecoderAddress()
        {
            // Arrange
            byte[] bytes = GetBytes("08-00-00-AA-06-03-80-02-E3-C9-FE");
            FeedbackDynStateMessage message = new FeedbackDynStateMessage(bytes);

            BiDiBNode node = new BiDiBNode { FeedbackPorts = new FeedbackPort[2] };
            FeedbackPort feedbackPort1 = new FeedbackPort { Number = 3 };
            feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 1, Temperature = 0 });
            feedbackPort1.AddOccupancy(new OccupancyInfo { Address = 3, Temperature = 0 });
            FeedbackPort feedbackPort2 = new FeedbackPort { Number = 6 };
            feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 1, Temperature = 0 });
            feedbackPort2.AddOccupancy(new OccupancyInfo { Address = 3, Temperature = 0 });
            node.FeedbackPorts[0] = feedbackPort1;
            node.FeedbackPorts[1] = feedbackPort2;

            nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

            // Act
            target.ProcessMessage(message);

            // Assert
            feedbackPort1.Occupancies[0].Temperature.Should().Be(0);
            feedbackPort1.Occupancies[1].Temperature.Should().Be(-29);
            feedbackPort1.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
            feedbackPort2.Occupancies[0].Temperature.Should().Be(0);
            feedbackPort2.Occupancies[1].Temperature.Should().Be(-29);
            feedbackPort2.Occupancies[1].Direction.Should().Be(DecoderDirection.BackwardDirection);
        }
    }
}