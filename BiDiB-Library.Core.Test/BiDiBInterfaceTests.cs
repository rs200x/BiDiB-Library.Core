using System;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBInterfaceTests
    {
        private Mock<IConnectionService> connectionService;
        private Mock<IBiDiBMessageService> messageService;
        private Mock<IBiDiBMessageProcessor> messageProcessor;
        private Mock<IBiDiBNodesFactory> nodesFactory;
        private Mock<IBiDiBBoosterNodesManager> boosterNodesManager;
        private BiDiBInterface Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            connectionService = new Mock<IConnectionService>();
            messageService = new Mock<IBiDiBMessageService>();
            messageProcessor = new Mock<IBiDiBMessageProcessor>();
            nodesFactory = new Mock<IBiDiBNodesFactory>();
            nodesFactory.SetupProperty(x => x.NodeAdded);
            nodesFactory.SetupProperty(x => x.NodeRemoved);
            boosterNodesManager = new Mock<IBiDiBBoosterNodesManager>();

            Target = new BiDiBInterface(connectionService.Object, messageService.Object, messageProcessor.Object, nodesFactory.Object, 
                boosterNodesManager.Object, Array.Empty<IMessageReceiver>(), Array.Empty<IConnectionStrategy>(), NullLogger<BiDiBInterface>.Instance);
        }

        [TestMethod]
        public void HandleNodesFactoryNodeAdded_ShouldEnableNode_WhenRootEnabled()
        {
            // Arrange
            var root = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);
            root.Enable();
            var newNode = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);

            nodesFactory.Setup(x => x.GetRootNode()).Returns(root);            
            Target.Initialize();

            // Act
            nodesFactory.Object.NodeAdded.Invoke(newNode);

            // Assert
            newNode.IsEnabled.Should().BeTrue();
            messageProcessor.Verify(x => x.SendMessage(newNode, BiDiBMessage.MSG_SYS_ENABLE), Times.Once);
        }

        [TestMethod]
        public void HandleNodesFactoryNodeAdded_ShouldNotEnableNode_WhenNewNodeIsRootNode()
        {
            // Arrange
            var root = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);

            nodesFactory.Setup(x => x.GetRootNode()).Returns(root);
            Target.Initialize();

            // Act
            nodesFactory.Object.NodeAdded.Invoke(root);

            // Assert
            root.IsEnabled.Should().BeFalse();
            messageProcessor.Verify(x => x.SendMessage(root, BiDiBMessage.MSG_SYS_ENABLE), Times.Never);
        }

        [TestMethod]
        public void HandleNodesFactoryNodeAdded_ShouldGetChildNodes_WhenNodeHasSubNodesFunctions()
        {
            // Arrange
            var root = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);
            var newNode = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance) { UniqueId = 36028854766141696, Address = new byte[] { 1 } };

            nodesFactory.Setup(x => x.GetRootNode()).Returns(root);
            Target.Initialize();

            // Act
            nodesFactory.Object.NodeAdded.Invoke(newNode);

            // Assert
            messageProcessor.Verify(x => x.GetChildNodes(newNode.Address), Times.Once);
        }
    }
}