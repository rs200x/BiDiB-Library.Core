using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class MessageServiceTests : TestClass
    {
        private BiDiBMessageService target;
        private Mock<IConnectionService> connectionServiceMock;
        private IBiDiBMessageExtractor messageExtractor;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            connectionServiceMock = new Mock<IConnectionService>();
            messageExtractor = new BiDiBMessageExtractor();
            var loggerFactory = new Mock<ILoggerFactory>();
            target = new BiDiBMessageService(connectionServiceMock.Object, messageExtractor, loggerFactory.Object);
        }


        [TestMethod]
        public void GetAddressHash()
        {
            // Arrange
            byte[] adr1 = new byte[] { 0, 0, 0, 0 };
            byte[] adr1X = new byte[] { 0x00, 0, 0, 0 };
            byte[] adr2 = new byte[] { 0, 0, 1, 1 };
            byte[] adr3 = new byte[] { 0, 0, 0, 2 };
            byte[] adr4 = new byte[] { 1, 1, 0, 0 };
            byte[] adr5 = new byte[] { 2, 0, 0, 0 };
            byte[] adr6 = new byte[] { 1, 1, 0, 0 };

            // Act
            int adr1N = adr1.Aggregate(0, (current, b) => current + b);
            int adr2N = adr2.Aggregate(0, (current, b) => current + b);
            int adr3N = adr3.Aggregate(0, (current, b) => current + b);
            int adr4N = adr4.Aggregate(0, (current, b) => current + b);
            int adr5N = adr5.Aggregate(0, (current, b) => current + b);

            int adr1H = adr1.GetHashCode();
            int adr1xH = adr1X.GetHashCode();
            int adr2H = adr2.GetHashCode();
            int adr3H = adr3.GetHashCode();
            int adr4H = adr4.GetHashCode();
            int adr5H = adr5.GetHashCode();
            int adr6H = adr6.GetHashCode();

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
            MethodInfo dynMethod = target.GetType().GetMethod("GetNextSequenceNumber", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            byte sequence1 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence2 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence3 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence4 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });

            // Assert
            sequence1.Should().Be(0x00);
            sequence2.Should().Be(0x01);
            sequence3.Should().Be(0x02);
            sequence4.Should().Be(0x03);
        }

        [TestMethod]
        public void GetNextSequenceNumber_ShouldResetSequenceWhenOverflow()
        {
            // Arrange
            byte[] defaultAddress = { 0 };
            MethodInfo dynMethod = target.GetType().GetMethod("GetNextSequenceNumber", BindingFlags.NonPublic | BindingFlags.Instance);
            var start = new ConcurrentDictionary<int, byte> ();
            start.AddOrUpdate(0, 254, null);
            target.GetType().GetProperty("addressSequenceNumbers", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, start);

            // Act
            byte sequence1 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence2 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence3 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });
            byte sequence4 = (byte)dynMethod.Invoke(target, new object[] { defaultAddress });

            // Assert
            sequence1.Should().Be(0xff);
            sequence2.Should().Be(0x01);
            sequence3.Should().Be(0x02);
            sequence4.Should().Be(0x03);
        }
    }
}
