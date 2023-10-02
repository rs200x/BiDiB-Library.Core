using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services;
using org.bidib.Net.Core.Utils;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Services
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


            Target = new BiDiBBoosterNodesManager(NullLoggerFactory.Instance);
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
    }
}