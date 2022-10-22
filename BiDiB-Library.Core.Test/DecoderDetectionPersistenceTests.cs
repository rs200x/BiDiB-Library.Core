using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Decoder;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem("data/" + TestFileName1)]
    [DeploymentItem("data/Schema/DecoderDB/" + DecoderDetectionXsd)]
    [DeploymentItem("data/Schema/DecoderDB/" + CommonTypesXsd)]
    public class DecoderDetectionPersistenceTests : TestClass
    {
        private const string TestFileName1 = "DecoderDetection.decdb";

        private const string DecoderDetectionXsd = "decoderDetection.xsd";
        private const string CommonTypesXsd = "commonTypes.xsd";

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