using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class FeatureMessageReceiverTests : TestClass<FeatureMessageReceiver>
{
    private Mock<IBiDiBNodesFactory> nodesFactory;
    private Feature feature;
    private BiDiBNode node;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        feature = new Feature { FeatureId = 102, Value = 4 };
        node = new BiDiBNode { Features = new[] { feature } };

        nodesFactory = new Mock<IBiDiBNodesFactory>();

        Target = new FeatureMessageReceiver(nodesFactory.Object);
    }

    [TestMethod]
    public void HandleFeatureMessage_ShouldNotHandle_WhenMessageNull()
    {
        // Arrange

        // Act
        Target.ProcessMessage(null);

        // Assert
        feature.Value.Should().Be(4);
        nodesFactory.Verify(x => x.GetNode(It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public void HandleFeatureMessage_ShouldNotHandle_WhenNoFeatureMessage()
    {
        // Arrange
        var bytes = GetBytes("05-00-3F-90-66-02");

        // Act
        Target.ProcessMessage(new BiDiBInputMessage(bytes));

        // Assert
        feature.Value.Should().Be(4);
        nodesFactory.Verify(x => x.GetNode(It.IsAny<byte[]>()), Times.Never);
    }

    [TestMethod]
    public void HandleFeatureMessage_ShouldNotUpdate_WhenNoFeature()
    {
        // Arrange
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);
        var bytes = GetBytes("05-00-3F-90-67-02");

        // Act
        Target.ProcessMessage(new FeatureMessage(bytes));

        // Assert
        feature.Value.Should().Be(4);
    }

    [TestMethod]
    public void HandleFeatureMessage_ShouldUpdateFeatureValue()
    {
        // Arrange
        nodesFactory.Setup(x => x.GetNode(It.IsAny<byte[]>())).Returns(node);
        var bytes = GetBytes("05-00-3F-90-66-02");

        // Act
        Target.ProcessMessage(new FeatureMessage(bytes));

        // Assert
        feature.Value.Should().Be(2);
    }
}