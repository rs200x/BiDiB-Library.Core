using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Decoder;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(TestFileName1, "TestData")]
    [DeploymentItem(DecoderDetectionXsd, "TestData")]
    [DeploymentItem("TestData/commonTypes.xsd", "TestData")]
    public class DecoderDetectionPersistenceTests : TestClass
    {
        private const string TestFileName1 = "TestData/DecoderDetection.decdb";
        private const string DecoderDetectionXsd = "TestData/decoderDetection.xsd";

        [TestMethod]
        public void TestFiles_ShouldBeSchemaValid()
        {
            // Arrange

            // Act
            ValidateFile(TestFileName1, Namespaces.DecoderDetectionNamespaceUrl, DecoderDetectionXsd);

            // Assert
        }

        [TestMethod]
        public void TestFile1_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            DecoderDetection definition = LoadFromXmlFile<DecoderDetection>(TestFileName1);

            // Assert
            definition.Should().NotBeNull();
            definition.Protocols.Should().HaveCount(3);
            DecoderDetectionProtocol protocol = definition.Protocols[0];
            protocol.Type.Should().Be(ProtocolType.DCC);
            protocol.Default.Should().HaveCount(3);
            Detection detection = protocol.Default[0];
            detection.Type.Should().Be(DetectionType.ManufacturerId);
            detection.Items.Should().HaveCount(1);
        }

    }
}