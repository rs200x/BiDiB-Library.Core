using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.Messages.Input;
using org.bidib.netbidibc.core.Models.Messages.Output;
using FeatureCountInputMessage = org.bidib.netbidibc.core.Models.Messages.Input.FeatureCountMessage;
using FeatureInputMessage = org.bidib.netbidibc.core.Models.Messages.Input.FeatureMessage;
using FeatureGetAllOutputMessage = org.bidib.netbidibc.core.Models.Messages.Output.FeatureGetAllMessage;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBNodeTests : TestClass<BiDiBNode>
    {
        private Mock<IBiDiBMessageProcessor> messageProcessor;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            messageProcessor = new Mock<IBiDiBMessageProcessor>();

            Target = new BiDiBNode(messageProcessor.Object, NullLogger<BiDiBNode>.Instance);
        }

        [TestMethod]
        public void Enable_ShouldSendSysEnableMessage()
        {
            // Act
            Target.Enable();

            // Assert
            Target.IsEnabled.Should().BeTrue();
            messageProcessor.Verify(x => x.SendMessage(Target, BiDiBMessage.MSG_SYS_ENABLE));
        }

        [TestMethod]
        public void Enable_ShouldSendSysDisableMessage()
        {
            // Arrange
            Target.Enable();

            // Act
            Target.Disable();

            // Assert
            Target.IsEnabled.Should().BeFalse();
            messageProcessor.Verify(x => x.SendMessage(Target, BiDiBMessage.MSG_SYS_DISABLE));
        }

        [TestMethod]
        public void GetFeatures_ShouldRequestFeatures()
        {
            // Arrange
            var featureCountBytes = GetBytes("05-02-00-03-92-05");
            messageProcessor.Setup(x => x.SendMessage<FeatureCountInputMessage>(It.IsAny<FeatureGetAllOutputMessage>(), 500))
                .Returns(new FeatureCountInputMessage(featureCountBytes));

            int featureIndex = 0;

            messageProcessor
                .Setup(x => x.SendMessages<FeatureInputMessage>(It.IsAny<ICollection<BiDiBOutputMessage>>(), 500)).Returns<ICollection<BiDiBOutputMessage>, int>((mi, to) =>
                {
                    var featureMessages = new List<FeatureInputMessage>();
                    if (mi is ICollection<BiDiBOutputMessage> messages)
                    {

                        for (int i = 0; i < messages.Count; i++)
                        {
                            featureMessages.Add(new FeatureInputMessage(GetBytes($"05-00-15-90-0{featureIndex++}-01")));
                        }
                    }

                    return featureMessages;
                });

            // Act
            Target.GetFeatures();

            // Assert
            Target.FeatureCount.Should().Be(5);
            Target.Features.Length.Should().Be(5);
        }

        [TestMethod]
        public void GetFeedbackInfo_ShouldSetFeedbackPorts()
        {
            // Arrange
            byte[] bytes = GetBytes("09-01-00-0A-A2-00-18-00-01-00");
            FeedbackMultipleMessage message = new FeedbackMultipleMessage(bytes);
            messageProcessor.Setup(x => x.SendMessage<FeedbackMultipleMessage>(Target, BiDiBMessage.MSG_BM_GET_RANGE, 500, 0, 20))
                .Returns(message);


            // Act
            Target.GetType().GetMethod("GetFeedbackInfo", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(Target, new[] { new Feature { Value = 20 } });
            

            // Assert
            Target.FeedbackPorts.Should().HaveCount(24);
            Target.FeedbackPorts.Count(x => !x.IsFree).Should().Be(1);
            Target.FeedbackPorts[8].IsFree.Should().BeFalse();
            Target.FeedbackPorts.Count(x => x.IsFree).Should().Be(23);

            messageProcessor.Verify(x => x.SendMessage(Target, BiDiBMessage.MSG_BM_ADDR_GET_RANGE, 0, 20));
            messageProcessor.Verify(x => x.SendMessage(Target, BiDiBMessage.MSG_BM_GET_CONFIDENCE));
        }
    }
}