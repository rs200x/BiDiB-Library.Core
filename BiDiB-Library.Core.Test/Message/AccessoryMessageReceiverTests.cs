using FluentAssertions;
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
public class AccessoryMessageReceiverTests : TestClass<AccessoryMessageReceiver>
{
    private Mock<IBiDiBNodesFactory> nodesFactory;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        nodesFactory = new Mock<IBiDiBNodesFactory>();

        Target = new AccessoryMessageReceiver(nodesFactory.Object);
    }
    
    [TestMethod]
    public void HandleAccessoryState_ShouldSetAccessory()
    {
        // arrange
        var messageBytes = GetBytes("0D-01-00-1D-B8-00-03-14-01-DB-01-69-02-14");
        var accessory = new Accessory { Number = 0 };
        var node = new BiDiBNode
        {
            Accessories = new[]
            {
                accessory
            }
        };
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>()))
            .Returns(node);
        
        // act
        Target.ProcessMessage(new AccessoryStateMessage(messageBytes));
        
        // assert
        accessory.Number.Should().Be(0);
        accessory.ActiveAspect.Should().Be(3);
        accessory.ExecutionState.Should().Be(AccessoryExecutionState.Running);
    }
    
    [TestMethod]
    public void HandleAccessoryState_ShouldNotUpdate_WhenNoNode()
    {
        // arrange
        var messageBytes = GetBytes("0D-01-00-1D-B8-00-03-14-01-DB-01-69-02-14");
        var accessory = new Accessory { Number = 0 };
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns((BiDiBNode) null);
        
        // act
        Target.ProcessMessage(new AccessoryStateMessage(messageBytes));
        
        // assert
        accessory.Number.Should().Be(0);
        accessory.ActiveAspect.Should().Be(0);
        accessory.ExecutionState.Should().Be(AccessoryExecutionState.Idle);
    }
    
    [TestMethod]
    public void HandleAccessoryState_ShouldNotUpdate_WhenNoMessage()
    {
        // arrange
        var messageBytes = GetBytes("0D-01-00-1D-B8-00-03-14-01-DB-01-69-02-14");
        var accessory = new Accessory { Number = 0 };
        var node = new BiDiBNode
        {
            Accessories = new[]
            {
                accessory
            }
        };
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>()))
            .Returns(node);
        
        // act
        Target.ProcessMessage(new BiDiBInputMessage(messageBytes));
        
        // assert
        accessory.Number.Should().Be(0);
        accessory.ActiveAspect.Should().Be(0);
        accessory.ExecutionState.Should().Be(AccessoryExecutionState.Idle);
    }
}