using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models.Messages.Output;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class MessageGeneratorTests : TestClass
    {
        private readonly byte[] defaultAddress = { 0 };
        private BiDiBMessage message;
        private byte[] messageParameters;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();
            BiDiBMessageGenerator.SecureMessages = true;
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_SYS_GET_MAGIC()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_GET_MAGIC;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-01-D6-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_SYS_MAGIC()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_MAGIC;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 0, 0xFE, 0xAF);

            // Assert
            //AssertBasicMessageInfo(message,5);
            BitConverter.ToString(messageBytes, 0, 10).Should().Be("FE-05-00-00-81-FD-DE-AF-89-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_SYS_RESET()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_RESET;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-09-14-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_SYS_PING()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_PING;
            messageParameters = new byte[] { 0xee };

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 1, messageParameters);

            // Assert
            AssertBasicMessageInfo(messageBytes, 4);
            BitConverter.ToString(messageBytes, 0, 8).Should().Be("FE-04-00-01-07-EE-2C-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_SYS_GET_UNIQUE_ID()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_GET_UNIQUE_ID;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 2);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-02-05-26-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_BM_ADDRESS()
        {
            // Arrange
            message = BiDiBMessage.MSG_BM_ADDRESS;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-A3-C5-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_BM_FREE()
        {
            // Arrange
            message = BiDiBMessage.MSG_BM_FREE;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-A1-79-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_BM_SPEED()
        {
            // Arrange
            message = BiDiBMessage.MSG_BM_SPEED;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message);

            // Assert
            AssertBasicMessageInfo(messageBytes, 3);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-A6-FA-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_CS_PROG_WithCrcEscape()
        {
            // Arrange
            byte[] cvBytes = BitConverter.GetBytes(18);
            byte[] messageParams = { 0x02, cvBytes[0], cvBytes[1], 0 };
            message = BiDiBMessage.MSG_CS_PROG;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 217, messageParams);

            // Assert
            //AssertBasicMessageInfo(message, 3);
            BitConverter.ToString(messageBytes, 0, 12).Should().Be("FE-07-00-D9-6F-02-12-00-00-FD-DE-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_CS_PROG_WithSquenceEscape()
        {
            // Arrange
            byte[] cvnum = BitConverter.GetBytes(48);
            byte[] messageParams = { 0x02, cvnum[0], cvnum[1], 0 };
            message = BiDiBMessage.MSG_CS_PROG;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 253, messageParams);

            // Assert
            BitConverter.ToString(messageBytes, 0, 12).Should().Be("FE-07-00-FD-DD-6F-02-30-00-00-4F-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_BM_DYN_STATE()
        {
            // Arrange
            message = BiDiBMessage.MSG_BM_DYN_STATE;
            byte[] setup = new byte[5];
            setup[0] = 0x06;
            setup[1] = 0x03;
            setup[2] = 0x00;
            setup[3] = 0x02; // temp
            setup[4] = 0x1E; // 30°C


            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 0, setup);

            // Assert
            //AssertBasicMessageInfo(message, 8);
            BitConverter.ToString(messageBytes, 0, 12).Should().Be("FE-08-00-00-AA-06-03-00-02-1E-C9-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_BM_MIRROR_MULTIPLE()
        {
            // Arrange
            message = BiDiBMessage.MSG_BM_MIRROR_MULTIPLE;
            byte[] setup = new byte[4];

            setup[1] = 0x10;

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 44, setup);

            // Assert
            //AssertBasicMessageInfo(message, 7);
            BitConverter.ToString(messageBytes, 0, 11).Should().Be("FE-07-00-2C-21-00-10-00-00-64-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_CS_DRIVE()
        {
            // Arrange
            message = BiDiBMessage.MSG_CS_DRIVE;
            byte[] param = { 0, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 3, param);

            // Assert
            //AssertBasicMessageInfo(message, 7);
            BitConverter.ToString(messageBytes, 0, 15).Should().Be("FE-0B-00-03-64-00-00-00-00-00-00-00-00-8D-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_FEATURE_NEXT()
        {
            // Arrange

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(new FeatureNextMessage(defaultAddress));

            // Assert
            //AssertBasicMessageInfo(message, 7);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-11-4B-FE");
        }

        [TestMethod]
        public void GenerateMessages_ShouldReturnMessage_MSG_FEATURE_NEXT()
        {
            // Arrange

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(new List<BiDiBOutputMessage>{ new FeatureNextMessage(defaultAddress)});

            // Assert
            //AssertBasicMessageInfo(message, 7);
            BitConverter.ToString(messageBytes, 0, 7).Should().Be("FE-03-00-00-11-4B-FE");
        }

        [TestMethod]
        public void GenerateMessage_ShouldReturnMessage_MSG_UniqueId()
        {
            // Arrange
            message = BiDiBMessage.MSG_SYS_UNIQUE_ID;
            List<byte> paramBytes = new List<byte>();
            paramBytes.AddRange(GetBytes("DA-00-0D-68-00-F1-EA"));
            paramBytes.AddRange(new byte[] { 128, 27, 206, 111 });

            // Act
            byte[] messageBytes = BiDiBMessageGenerator.GenerateMessage(defaultAddress, message, 3, paramBytes.ToArray());

            // Assert
            //AssertBasicMessageInfo(message, 7);
            BitConverter.ToString(messageBytes, 0, 18).Should().Be("FE-0E-00-03-84-DA-00-0D-68-00-F1-EA-80-1B-CE-6F-3C-FE");
        }

        private void AssertBasicMessageInfo(byte[] messageArray, int membersCount)
        {
            messageArray.Should().NotBeNull();
            messageArray[0].Should().Be(0xfe);
            messageArray[1].Should().Be((byte)membersCount);

            int lastDataIndex = 0;
            for (int i = 0; i < messageArray.Length; i++)
            {
                byte data = messageArray[i];
                if (data != 0)
                {
                    lastDataIndex = i;
                }
            }
            int messageNumberIndex = lastDataIndex - (2 + (messageParameters?.Length ?? 0));
            messageArray[messageNumberIndex].Should().Be((byte)message);
            messageArray[lastDataIndex].Should().Be(0xfe);
        }
    }
}
