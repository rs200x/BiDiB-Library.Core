using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models.Messages.Output;
using org.bidib.Net.Core.Services.Interfaces;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBMessageServiceTests : TestClass<BiDiBMessageService>
    {
        private Mock<IConnectionService> connectionServiceMock;
        private IBiDiBMessageExtractor messageExtractor;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            connectionServiceMock = new Mock<IConnectionService>();
            messageExtractor = new BiDiBMessageExtractor(Mock.Of<IMessageFactory>());
            var loggerFactory = new Mock<ILoggerFactory>();
            Target = new BiDiBMessageService(connectionServiceMock.Object, messageExtractor, loggerFactory.Object);
        }

        [TestMethod]
        public void GetAddressHash()
        {
            // Arrange
            byte[] adr1 = { 0, 0, 0, 0 };
            byte[] adr1X = { 0x00, 0, 0, 0 };
            byte[] adr2 = { 0, 0, 1, 1 };
            byte[] adr3 = { 0, 0, 0, 2 };
            byte[] adr4 = { 1, 1, 0, 0 };
            byte[] adr5 = { 2, 0, 0, 0 };
            byte[] adr6 = { 1, 1, 0, 0 };

            // Act
            var adr1N = adr1.Aggregate(0, (current, b) => current + b);
            var adr2N = adr2.Aggregate(0, (current, b) => current + b);
            var adr3N = adr3.Aggregate(0, (current, b) => current + b);
            var adr4N = adr4.Aggregate(0, (current, b) => current + b);
            var adr5N = adr5.Aggregate(0, (current, b) => current + b);

            var adr1H = adr1.GetHashCode();
            var adr1xH = adr1X.GetHashCode();
            var adr2H = adr2.GetHashCode();
            var adr3H = adr3.GetHashCode();
            var adr4H = adr4.GetHashCode();
            var adr5H = adr5.GetHashCode();
            var adr6H = adr6.GetHashCode();

            // Assert
            adr2N.Should().Be(adr3N);
            adr2H.Should().NotBe(adr3H);
            adr4.SequenceEqual(adr6).Should().BeTrue();
            adr1.SequenceEqual(adr1X).Should().BeTrue();
        }

        [TestMethod]
        public void GetNextSequenceNumber_ShouldRaiseWithEveryCall()
        {
            // Arrange
            byte[] defaultAddress = { 0 };
            var dynMethod = Target.GetType().GetMethod("GetNextSequenceNumber", BindingFlags.NonPublic | BindingFlags.Instance);

            var message = new BiDiBOutputMessage(defaultAddress, BiDiBMessage.MSG_BM_CV);
            
            // Act
            var sequence1 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence2 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence3 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence4 = (byte)dynMethod.Invoke(Target, new object[] { message });

            // Assert
            sequence1.Should().Be(0x00);
            sequence2.Should().Be(0x01);
            sequence3.Should().Be(0x02);
            sequence4.Should().Be(0x03);
        }
        
        [TestMethod]
        public void GetNextSequenceNumber_ShouldReturnNull_WhenLocalMessageType()
        {
            // Arrange
            byte[] defaultAddress = { 0 };
            var dynMethod = Target.GetType().GetMethod("GetNextSequenceNumber", BindingFlags.NonPublic | BindingFlags.Instance);

            var localTypes = Enum.GetValues<BiDiBMessage>().Where(x => x >= BiDiBMessage.MSG_LOCAL_LOGON).ToList();

            foreach (var type in localTypes)
            {
                var message = new BiDiBOutputMessage(defaultAddress, type);
                
                // Act
                var sequence = (byte)dynMethod?.Invoke(Target, new object[] { message });
                
                // Assert
                sequence.Should().Be(0x00);
            }
        }

        [TestMethod]
        public void GetNextSequenceNumber_ShouldResetSequenceWhenOverflow()
        {
            // Arrange
            byte[] defaultAddress = { 0 };
            var dynMethod = Target.GetType().GetMethod("GetNextSequenceNumber", BindingFlags.NonPublic | BindingFlags.Instance);
            var start = (ConcurrentDictionary<int, byte>)Target.GetType().GetField("addressSequenceNumbers", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(Target);
            start?.AddOrUpdate(0, 254, (_, value) => value);
            
            var message = new BiDiBOutputMessage(defaultAddress, BiDiBMessage.MSG_BM_CV);

            // Act
            var sequence1 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence2 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence3 = (byte)dynMethod.Invoke(Target, new object[] { message });
            var sequence4 = (byte)dynMethod.Invoke(Target, new object[] { message });

            // Assert
            sequence1.Should().Be(0xff);
            sequence2.Should().Be(0x01);
            sequence3.Should().Be(0x02);
            sequence4.Should().Be(0x03);
        }
    }
}
