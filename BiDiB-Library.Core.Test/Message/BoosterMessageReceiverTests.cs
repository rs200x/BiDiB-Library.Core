using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message;

[TestClass, TestCategory(TestCategory.UnitTest)]
public class BoosterMessageReceiverTests : TestClass<BoosterMessageReceiver>
{
    private Mock<IBiDiBBoosterNodesManager> boosterNodesManager;

    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();

        boosterNodesManager = new Mock<IBiDiBBoosterNodesManager>();

        Target = new BoosterMessageReceiver(boosterNodesManager.Object);
    }

    [TestMethod]
    public void HandleBoostStat_ShouldApplyStates()
    {
        // Arrange
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode()) {BoosterState = BoosterState.BIDIB_BST_STATE_OFF};
        boosterNodesManager.Setup(x => x.Boosters).Returns(new [] { boosterNode });

        var bytes = GetBytes("04-00-67-B0-C0");

        // Act
        Target.ProcessMessage(new BoostStatMessage(bytes));

        // Assert
        boosterNode.BoosterControl.Should().Be(BoosterControl.Local);
        boosterNode.BoosterState.Should().Be(BoosterState.BIDIB_BST_STATE_ON);
    }
    
    [TestMethod]
    public void HandleBoostStat_ShouldNotUpdate_WhenNoNode()
    {
        // Arrange
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode())
        {
            BoosterState = BoosterState.BIDIB_BST_STATE_OFF
        };
        boosterNodesManager.Setup(x => x.Boosters)
            .Returns(Array.Empty<BiDiBBoosterNode>());

        var bytes = GetBytes("04-00-67-B0-C0");

        // Act
        Target.ProcessMessage(new BoostStatMessage(bytes));

        // Assert
        boosterNode.BoosterControl.Should().Be(BoosterControl.Bus);
        boosterNode.BoosterState.Should().Be(BoosterState.BIDIB_BST_STATE_OFF);
    }    
    
    [TestMethod]
    public void HandleBoostStat_ShouldNotUpdate_WhenNoMessage()
    {
        // Arrange
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode())
        {
            BoosterState = BoosterState.BIDIB_BST_STATE_OFF
        };
        boosterNodesManager.Setup(x => x.Boosters)
            .Returns(new [] { boosterNode });

        var bytes = GetBytes("04-00-67-B0-C0");

        // Act
        Target.ProcessMessage(new BiDiBInputMessage(bytes));

        // Assert
        boosterNode.BoosterControl.Should().Be(BoosterControl.Bus);
        boosterNode.BoosterState.Should().Be(BoosterState.BIDIB_BST_STATE_OFF);
    }
    
    [TestMethod]
    public void HandleCommandStationState_ShouldSetDccState()
    {
        // arrange
        var messageBytes = GetBytes("04-00-3C-E1-08-09-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode());
        boosterNodesManager.Setup(x => x.Boosters).Returns(new[] { boosterNode });
        
        // act
        Target.ProcessMessage(new CommandStationStateMessage(messageBytes));
        
        // assert
        boosterNode.DccState.Should().Be(CommandStationState.BIDIB_CS_STATE_PROG);
    }
    
    [TestMethod]
    public void HandleCommandStationState_ShouldNotUpdate_WhenNoNode()
    {
        // arrange
        var messageBytes = GetBytes("04-00-3C-E1-08-09-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode()) {DccState = CommandStationState.BIDIB_CS_STATE_GO };
        boosterNodesManager.Setup(x => x.Boosters).Returns(Array.Empty<BiDiBBoosterNode>());
        
        // act
        Target.ProcessMessage(new CommandStationStateMessage(messageBytes));
        
        // assert
        boosterNode.DccState.Should().Be(CommandStationState.BIDIB_CS_STATE_GO);
    }
    
    [TestMethod]
    public void HandleCommandStationState_ShouldNotUpdate_WhenNoMessage()
    {
        // arrange
        var messageBytes = GetBytes("04-00-3C-E1-08-09-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode()) {DccState = CommandStationState.BIDIB_CS_STATE_GO };
        boosterNodesManager.Setup(x => x.Boosters).Returns(Array.Empty<BiDiBBoosterNode>());
        
        // act
        Target.ProcessMessage(new BiDiBInputMessage(messageBytes));
        
        // assert
        boosterNode.DccState.Should().Be(CommandStationState.BIDIB_CS_STATE_GO);
    }
    
    [TestMethod]
    public void HandleBoostDiagnostic_ShouldSetDiagnosticValues()
    {
        // arrange
        var messageBytes = GetBytes("09-00-B9-B2-00-1D-01-A4-02-16-B0-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode());
        boosterNodesManager.Setup(x => x.Boosters).Returns(new[] { boosterNode });
        
        // act
        Target.ProcessMessage(new BoostDiagnosticMessage(messageBytes));
        
        // assert
        boosterNode.Current.Should().Be(68);
        boosterNode.Voltage.Should().Be(16.4);
        boosterNode.Temp.Should().Be(22);
    }
    
    [TestMethod]
    public void HandleBoostDiagnostic_ShouldNotUpdate_WhenNoNode()
    {
        // arrange
        var messageBytes = GetBytes("09-00-B9-B2-00-1D-01-A4-02-16-B0-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode()) {DccState = CommandStationState.BIDIB_CS_STATE_GO };
        boosterNodesManager.Setup(x => x.Boosters).Returns(Array.Empty<BiDiBBoosterNode>());
        
        // act
        Target.ProcessMessage(new BoostDiagnosticMessage(messageBytes));
        
        // assert
        boosterNode.Current.Should().Be(0);
        boosterNode.Voltage.Should().Be(0);
        boosterNode.Temp.Should().Be(0);
    }
    
    [TestMethod]
    public void HandleBoostDiagnostic_ShouldNotUpdate_WhenNoMessage()
    {
        // arrange
        var messageBytes = GetBytes("09-00-B9-B2-00-1D-01-A4-02-16-B0-FE");
        var boosterNode = new BiDiBBoosterNode(new BiDiBNode()) {DccState = CommandStationState.BIDIB_CS_STATE_GO };
        boosterNodesManager.Setup(x => x.Boosters).Returns(Array.Empty<BiDiBBoosterNode>());
        
        // act
        Target.ProcessMessage(new BiDiBInputMessage(messageBytes));
        
        // assert
        boosterNode.Current.Should().Be(0);
        boosterNode.Voltage.Should().Be(0);
        boosterNode.Temp.Should().Be(0);
    }
}