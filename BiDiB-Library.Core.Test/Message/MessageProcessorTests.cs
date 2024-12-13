using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message;

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

        Target = new BiDiBMessageProcessor(nodesFactory.Object, messageService.Object, NullLoggerFactory.Instance);
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
            .Callback<ICollection<BiDiBOutputMessage>>(_ =>
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
            .Callback<ICollection<BiDiBOutputMessage>>(_ =>
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

    [TestMethod]
    public void SendMessage_ShouldThrow_WhenNodeNull()
    {
        // Arrange
        
        // Act
        var action = () => Target.SendMessage(null, BiDiBMessage.MSG_BM_CV, Array.Empty<byte>());
        
        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'node')");
    }
    
    [TestMethod]
    public void SendMessage_ShouldForwardToService()
    {
        // Arrange
        
        // Act
        Target.SendMessage(new BiDiBNode(), BiDiBMessage.MSG_BM_CV, Array.Empty<byte>());
        
        // Assert
       messageService.Verify(x=>x.SendMessage(BiDiBMessage.MSG_BM_CV, It.IsAny<byte[]>(), It.IsAny<byte[]>()));
    } 
    
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SendMessage_ShouldReturnReceivedMessage_WhenFromOtherNode(bool acceptOther)
    {
        // Arrange
        IMessageReceiver registeredReceiver = null;
        messageService.Setup(x => x.Register(It.IsAny<IMessageReceiver>()))
            .Callback((IMessageReceiver receiver) => registeredReceiver = receiver);

        var vendorMessage = new VendorMessage(GetBytes("0C-01-00-03-93-03-32-34-30-03-31-34-39-FD-DD-FE"));
        messageService.Setup(x => x.SendMessages(It.IsAny<ICollection<BiDiBOutputMessage>>()))
            .Callback((ICollection<BiDiBOutputMessage> _) => registeredReceiver?.ProcessMessage(vendorMessage));
        
        // Act
        var response = Target.SendMessage<VendorMessage>(new VendorGetMessage([0], "1"), timeout:10, acceptOther);
        
        // Assert
        if (acceptOther)
        {
            response.Should().Be(vendorMessage);
        }
        else
        {
            response.Should().BeNull();
        }
    } 
}