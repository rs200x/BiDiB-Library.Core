using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message;

[TestClass, TestCategory(TestCategory.UnitTest)]
public class StringMessageReceiverTests: TestClass<StringMessageReceiver>
{
    private Mock<IBiDiBNodesFactory> nodesFactory;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        nodesFactory = new Mock<IBiDiBNodesFactory>();

        Target = new StringMessageReceiver(nodesFactory.Object, NullLogger<StringMessageReceiver>.Instance);
    }

    [TestMethod]
    public void HandleStringMessage_ShouldSetProductName()
    {
        // arrange
        var messageBytes = GetBytes("0F-00-09-95-00-00-09-42-69-44-69-42-2D-49-46-32");
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // act
        Target.ProcessMessage(new StringMessage(messageBytes));
        
        // assert
        node.ProductName.Should().Be("BiDiB-IF2");
    }
    
    [TestMethod]
    public void HandleStringMessage_ShouldSetUserName()
    {
        // arrange
        var messageBytes = GetBytes("0D-00-0A-95-00-01-07-4D-65-69-6E-49-46-32");
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // act
        Target.ProcessMessage(new StringMessage(messageBytes));
        
        // assert
        node.UserName.Should().Be("MeinIF2");
    }
    
    [TestMethod]
    public void HandleStringMessage_ShouldNotUpdate_WhenNoNode()
    {
        // arrange
        var messageBytes = GetBytes("0D-00-0A-95-00-01-07-4D-65-69-6E-49-46-32");
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns((BiDiBNode)null);

        // act
        Target.ProcessMessage(new StringMessage(messageBytes));
        
        // assert
        node.ProductName.Should().BeNullOrEmpty();
        node.UserName.Should().BeNullOrEmpty();
    }
    
    [TestMethod]
    public void HandleStringMessage_ShouldNotUpdate_WhenWrongNamespace()
    {
        // arrange
        var messageBytes = GetBytes("0D-00-0A-95-01-01-07-4D-65-69-6E-49-46-32");
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // act
        Target.ProcessMessage(new StringMessage(messageBytes));
        
        // assert
        node.ProductName.Should().BeNullOrEmpty();
        node.UserName.Should().BeNullOrEmpty();
    }
    
    [TestMethod]
    public void HandleStringMessage_ShouldNotUpdate_WhenMessageNull()
    {
        // arrange
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // act
        Target.ProcessMessage(null);
        
        // assert
        node.ProductName.Should().BeNullOrEmpty();
        node.UserName.Should().BeNullOrEmpty();
    }
    
    [TestMethod]
    public void HandleStringMessage_ShouldNotUpdate_WhenNoStringMessage()
    {
        // arrange
        var messageBytes = GetBytes("0D-00-0A-95-00-01-07-4D-65-69-6E-49-46-32");
        var node = new BiDiBNode();
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);

        // act
        Target.ProcessMessage(new BiDiBInputMessage(messageBytes));
        
        // assert
        node.ProductName.Should().BeNullOrEmpty();
        node.UserName.Should().BeNullOrEmpty();
    }
}