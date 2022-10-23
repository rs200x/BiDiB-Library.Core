using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.core.Services.Interfaces;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class MessageProcessorTests : TestClass<BiDiBMessageProcessor>
    {
        private Mock<IBiDiBMessageService> messageService;
        private Mock<IBiDiBNodesFactory> nodesFactory;


        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            nodesFactory = new Mock<IBiDiBNodesFactory>();
            messageService = new Mock<IBiDiBMessageService>();

            Target = new BiDiBMessageProcessor(nodesFactory.Object, messageService.Object, NullLogger<BiDiBMessageProcessor>.Instance);
        }

        [TestMethod]
        public void GetChildNodes_ShouldNotSendNodeTabCount_WhenNodeNotExists()
        {
            // Arrange
            byte[] parent = { 0, 0, 0, 0 };
            nodesFactory.Setup(x => x.GetNode(parent)).Returns((BiDiBNode)null);

            // Act
            Target.GetChildNodes(parent);

            // Assert
            messageService.Verify(x=>x.SendMessages(It.IsAny<List<BiDiBOutputMessage>>()), Times.Never);
        }

        [TestMethod]
        public void GetChildNodes_ShouldNotSendNodeTabGetNext_WhenNoNodeTabCount()
        {
            // Arrange
            byte[] parent = { 0, 0, 0, 0 };
            var node = new BiDiBNode();
            nodesFactory.Setup(x => x.GetNode(parent)).Returns(node);

            // Act
            Target.GetChildNodes(parent);

            // Assert
            messageService.Verify(x=>x.SendMessages(It.Is<List<BiDiBOutputMessage>>(l=>l.Count ==1 && l[0].MessageType == BiDiBMessage.MSG_NODETAB_GETNEXT)), Times.Never);
        }

        [TestMethod]
        public void GetChildNodes_ShouldRequestAgainWhileCountZero()
        {
            // Arrange
            byte[] parent = { 0, 0, 0, 0 };
            var node = new BiDiBNode();
            nodesFactory.Setup(x => x.GetNode(parent)).Returns(node);

            IMessageReceiver messageReceiver = null;
            messageService.Setup(x => x.Register(It.IsAny<IMessageReceiver>()))
                .Callback<IMessageReceiver>((receiver) => messageReceiver = receiver);


            byte messageSendCount = 0;
            messageService.Setup(x => x.SendMessages(It.Is<List<BiDiBOutputMessage>>(l=>l.Count == 1 && l[0].MessageType == BiDiBMessage.MSG_NODETAB_GETALL)))
                .Callback<List<BiDiBOutputMessage>>(mi =>
                {
                var message = new NodeTabCountMessage(BiDiBMessageGenerator.GenerateMessage(new byte[] { 0 }, BiDiBMessage.MSG_NODETAB_COUNT, 0, messageSendCount));
                messageReceiver.ProcessMessage(message);
                messageSendCount++;
            });

            // Act
            Target.GetChildNodes(parent);

            // Assert
            messageSendCount.Should().Be(2);
        }

        [TestMethod]
        public void GetChildNodes_ShouldAbortRequestAgain()
        {
            // Arrange
            byte[] parent = { 0, 0, 0, 0 };
            var node = new BiDiBNode();
            nodesFactory.Setup(x => x.GetNode(parent)).Returns(node);

            IMessageReceiver messageReceiver = null;
            messageService.Setup(x => x.Register(It.IsAny<IMessageReceiver>()))
                .Callback<IMessageReceiver>((receiver) => messageReceiver = receiver);

            byte getAllSendCount = 0;
            messageService.Setup(x => x.SendMessages(It.Is<List<BiDiBOutputMessage>>(l=>l.Count == 1 && l[0].MessageType == BiDiBMessage.MSG_NODETAB_GETALL)))
                .Callback<List<BiDiBOutputMessage>>(mi =>
            {
                var message = new NodeTabCountMessage(BiDiBMessageGenerator.GenerateMessage(new byte[] { 0 }, BiDiBMessage.MSG_NODETAB_COUNT, 0, 0));
                messageReceiver.ProcessMessage(message);
                getAllSendCount++;
            });
            
            // Act
            Target.GetChildNodes(parent);

            // Assert
            getAllSendCount.Should().Be(6);
            messageService.Verify(x=>x.SendMessages(It.Is<List<BiDiBOutputMessage>>(l=>l.Count ==1 && l[0].MessageType == BiDiBMessage.MSG_NODETAB_GETNEXT)), Times.Never);
        }
    }
}