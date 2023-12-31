﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Controllers.Interfaces;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Services;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class ConnectionServiceTests : TestClass<IBiDiBMessageService>
    {
        private ConnectionService target;
        private Mock<IConnectionController> connectionControllerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            connectionControllerMock = new Mock<IConnectionController>();
            target = new ConnectionService(Array.Empty<IConnectionControllerFactory>(), NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void OpenConnection_ShouldThrow_WhenNoActiveConnectionController()
        {
            // Arrange

            // Act
            Func<Task> action = async () => await target.OpenConnectionAsync();

            // Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("connection service is not configured for any connection");
        }

        [TestMethod]
        public async Task OpenConnection_ShouldOpenConnectionOnActiveController()
        {
            // Arrange
            SetConnectionController();
            connectionControllerMock.Setup(x => x.OpenConnectionAsync()).Returns(Task.Factory.StartNew(() => new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialSimulation)));

            // Act
            await target.OpenConnectionAsync();

            // Assert
            connectionControllerMock.Verify(x => x.OpenConnectionAsync(), Times.Once);
        }

        [TestMethod]
        public async Task OpenConnection_ShouldReturnStatusFromActiveController()
        {
            // Arrange
            SetConnectionController();
            connectionControllerMock.Setup(x => x.ConnectionState)
                .Returns(new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialSimulation));
            connectionControllerMock.Setup(x => x.OpenConnectionAsync())
                .Returns(Task.Factory.StartNew(() => new ConnectionStateInfo(InterfaceConnectionState.FullyConnected, InterfaceConnectionType.SerialSimulation)));

            // Act
            var state = await target.OpenConnectionAsync();

            // Assert
            state.IsConnected.Should().BeTrue();
            target.ConnectionState.InterfaceState.Should().Be(InterfaceConnectionState.FullyConnected);
        }

        [TestMethod]
        public void CloseConnection_ShouldCloseOnActiveController()
        {
            // Arrange
            SetConnectionController();

            // Act
            target.CloseConnection();

            // Assert
            connectionControllerMock.Verify(x => x.Close(), Times.Once);
        }

        private void SetConnectionController()
        {
            var prop = target.GetType()
                .GetField("activeConnectionController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(target, connectionControllerMock.Object);


        }
    }
}