using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Models.BiDiB.Base;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Utils;
using org.bidib.Net.Testing;
using SysMagicMessage = org.bidib.Net.Core.Models.Messages.Input.SysMagicMessage;

namespace org.bidib.Net.Core.Test.Message
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class MessageFactoryTests : TestClass<MessageFactory>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new MessageFactory(NullLogger<MessageFactory>.Instance);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysErrorMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-30-86-02-FC-56-FE");

            // Act
            SysErrorMessage message = Target.CreateInputMessage(bytes) as SysErrorMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Error.Should().Be(SystemError.BIDIB_ERR_CRC);
            message.Data.Should().Be("MSG_NUM: FC");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysErrorMessage_ADDRSTACK()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-3E-86-11-01");

            // Act
            SysErrorMessage message = Target.CreateInputMessage(bytes) as SysErrorMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Error.Should().Be(SystemError.BIDIB_ERR_ADDRSTACK);
            message.Data.Should().Be("Node: 01, ADDR_STACK: ");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysErrorMessage_NO_ACK_BY_HOST()
        {
            // Arrange
            byte[] bytes = GetBytes("06-01-00-30-86-30-00-6F-FE");

            // Act
            SysErrorMessage message = Target.CreateInputMessage(bytes) as SysErrorMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Error.Should().Be(SystemError.BIDIB_ERR_NO_ACK_BY_HOST);
            message.Data.Should().Be("ErrorCode: 00");

        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysUniqueIdMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0A-00-02-84-DA-00-0D-68-00-F1-EA-11-FE");

            // Act
            SysUniqueIdMessage message = Target.CreateInputMessage(bytes) as SysUniqueIdMessage;

            // Assert
            Assert.IsNotNull(message);
            message.VendorProducerId.Should().Be("V 0D P 6800F1EA");
            message.HexUId.Should().Be("DA.00.0D.68.00.F1.EA");
            message.Fingerprint.Should().Be(0);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysUniqueIdMessageWithFingerprint()
        {
            // Arrange
            byte[] bytes = GetBytes("0E-00-03-84-DA-00-0D-68-00-F1-EA-80-1B-CE-6F-3C-FE");

            // Act
            SysUniqueIdMessage message = Target.CreateInputMessage(bytes) as SysUniqueIdMessage;

            // Assert
            Assert.IsNotNull(message);
            message.VendorProducerId.Should().Be("V 0D P 6800F1EA");
            message.HexUId.Should().Be("DA.00.0D.68.00.F1.EA");
            message.Fingerprint.Should().Be(1875778432);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnCommandStationProgStateMessage()
        {
            // Arrange
            string messageString = "08-00-3F-EF-80-00-06-00-1F-C0-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            CommandStationProgStateMessage message = Target.CreateInputMessage(bytes) as CommandStationProgStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.ProgState.Should().Be(CommandStationProgState.BIDIB_CS_PROG_OKAY);
            message.CvHigh.Should().Be(0x00);
            message.CvLow.Should().Be(0x06);
            message.CvNumber.Should().Be(7);
            message.Data.Should().Be(0x1f);

        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnCommandStationStateMessage()
        {
            // Arrange
            string messageString = "04-00-3C-E1-08-09-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            CommandStationStateMessage message = Target.CreateInputMessage(bytes) as CommandStationStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.State.Should().Be(CommandStationState.BIDIB_CS_STATE_PROG);

        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnCommandStationPoMAckMessage()
        {
            // Arrange
            string messageString = "09-00-93-E4-03-00-00-00-00-01-A7-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            CommandStationPoMAckMessage message = Target.CreateInputMessage(bytes) as CommandStationPoMAckMessage;

            // Assert
            Assert.IsNotNull(message);
            message.DecoderAddress.Should().Be(3);
            message.Receipt.Should().Be(1);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackCvMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("08-00-77-A5-04-00-07-00-9D-03-FE");

            // Act
            FeedbackCvMessage message = Target.CreateInputMessage(bytes) as FeedbackCvMessage;

            // Assert
            Assert.IsNotNull(message);
            message.DecoderAddress.Should().Be(4);
            message.CvNumber.Should().Be(8);
            message.Data.Should().Be(157);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnBoosterDiagnosticMessage()
        {
            // Arrange
            string messageString = "09-00-B9-B2-00-1D-01-A4-02-16-B0-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            BoostDiagnosticMessage message = Target.CreateInputMessage(bytes) as BoostDiagnosticMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Current.Should().Be(68);
            message.Voltage.Should().Be(16400);
            message.Temperature.Should().Be(22);

        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnNodeNewMessage()
        {
            // Arrange
            string messageString = "0C-00-27-8D-04-01-45-00-0D-79-00-01-EC-3E-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            NodeNewMessage message = Target.CreateInputMessage(bytes) as NodeNewMessage;

            // Assert
            Assert.IsNotNull(message);
            message.TableVersion.Should().Be(4);
            message.LocalNodeAddress.Should().Be(1);
            message.HexUid.Should().Be("45.00.0D.79.00.01.EC");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnNodeLostMessage()
        {
            // Arrange
            string messageString = "0C-00-28-8C-05-01-45-00-0D-79-00-01-EC-B3-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            NodeLostMessage message = Target.CreateInputMessage(bytes) as NodeLostMessage;

            // Assert
            Assert.IsNotNull(message);
            message.TableVersion.Should().Be(5);
            message.LocalNodeAddress.Should().Be(1);
            message.HexUid.Should().Be("45.00.0D.79.00.01.EC");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnNodeTabMessage()
        {
            // Arrange
            string messageString = "0C-00-30-89-05-01-45-00-0D-79-00-BD-EB-B9-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            NodeTabMessage message = Target.CreateInputMessage(bytes) as NodeTabMessage;

            // Assert
            Assert.IsNotNull(message);
            message.TableVersion.Should().Be(5);
            message.LocalNodeAddress.Should().Be(1);
            message.HexUid.Should().Be("45.00.0D.79.00.BD.EB");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnNodeTabCountMessage()
        {
            // Arrange
            string messageString = "04-00-2E-88-05-19-FE";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            NodeTabCountMessage message = Target.CreateInputMessage(bytes) as NodeTabCountMessage;

            // Assert
            Assert.IsNotNull(message);
            message.NodeCount.Should().Be(5);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackAddressMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("08-00-18-A3-06-04-80-03-00-95-FE");

            // Act
            FeedbackAddressMessage message = Target.CreateInputMessage(bytes) as FeedbackAddressMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(6);
            message.Addresses.Should().HaveCount(2);
            message.Addresses.ElementAt(0).Address.Should().Be(4);
            message.Addresses.ElementAt(0).Direction.Should().Be(DecoderDirection.BackwardDirection);
            message.Addresses.ElementAt(1).Address.Should().Be(3);
            message.Addresses.ElementAt(1).Direction.Should().Be(DecoderDirection.ForwardDirection);
        }
        
        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackAddressMessage_WithSameAddress()
        {
            // Arrange
            byte[] bytes = GetBytes("09-04-00-00-A3-01-65-84-65-C4");

            // Act
            FeedbackAddressMessage message = Target.CreateInputMessage(bytes) as FeedbackAddressMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(1);
            message.Addresses.Should().HaveCount(2);
            message.Addresses.ElementAt(0).Address.Should().Be(1125);
            message.Addresses.ElementAt(0).Direction.Should().Be(DecoderDirection.BackwardDirection);
            message.Addresses.ElementAt(1).Address.Should().Be(1125);
            message.Addresses.ElementAt(1).Direction.Should().Be(DecoderDirection.ExtendedAccessoryDirection);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackDynStateMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("08-00-00-AA-06-03-80-02-1E-C9-FE");

            // Act
            FeedbackDynStateMessage message = Target.CreateInputMessage(bytes) as FeedbackDynStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(6);
            message.DecoderAddress.Should().Be(3);
            message.Direction.Should().Be(DecoderDirection.BackwardDirection);
            message.DynState.Should().Be(DynState.Temperature);
            message.Value.Should().Be(30);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackDynStateMessage_WithTimestamp()
        {
            // Arrange
            byte[] bytes = GetBytes("FE-0B-00-00-AA-06-03-80-06-01-05-02-0B-60-FE");

            // Act
            FeedbackDynStateMessage message = Target.CreateInputMessage(bytes) as FeedbackDynStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(6);
            message.DecoderAddress.Should().Be(3);
            message.Direction.Should().Be(DecoderDirection.BackwardDirection);
            message.DynState.Should().Be(DynState.Distance);
            message.Distance.Should().Be(1281);
            message.Timestamp.Should().Be(2818);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-D6-A1-06-20-FE");

            // Act
            FeedbackMessage message = Target.CreateInputMessage(bytes) as FeedbackMessage;

            // Assert
            Assert.IsNotNull(message);
            message.MessageType.Should().Be(BiDiBMessage.MSG_BM_FREE);
            message.FeedbackNumber.Should().Be(6);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackOccupiedMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-F2-A0-05-0C-FE");

            // Act
            FeedbackOccupiedMessage message = Target.CreateInputMessage(bytes) as FeedbackOccupiedMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(5);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackMultipleMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("07-06-00-21-A2-08-08-20");

            // Act
            FeedbackMultipleMessage message = Target.CreateInputMessage(bytes) as FeedbackMultipleMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackNumber.Should().Be(8);
            message.StateSize.Should().Be(8);
            message.PortStates[5].Should().BeTrue();
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackSpeedMessage_WithForwardDirection()
        {
            // Arrange
            byte[] bytes = GetBytes("07-00-0A-A6-03-00-09-00-8A-FE");

            // Act
            FeedbackSpeedMessage message = Target.CreateInputMessage(bytes) as FeedbackSpeedMessage;

            // Assert
            Assert.IsNotNull(message);
            message.DecoderAddress.Should().Be(3);
            message.Speed.Should().Be(9);
            message.Direction.Should().Be(DecoderDirection.ForwardDirection);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackSpeedMessage_WithBackwardDirection()
        {
            // Arrange
            byte[] bytes = GetBytes("07-00-0A-A6-EC-A7-09-00-8A-FE");

            // Act
            FeedbackSpeedMessage message = Target.CreateInputMessage(bytes) as FeedbackSpeedMessage;

            // Assert
            Assert.IsNotNull(message);
            message.DecoderAddress.Should().Be(10220);
            message.Speed.Should().Be(9);
            message.Direction.Should().Be(DecoderDirection.BackwardDirection);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnFeedbackPositionMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("09-01-00-2D-AC-01-00-00-03-00-22-FE");

            // Act
            FeedbackPositionMessage message = Target.CreateInputMessage(bytes) as FeedbackPositionMessage;

            // Assert
            Assert.IsNotNull(message);
            message.FeedbackAddress.Should().Be(1);
            message.FeedbackType.Should().Be(0);
            message.Location.Should().Be(3);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysMagicMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-00-81-FE-AF-89-FE");

            // Act
            SysMagicMessage message = Target.CreateInputMessage(bytes) as SysMagicMessage;

            // Assert
            Assert.IsNotNull(message);
            message.MagicHigh.Should().Be(0xAF);
            message.MagicLow.Should().Be(0xFE);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnSysMagicMessage_WithBootMagic()
        {
            // Arrange
            byte[] bytes = GetBytes("05-00-00-81-0D-B0-89-FE");

            // Act
            SysMagicMessage message = Target.CreateInputMessage(bytes) as SysMagicMessage;

            // Assert
            Assert.IsNotNull(message);
            message.MagicHigh.Should().Be(0xB0);
            message.MagicLow.Should().Be(0x0D);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnVendorAckMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("05-01-00-8D-94-01-F2-FE");

            // Act
            VendorAckMessage message = Target.CreateInputMessage(bytes) as VendorAckMessage;

            // Assert
            Assert.IsNotNull(message);
            message.ConfigModeEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnVendorMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-01-00-03-93-03-32-34-30-03-31-34-39-FD-DD-FE");

            // Act
            VendorMessage message = Target.CreateInputMessage(bytes) as VendorMessage;

            // Assert
            Assert.IsNotNull(message);
            message.CvName.Should().Be("240");
            message.CvValue.Should().Be("149");
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnNull_WhenInvalidMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0D-01-00-03-93-03-32-34-30-03-31-34-FE");

            // Act
            var message = Target.CreateInputMessage(bytes);

            // Assert
            message.Should().BeNull();
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnAccessoryParaMessage_NotExistingParam()
        {
            // Arrange
            byte[] bytes = GetBytes("07-01-00-6F-B9-01-FF-FC");

            // Act
            AccessoryParaMessage message = Target.CreateInputMessage(bytes) as AccessoryParaMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Parameter.Should().Be(AccessoryParameter.ACCESSORY_PARA_NOTEXIST);
            message.Data.Should().BeNull();
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnAccessoryParaMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("07-01-00-77-B9-05-FC-FE");

            // Act
            AccessoryParaMessage message = Target.CreateInputMessage(bytes) as AccessoryParaMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Parameter.Should().Be(AccessoryParameter.ACCESSORY_PARA_STARTUP);
            message.Data.Should().HaveCount(1);
            message.Data[0].Should().Be(254);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnAccessoryStateMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0D-01-00-1D-B8-00-03-14-01-DB-01-69-02-14");

            // Act
            AccessoryStateMessage message = Target.CreateInputMessage(bytes) as AccessoryStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Number.Should().Be(0);
            message.Aspect.Should().Be(3);
            message.Total.Should().Be(20);
            message.Execute.Should().Be(1);
            message.ExecutionState.Should().Be(AccessoryExecutionState.Running);
            message.Wait.Should().Be(219);
            message.WaitTime.Should().Be(91);
            message.Options.Should().HaveCount(4);
            message.Options.Should().ContainInOrder(1, 105, 2, 20);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnAccessoryStateMessageWithError()
        {
            // Arrange
            byte[] bytes = GetBytes("0D-01-00-54-B8-00-FF-30-80-01-01-00-02-00-37-FE");

            // Act
            AccessoryStateMessage message = Target.CreateInputMessage(bytes) as AccessoryStateMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Number.Should().Be(0);
            message.Aspect.Should().Be(255);
            message.Total.Should().Be(48);
            message.Execute.Should().Be(128);
            message.ExecutionState.Should().Be(AccessoryExecutionState.Error);
            message.HasMoreErrors.Should().BeFalse();
            message.ErrorState.Should().Be(AccessoryErrorState.BIDIB_ACC_STATE_ERROR_VOID);
            message.Wait.Should().Be(1);
            message.Options.Should().HaveCount(4);
            message.Options.Should().ContainInOrder(1, 0, 2, 0);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnLcConfigXMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("10-01-00-2C-C6-01-03-44-00-01-43-00-06-02-03-01-CB-0B-FE");

            // Act
            LcConfigXMessage message = Target.CreateInputMessage(bytes) as LcConfigXMessage;

            // Assert
            Assert.IsNotNull(message);
            message.PortNumber.Should().Be(3);
            message.PortType.Should().Be(PortType.Light);
            message.Data.Should().HaveCount(10);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnLStatMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("07-01-00-04-C0-04-00-E3-5E-FE");

            // Act
            LcStatMessage message = Target.CreateInputMessage(bytes) as LcStatMessage;

            // Assert
            Assert.IsNotNull(message);
            message.PortNumber.Should().Be(0);
            message.PortType.Should().Be(PortType.Motor);
            message.Status.Should().HaveCount(1);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnBoostStatMessage_WithOFFShort()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-C5-B0-01");

            // Act
            BoostStatMessage message = Target.CreateInputMessage(bytes) as BoostStatMessage;

            // Assert
            Assert.IsNotNull(message);
            message.State.Should().Be(BoosterState.BIDIB_BST_STATE_OFF_SHORT);
            message.Control.Should().Be(BoosterControl.Bus);
        }
       
        [TestMethod]
        public void CreateInputMessage_ShouldReturnBoostStatMessage_WithOnHot()
        {
            // Arrange
            byte[] bytes = GetBytes("04-00-C5-B0-82");

            // Act
            BoostStatMessage message = Target.CreateInputMessage(bytes) as BoostStatMessage;


            // Assert
            Assert.IsNotNull(message);
            message.State.Should().Be(BoosterState.BIDIB_BST_STATE_ON_HOT);
            message.Control.Should().Be(BoosterControl.Bus);
        }


        [TestMethod]
        public void CreateInputMessage_ShouldReturnBoostStatMessage_WithBusControl()
        {
            // Arrange
            byte[] bytes = GetBytes("06-01-05-00-69-B0-80");

            // Act
            BoostStatMessage message = Target.CreateInputMessage(bytes) as BoostStatMessage;

            // Assert
            Assert.IsNotNull(message);
            message.State.Should().Be(BoosterState.BIDIB_BST_STATE_ON);
            message.Control.Should().Be(BoosterControl.Bus);
        }
        
        [TestMethod]
        public void CreateInputMessage_ShouldReturnBoostStatMessage_WithLocalControl()
        {
            // Arrange
            byte[] bytes = GetBytes("06-01-05-00-67-B0-C0");

            // Act
            BoostStatMessage message = Target.CreateInputMessage(bytes) as BoostStatMessage;

            // Assert
            Assert.IsNotNull(message);
            message.State.Should().Be(BoosterState.BIDIB_BST_STATE_ON);
            message.Control.Should().Be(BoosterControl.Local);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnResponseSubscriptionMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0D-01-00-00-51-09-00-00-00-00-00-FD-01-01-C0");

            // Act
            GuestResponseSubscriptionMessage message = Target.CreateInputMessage(bytes) as GuestResponseSubscriptionMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Result.Should().Be(SubscriptionResult.SubscriptionNotEstablished);
            message.MultiNodeResolution.Should().BeTrue();
            message.TargetMode.Should().Be(TargetMode.BIDIB_TARGET_MODE_BOOSTER);
            message.DownstreamSubscriptions.Should().Be(1);
            message.UpstreamSubscriptions.Should().Be(1);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnResponseSentMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0D-01-00-00-52-09-00-00-00-00-00-02-02-01-C0");

            // Act
            GuestResponseSentMessage message = Target.CreateInputMessage(bytes) as GuestResponseSentMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Result.Should().Be(RequestResult.GrantedNoResponse);
            message.AckSequenceNumber.Should().Be(2);
        }

        [TestMethod]
        public void CreateInputMessage_ShouldReturnResponseNotifyMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0C-01-00-00-53-09-00-00-00-00-00-02-A1-C0");

            // Act
            GuestResponseNotifyMessage message = Target.CreateInputMessage(bytes) as GuestResponseNotifyMessage;

            // Assert
            Assert.IsNotNull(message);
            message.SubSequenceNumber.Should().Be(2);
            message.SubMessageType.Should().Be(BiDiBMessage.MSG_BM_FREE);
        }        
        
        [TestMethod]
        public void CreateInputMessage_ShouldReturnResponseSubscriptionCountMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("0B-00-50-50-00-0D-84-00-24-00-01-00");

            // Act
            GuestResponseSubscriptionCountMessage message = Target.CreateInputMessage(bytes) as GuestResponseSubscriptionCountMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Count.Should().Be(1);
        }
        
        [TestMethod]
        public void CreateInputMessage_ShouldReturnResponseCommandStationDriveAckMessage()
        {
            // Arrange
            byte[] bytes = GetBytes("07-01-00-11-E2-10-00-01");

            // Act
            var inputMessage = Target.CreateInputMessage(bytes);
            CommandStationDriveAckMessage message = inputMessage as CommandStationDriveAckMessage;

            // Assert
            Assert.IsNotNull(message);
            message.Address.GetArrayValue().Should().Be(1);
        }
    }
}