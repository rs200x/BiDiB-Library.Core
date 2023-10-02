using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBMessageExtractorTests : TestClass<BiDiBMessageExtractor>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            
            Target = new BiDiBMessageExtractor(new MessageFactory(NullLogger<MessageFactory>.Instance));
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenCheckCrcTrue()
        {
            // Arrange

            /* Test data from log 
            [20151101101101289] Input  : |04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09-83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE|
            [20151101101101293] IN --- : MSG_SYS_PONG              04-00-07-82-01
            [20151101101101297] IN --- : MSG_SYS_SW_VERSION        06-00-08-85-05-02-02
            [20151101101101301] IN --- : MSG_SYS_P_VERSION         05-00-09-83-06-00
            [20151101101101305] IN --- : MSG_PKT_CAPACITY          04-00-0A-8A-40
            [20151101101101309] IN --- : MSG_FEATURE_COUNT         04-00-0B-92-1E
            */
            byte[] bytes = GetBytes("04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09-83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenCheckCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-07-82-01-06-00-08-85-05-02-02-05-00-09-83-06-00-04-00-0A-8A-40-04-00-0B-92-1E");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages(bytes, false);
        }

        private void ExtractMessage_ShouldExtractMultipleMessages(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(5);
            BitConverter.ToString(inputMessages[0].Message, 0, 5).Should().Be("04-00-07-82-01");
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_SYS_PONG);
            BitConverter.ToString(inputMessages[1].Message, 0, 7).Should().Be("06-00-08-85-05-02-02");
            inputMessages[1].MessageType.Should().Be(BiDiBMessage.MSG_SYS_SW_VERSION);
            BitConverter.ToString(inputMessages[2].Message, 0, 6).Should().Be("05-00-09-83-06-00");
            inputMessages[2].MessageType.Should().Be(BiDiBMessage.MSG_SYS_P_VERSION);
            BitConverter.ToString(inputMessages[3].Message, 0, 5).Should().Be("04-00-0A-8A-40");
            inputMessages[3].MessageType.Should().Be(BiDiBMessage.MSG_PKT_CAPACITY);
            BitConverter.ToString(inputMessages[4].Message, 0, 5).Should().Be("04-00-0B-92-1E");
            inputMessages[4].MessageType.Should().Be(BiDiBMessage.MSG_FEATURE_COUNT);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitAndCrcTrue()
        {
            // Arrange

            /* Test data from log 
            [20151101101101289] Input  : |04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09-83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE|
            [20151101101101293] IN --- : MSG_SYS_PONG              04-00-07-82-01
            [20151101101101297] IN --- : MSG_SYS_SW_VERSION        06-00-08-85-05-02-02
            [20151101101101301] IN --- : MSG_SYS_P_VERSION         05-00-09-83-06-00
            [20151101101101305] IN --- : MSG_PKT_CAPACITY          04-00-0A-8A-40
            [20151101101101309] IN --- : MSG_FEATURE_COUNT         04-00-0B-92-1E
            */

            byte[] messageBytes1 = GetBytes("04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09");
            byte[] messageBytes2 = GetBytes("83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages(messageBytes1, messageBytes2, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitAndCrcTrue2()
        {
            // Arrange

            /* Test data from log 
            [20151101101101289] Input  : |04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09-83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE|
            [20151101101101293] IN --- : MSG_SYS_PONG              04-00-07-82-01
            [20151101101101297] IN --- : MSG_SYS_SW_VERSION        06-00-08-85-05-02-02
            [20151101101101301] IN --- : MSG_SYS_P_VERSION         05-00-09-83-06-00
            [20151101101101305] IN --- : MSG_PKT_CAPACITY          04-00-0A-8A-40
            [20151101101101309] IN --- : MSG_FEATURE_COUNT         04-00-0B-92-1E
            */

            byte[] messageBytes1 = GetBytes("06-01-00-01-83-06-00-1A");
            byte[] messageBytes2 = Array.Empty<byte>();
            byte[] messageBytes3 = GetBytes("FE-07-01-00-02-85-02-01-02-01-FE");
            byte[] messageBytes4 = GetBytes("05-01-00-03-92-12-B1-FE");
            byte[] messageBytes5 = GetBytes("0C-00-14-89-05-02-05-00-0D-7A-00-77-ED-11-FE");

            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, true).ToList();
            inputMessages.AddRange(Target.ExtractMessage(messageBytes2, true).ToList());
            inputMessages.AddRange(Target.ExtractMessage(messageBytes3, true).ToList());
            inputMessages.AddRange(Target.ExtractMessage(messageBytes4, true).ToList());
            inputMessages.AddRange(Target.ExtractMessage(messageBytes5, true).ToList());

            // Assert
            inputMessages.Should().HaveCount(4);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitAndCrcTrue3()
        {
            // Arrange

            /* Test data from log 
            [20151101101101289] Input  : |04-00-07-82-01-85-FE-06-00-08-85-05-02-02-F0-FE-05-00-09-83-06-00-54-FE-04-00-0A-8A-40-FB-FE-04-00-0B-92-1E-0E-FE|
            [20151101101101293] IN --- : MSG_SYS_PONG              04-00-07-82-01
            [20151101101101297] IN --- : MSG_SYS_SW_VERSION        06-00-08-85-05-02-02
            [20151101101101301] IN --- : MSG_SYS_P_VERSION         05-00-09-83-06-00
            [20151101101101305] IN --- : MSG_PKT_CAPACITY          04-00-0A-8A-40
            [20151101101101309] IN --- : MSG_FEATURE_COUNT         04-00-0B-92-1E
            */

            byte[] messageBytes1 = GetBytes("07-04-04-00-1A-90-3E-20");

            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, true).ToList();

            // Assert
            inputMessages.Should().HaveCount(0);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitAndCrcFalse()
        {
            // Arrange
            byte[] messageBytes1 = GetBytes("04-00-07-82-01-06-00-08-85-05-02-02-05-00-09");
            byte[] messageBytes2 = GetBytes("83-06-00-04-00-0A-8A-40-04-00-0B-92-1E");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages(messageBytes1, messageBytes2, false);
        }

        private void ExtractMessage_ShouldExtractMultipleMessages(byte[] messageBytes1, byte[] messageBytes2, bool checkCrc)
        {
            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, checkCrc).ToList();
            inputMessages.AddRange(Target.ExtractMessage(messageBytes2, checkCrc).ToList());

            // Assert
            inputMessages.Should().HaveCount(5);
            BitConverter.ToString(inputMessages[0].Message, 0, 5).Should().Be("04-00-07-82-01");
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_SYS_PONG);
            BitConverter.ToString(inputMessages[1].Message, 0, 7).Should().Be("06-00-08-85-05-02-02");
            inputMessages[1].MessageType.Should().Be(BiDiBMessage.MSG_SYS_SW_VERSION);
            BitConverter.ToString(inputMessages[2].Message, 0, 6).Should().Be("05-00-09-83-06-00");
            inputMessages[2].MessageType.Should().Be(BiDiBMessage.MSG_SYS_P_VERSION);
            BitConverter.ToString(inputMessages[3].Message, 0, 5).Should().Be("04-00-0A-8A-40");
            inputMessages[3].MessageType.Should().Be(BiDiBMessage.MSG_PKT_CAPACITY);
            BitConverter.ToString(inputMessages[4].Message, 0, 5).Should().Be("04-00-0B-92-1E");
            inputMessages[4].MessageType.Should().Be(BiDiBMessage.MSG_FEATURE_COUNT);

        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitBlocksAndCrcTrue()
        {
            // Arrange
            byte[] messageBytes1 = GetBytes("08-03-00-17-C6-00-00-00-00-08-03-00-18-C6-00-01-00-00-08-03-00-19-C6-00-02-00-00-08-03-00-1A-C6-00-03-00");
            byte[] messageBytes2 = GetBytes("00-08-03-00-1B-C6-00-04-00-00-08-03-00-1C-C6-00-05-00-00-08-03-00-1D-C6-00-06-00-00-F1-FE-08-03-00-1E-C6-00-07-00-00-08-03-00-1F-C6-00-08-00-00-08-03-00-20-C6-00-09-00-00-08-03-00-21-C6-00-0A-00-00-08-03-00-22-C6-00-0B-00-00-08-03-00-23-C6-00-0C-00-00-08-03-00-24-C6-00-0D-00-00-F5-FE-08-03-00-25-C6-00-0E-00-00-08-03-00-26-C6-00-0F-00-00-DA-FE");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages_WhenSplitBlocks(messageBytes1, messageBytes2, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitBlocksAndCrcFalse()
        {
            // Arrange
            byte[] messageBytes1 = GetBytes("08-03-00-17-C6-00-00-00-00-08-03-00-18-C6-00-01-00-00-08-03-00-19-C6-00-02-00-00-08-03-00-1A-C6-00-03-00");
            byte[] messageBytes2 = GetBytes("00-08-03-00-1B-C6-00-04-00-00-08-03-00-1C-C6-00-05-00-00-08-03-00-1D-C6-00-06-00-00-08-03-00-1E-C6-00-07-00-00-08-03-00-1F-C6-00-08-00-00-08-03-00-20-C6-00-09-00-00-08-03-00-21-C6-00-0A-00-00-08-03-00-22-C6-00-0B-00-00-08-03-00-23-C6-00-0C-00-00-08-03-00-24-C6-00-0D-00-00-08-03-00-25-C6-00-0E-00-00-08-03-00-26-C6-00-0F-00-00");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages_WhenSplitBlocks(messageBytes1, messageBytes2, false);
        }

        private void ExtractMessage_ShouldExtractMultipleMessages_WhenSplitBlocks(byte[] messageBytes1, byte[] messageBytes2, bool checkCrc)
        {
            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, checkCrc).ToList();
            inputMessages.AddRange(Target.ExtractMessage(messageBytes2, checkCrc).ToList());

            // Assert
            inputMessages.Should().HaveCount(16);
            inputMessages.Select(x=>x.MessageType).Should().AllBeEquivalentTo(BiDiBMessage.MSG_LC_CONFIGX);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages2_WhenSplitAndCrcTrue()
        {
            // Arrange

            /* Test data from log 
            INPUT: 05-00-00-81-FD-DE-AF-89-FE-04-00-01-82-EE-FC-FE-0A-00
            IN ---: MSG_SYS_MAGIC             00-00-81-FE-AF p:FE-AF-89-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
            IN ---: MSG_SYS_PONG              00-01-82-EE p:EE-FC-00-00-00-00-00-00-00-00-00-00-00-00
            INPUT: 02-84-DA-00-0D-68-00-F1-EA-11-FE-06-00-03-E2-00-00-01-D0-FE
            IN ---: MSG_SYS_UNIQUE_ID         84-DA-00-0D-68-00-F1-EA p:68-00-F1-EA-11-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
            IN ---: MSG_CS_DRIVE_ACK          00-03-E2-00-00-01 p:00-00-01-D0-00-00-00-00-00-00-00-00-00-00-00-00
            */

            byte[] messageBytes1 = GetBytes("05-00-00-81-FD-DE-AF-89-FE-04-00-01-82-EE-FC-FE-0A-00");
            byte[] messageBytes2 = GetBytes("02-84-DA-00-0D-68-00-F1-EA-11-FE-06-00-03-E2-00-00-01-D0-FE");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages2(messageBytes1, messageBytes2, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages2_WhenSplitAndCrcFalse()
        {
            // Arrange
            byte[] messageBytes1 = GetBytes("05-00-00-81-FE-AF-04-00-01-82-EE-0A-00");
            byte[] messageBytes2 = GetBytes("02-84-DA-00-0D-68-00-F1-EA-06-00-03-E2-00-00-01");

            // Act
            ExtractMessage_ShouldExtractMultipleMessages2(messageBytes1, messageBytes2, false);
        }

        private void ExtractMessage_ShouldExtractMultipleMessages2(byte[] messageBytes1, byte[] messageBytes2, bool checkCrc)
        {
            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, checkCrc).ToList();
            inputMessages.AddRange(Target.ExtractMessage(messageBytes2, checkCrc).ToList());

            // Assert
            //inputMessages.Should().HaveCount(4);
            BitConverter.ToString(inputMessages[0].Message, 0, 6).Should().Be("05-00-00-81-FE-AF");
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_SYS_MAGIC);
            BitConverter.ToString(inputMessages[1].Message, 0, 5).Should().Be("04-00-01-82-EE");
            inputMessages[1].MessageType.Should().Be(BiDiBMessage.MSG_SYS_PONG);
            BitConverter.ToString(inputMessages[2].Message, 0, 11).Should().Be("0A-00-02-84-DA-00-0D-68-00-F1-EA");
            inputMessages[2].MessageType.Should().Be(BiDiBMessage.MSG_SYS_UNIQUE_ID);
            BitConverter.ToString(inputMessages[3].Message, 0, 7).Should().Be("06-00-03-E2-00-00-01");
            inputMessages[3].MessageType.Should().Be(BiDiBMessage.MSG_CS_DRIVE_ACK);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_SYS_MAGIC_WhenCrcTrue()
        {
            // Arrange
            //string messageString = "07-06-00-27-A3-00-00-00-07-06-00-28-A3-01-00-00-07-06-00-29-A3-02-15-00-07-06-00-2A-A3-03-15-00-07-06-00-2B-A3-04-00-00-07-06-00-2C-A3-05-00-00-07-06-00-2D-A3-06-00-00-07-06-00-2E-A3-07-00-00-0E-FE";
            byte[] bytes = GetBytes("05-00-00-81-FD-DE-AF-89-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_SYS_MAGIC(bytes, true);

        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_SYS_MAGIC_WhenCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-00-81-FE-AF");

            // Act
            ExtractMessage_ShouldConvert_MSG_SYS_MAGIC(bytes, false);
        }


        [TestMethod]
        public void ExtractMessage_ShouldConvert_WhenLeadingMagic()
        {
            // Arrange
            byte[] bytes = GetBytes("FE-05-00-00-81-FD-DE-AF-89-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_SYS_MAGIC(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_WhenLeadingZeros()
        {
            // Arrange
            byte[] bytes = GetBytes("00-00-FE-05-00-00-81-FD-DE-AF-89-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_SYS_MAGIC(bytes, true);
        }

        private void ExtractMessage_ShouldConvert_MSG_SYS_MAGIC(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(1);
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_SYS_MAGIC);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenCrcTrue()
        {
            // Arrange

            //string messageString = "07-06-00-27-A3-00-00-00-07-06-00-28-A3-01-00-00-07-06-00-29-A3-02-15-00-07-06-00-2A-A3-03-15-00-07-06-00-2B-A3-04-00-00-07-06-00-2C-A3-05-00-00-07-06-00-2D-A3-06-00-00-07-06-00-2E-A3-07-00-00-0E-FE";
            byte[] bytes = GetBytes("09-04-00-2F-A3-00-2F-8A-31-0A-97-FE");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("09-04-00-2F-A3-00-2F-8A-31-0A");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS(bytes, false);
        }

        private void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(1);
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_BM_ADDRESS);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenSplitAndCrcTrue()
        {
            // Arrange
            //string messageString = "07-06-00-27-A3-00-00-00-07-06-00-28-A3-01-00-00-07-06-00-29-A3-02-15-00-07-06-00-2A-A3-03-15-00-07-06-00-2B-A3-04-00-00-07-06-00-2C-A3-05-00-00-07-06-00-2D-A3-06-00-00-07-06-00-2E-A3-07-00-00-0E-FE";
            byte[] bytes1 = GetBytes("09-04-00-2F-A3-00");
            byte[] bytes2 = GetBytes("2F-8A-31-0A-97-FE");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenSplit(bytes1, bytes2, true);

        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenSplitAndCrcFalse()
        {
            // Arrange
            byte[] bytes1 = GetBytes("09-04-00-2F-A3-00");
            byte[] bytes2 = GetBytes("2F-8A-31-0A");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenSplit(bytes1, bytes2, false);
        }

        private void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenSplit(byte[] messageBytes1, byte[] messageBytes2, bool checkCrc)
        {
            // Act
            List<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes1, checkCrc).ToList();
            inputMessages.AddRange(Target.ExtractMessage(messageBytes2, checkCrc).ToList());

            // Assert
            inputMessages.Should().HaveCount(1);
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_BM_ADDRESS);

        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenMultipleAndCrcTrue()
        {
            // Arrange
            byte[] bytes = GetBytes("07-06-00-27-A3-00-00-00-07-06-00-28-A3-01-00-00-07-06-00-29-A3-02-15-00-07-06-00-2A-A3-03-15-00-07-06-00-2B-A3-04-00-00-07-06-00-2C-A3-05-00-00-07-06-00-2D-A3-06-00-00-07-06-00-2E-A3-07-00-00-0E-FE");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenMultiple(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenMultipleAndCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("07-06-00-27-A3-00-00-00-07-06-00-28-A3-01-00-00-07-06-00-29-A3-02-15-00-07-06-00-2A-A3-03-15-00-07-06-00-2B-A3-04-00-00-07-06-00-2C-A3-05-00-00-07-06-00-2D-A3-06-00-00-07-06-00-2E-A3-07-00-00");

            // Act
            ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenMultiple(bytes, false);
        }

        private void ExtractMessage_ShouldExtract_MSG_BM_ADDRESS_WhenMultiple(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(8);
            inputMessages.Count(x => x.MessageType == BiDiBMessage.MSG_BM_ADDRESS).Should().Be(8);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_BM_FREE_WhenCrcTrue()
        {
            // Arrange
            byte[] bytes = GetBytes("05-01-00-22-A1-02-6B-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_BM_FREE(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_BM_FREE_WhenCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("05-01-00-22-A1-02");

            // Act
            ExtractMessage_ShouldConvert_MSG_BM_FREE(bytes, false);
        }

        private void ExtractMessage_ShouldConvert_MSG_BM_FREE(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(1);
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_BM_FREE);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractWithStartingMagic_WhenCrcTrue()
        {
            // Arrange
            byte[] bytes = GetBytes("FE-05-01-00-22-A1-02-6B-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_BM_FREE(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractWithStartingMagic_WhenCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("FE-05-01-00-22-A1-02");

            // Act
            ExtractMessage_ShouldConvert_MSG_BM_FREE(bytes, false);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_FEATURE_WhenCrcTrue()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-0C-90-00-10-4C-FE");

            // Act
            ExtractMessage_ShouldConvert_MSG_FEATURE(bytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldConvert_MSG_FEATURE_WhenCrcFalse()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-0C-90-00-10");

            // Act
            ExtractMessage_ShouldConvert_MSG_FEATURE(bytes, false);
        }

        private void ExtractMessage_ShouldConvert_MSG_FEATURE(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(1);
            BitConverter.ToString(inputMessages[0].Message, 0, 6).Should().Be("05-00-0C-90-00-10");
            inputMessages[0].MessageType.Should().Be(BiDiBMessage.MSG_FEATURE);
            byte[] parameters = inputMessages[0].MessageParameters;
            parameters.Should().HaveCount(2);
            parameters[0].Should().Be(0);
            parameters[1].Should().Be(16); // number value
            parameters[1].Should().Be(0x10); // byte value
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractStringAndFeature_WhenCrcTrue()
        {
            // Arrange
            byte[] messageBytes = GetBytes("1F-01-00-08-95-00-00-18-4F-6E-65-53-65-72-76-6F-54-75-72-6E-20-2D-20-34-53-65-2D-38-46-2D-34-49-08-01-00-09-95-00-01-01-20-94-FE-06-01-00-0A-90-00-00-8A-FE");

            // Act
            ExtractMessage_ShouldExtractStringAndFeature(messageBytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractStringAndFeature_WhenCrcFalse()
        {
            // Arrange
            byte[] messageBytes = GetBytes("1F-01-00-08-95-00-00-18-4F-6E-65-53-65-72-76-6F-54-75-72-6E-20-2D-20-34-53-65-2D-38-46-2D-34-49-08-01-00-09-95-00-01-01-20-06-01-00-0A-90-00-00");

            // Act
            ExtractMessage_ShouldExtractStringAndFeature(messageBytes, false);
        }

        private void ExtractMessage_ShouldExtractStringAndFeature(byte[] messageBytes, bool checkCrc)
        {
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(3);
            StringMessage message1 = inputMessages[0] as StringMessage;
            message1.Should().NotBeNull();
            message1.StringId.Should().Be(0);
            message1.StringValue.Should().Be("OneServoTurn - 4Se-8F-4I");

            StringMessage message2 = inputMessages[1] as StringMessage;
            message2.Should().NotBeNull();
            message2.StringId.Should().Be(1);
            message2.StringValue.Should().Be(" ");

            FeatureMessage message3 = inputMessages[2] as FeatureMessage;
            message3.Should().NotBeNull();
            message3.FeatureId.Should().Be(0);
            message3.Value.Should().Be(0);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_AccessoryState_WhenCrcTrue()
        {
            // Arrange
            //byte[] messageBytes = GetBytes("09-01-00-81-B8-04-FF-00-02-00-09-01-00-82-B8-05-FF-00-02-00-09-01-00-83-B8-06-FF-00-02-00-09-01-00-84-B8-07-FF-00-02-00-89-FE");
            byte[] messageBytes = GetBytes("0D-01-00-7D-B8-00-FF-30-02-00-00-00-01-00-09-01-00-7E-B8-01-01-04-02-00-09-01-00-7F-B8-02-00-02-02-00-09-01-00-80-B8-03-FF-00-02-00-61-FE");

            ExtractMessage_ShouldExtractMultipleMessages_AccessoryState(messageBytes, true);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_AccessoryState_WhenCrcFalse()
        {
            // Arrange
            //byte[] messageBytes = GetBytes("09-01-00-81-B8-04-FF-00-02-00-09-01-00-82-B8-05-FF-00-02-00-09-01-00-83-B8-06-FF-00-02-00-09-01-00-84-B8-07-FF-00-02-00-89-FE");
            byte[] messageBytes = GetBytes("0D-01-00-7D-B8-00-FF-30-02-00-00-00-01-00-09-01-00-7E-B8-01-01-04-02-00-09-01-00-7F-B8-02-00-02-02-00-09-01-00-80-B8-03-FF-00-02-00");

            ExtractMessage_ShouldExtractMultipleMessages_AccessoryState(messageBytes, false);
        }

        private void ExtractMessage_ShouldExtractMultipleMessages_AccessoryState(byte[] messageBytes, bool checkCrc)
        {
            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, checkCrc).ToList();

            // Assert
            inputMessages.Should().HaveCount(4);
        }

        [TestMethod]
        public void ExtractMessage_ShouldExtractMultipleMessages_SysError()
        {
            // Arrange
            byte[] messageBytes = GetBytes("05-00-3E-86-11-01-02-01-01-02-01-00-05-00-3F-86-11-01-8D-FE-05-00-40-86-11-01-CE-FE");

            // Act
            IList<BiDiBInputMessage> inputMessages = Target.ExtractMessage(messageBytes, true).ToList();

            // Assert
            inputMessages.Should().HaveCount(3);
        }
    }
}