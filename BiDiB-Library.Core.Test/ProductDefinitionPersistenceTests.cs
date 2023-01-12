using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Product;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(ProductDefinition0, TestDataDecoderFolder)]
    [DeploymentItem(ProductDefinition13, TestDataDecoderFolder)]
    [DeploymentItem(ProductDefinition157, TestDataDecoderFolder)]
    [DeploymentItem(ProductDefinition971, TestDataDecoderFolder)]
    [DeploymentItem(ProductDefinition972, TestDataDecoderFolder)]
    [DeploymentItem(XsdPath, "TestData")]
    [DeploymentItem("TestData/commonTypes.xsd", "TestData")]
    public class ProductDefinitionPersistenceTests : TestClass
    {
        private const string TestDataFolder = "TestData/";
        private const string TestDataDecoderFolder = TestDataFolder + "Decoder/";
        private const string ProductDefinition0 = TestDataDecoderFolder + "Decoder_0_NMRA-Standard.decdb";
        private const string ProductDefinition13 = TestDataDecoderFolder + "Decoder_13-257_CarDecoderV3.decdb";
        private const string ProductDefinition157 = TestDataDecoderFolder + "Decoder_157_N45.decdb";
        private const string ProductDefinition971 = TestDataDecoderFolder + "Decoder_97_DH05C.decdb";
        private const string ProductDefinition972 = TestDataDecoderFolder + "Decoder_97_DH10C.decdb";
        private const string XmlNamespace = Namespaces.DecoderNamespaceUrl;
        private const string XsdPath = TestDataFolder + "decoder.xsd";


        [TestMethod]
        public void TestFile1_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition0, XmlNamespace, XsdPath);
        }

        [TestMethod]
        public void TestFile2_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition13, XmlNamespace, XsdPath);
        }

        [TestMethod]
        public void TestFile3_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition157, XmlNamespace, XsdPath);
        }

        [TestMethod]
        public void TestFile4_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition971, XmlNamespace, XsdPath);
        }

        [TestMethod]
        public void TestFile5_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition971, XmlNamespace, XsdPath);
        }

        [TestMethod]
        public void TestFile1_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            var definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition0);

            // Assert
            definition.Should().NotBeNull();

            definition.Decoder.Should().NotBeNull();
            var decoder = definition.Decoder;

            decoder.Name.Should().Be("NMRA-Standard");
            decoder.ManufacturerId.Should().Be(0);
            decoder.ManufacturerShortName.Should().Be("Standard");
        }

        [TestMethod]
        public void TestFile2_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            var definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition13);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile3_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            var definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition157);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile4_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            var definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition971);

            // Assert
            definition.Should().NotBeNull();
            definition.Decoder.Should().NotBeNull();
            definition.Decoder.Description.Should().HaveCount(2);
            definition.Decoder.ArticleNumbers.Should().Be("DH05C-0;DH05C-1;DH05C-3");
            definition.Decoder.Specifications.Should().NotBeNull();
            definition.Decoder.Specifications.Connectors.List.Should().ContainAll("Pad", "Cable", "NEM651", "NEM652+Cable", "classicSUSI-Pads");
            definition.Decoder.Specifications.Electrical.Should().NotBeNull();
            definition.Decoder.Specifications.Dimensions.Should().NotBeNull();
            definition.Decoder.Specifications.FunctionConnectors.List.Should().ContainAll("F0f", "F0f", "Aux1", "Aux2", "Aux3|SUSI-CLK", "Aux4|SUSI-DAT");
            definition.Decoder.Images.Should().HaveCount(2);
        }

        [TestMethod]
        public void TestFile5_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            var definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition972);

            // Assert
            definition.Should().NotBeNull();
        }
    }
}