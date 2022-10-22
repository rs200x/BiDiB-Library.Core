using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.BiDiB.Extensions;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Services;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBBoosterNodesManagerTests : TestClass<BiDiBBoosterNodesManager>
    {
        private Mock<IBiDiBMessageProcessor> messageProcessor;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            messageProcessor = new Mock<IBiDiBMessageProcessor>();


            Target = new BiDiBBoosterNodesManager(new Mock<IBiDiBMessageService>().Object, NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void NodeAdded_ShouldAddNode_WhenHasCommandStationFunction()
        {
            // Arrange
            var node = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance) { Address = new byte[] { 1} };
            node.SetUniqueId(new byte[] { 0x98, 0x00, 0x0D, 0x84, 0x00, 0x80, 0x0D });
            BiDiBBoosterNode addedNode = null;
            Target.BoosterAdded = n => addedNode = n;

            // Act
            Target.NodeAdded(node);

            // Assert
            addedNode.Should().NotBeNull();
            messageProcessor
                .Verify(x =>
                    x.SendMessage(It.Is<CommandStationSetStateMessage>(m => m.State == CommandStationState.BIDIB_CS_STATE_QUERY && m.Address.GetArrayValue() == 1)));
        }        
        
        [TestMethod]
        public void NodeAdded_ShouldAddNode_WhenHasBoosterFunction()
        {
            // Arrange
            var node = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance) { Address = new byte[] { 0x00 } };
            node.SetUniqueId(new byte[] { 0xAF, 0x00, 0x0D, 0x84, 0x00, 0x80, 0x0D });
            BiDiBBoosterNode addedNode = null;
            Target.BoosterAdded = n => addedNode = n;

            // Act
            Target.NodeAdded(node);

            // Assert
            addedNode.Should().NotBeNull();
            messageProcessor.Verify(x=>x.SendMessage(node, BiDiBMessage.MSG_BOOST_QUERY));
        } 
        
        [TestMethod]
        public void NodeAdded_ShouldSetMaxCurrent_WhenFeature()
        {
            // Arrange
            var node = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance) { Address = new byte[] { 0x00 }, Features = new []{ new Feature {FeatureId = (byte)BiDiBFeature.FEATURE_BST_AMPERE, Value = 10}, }};
            node.SetUniqueId(new byte[] { 0xAF, 0x00, 0x0D, 0x84, 0x00, 0x80, 0x0D });
            BiDiBBoosterNode addedNode = null;
            Target.BoosterAdded = n => addedNode = n;

            // Act
            Target.NodeAdded(node);

            // Assert
            addedNode.MaxCurrent.Should().Be(10);
        }
        
        [TestMethod]
        public void HandleBoostStat_ShouldApplyStates()
        {
            // Arrange
            var node = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance) { Address = new byte[] { 0x00 } };
            node.SetUniqueId(new byte[] { 0xAF, 0x00, 0x0D, 0x84, 0x00, 0x80, 0x0D });
            Target.NodeAdded(node);

            byte[] bytes = GetBytes("04-00-67-B0-C0");

            // Act
            Target.ProcessMessage(new BoostStatMessage(bytes));

            // Assert
            var boosterNode = Target.Boosters.ElementAt(0);
            boosterNode.BoosterControl.Should().Be(BoosterControl.Local);
            boosterNode.BoosterState.Should().Be(BoosterState.BIDIB_BST_STATE_ON);
        }
    }
}