using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Product;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(TestDataFolder + "0/" + ProductDefinition0)]
    [DeploymentItem(TestDataFolder + "13/" + ProductDefinition13)]
    [DeploymentItem(TestDataFolder + "157/" + ProductDefinition157)]
    [DeploymentItem(TestDataFolder + "97/" + ProductDefinition971)]
    [DeploymentItem(TestDataFolder + "97/" + ProductDefinition972)]
    [DeploymentItem("data/Schema/DecoderDB/" + ProductXsd)]
    [DeploymentItem("data/Schema/DecoderDB/" + CommonTypesXsd)]
    public class ProductDefinitionPersistenceTests : TestClass
    {
        private const string TestDataFolder = "data/Decoder/";
        private const string ProductDefinition0 = "Decoder_0_NMRA-Standard.decdb";
        private const string ProductDefinition13 = "Decoder_13-257_CarDecoderV3.decdb";
        private const string ProductDefinition157 = "Decoder_157_N45.decdb";
        private const string ProductDefinition971 = "Decoder_97_DH05C.decdb";
        private const string ProductDefinition972 = "Decoder_97_DH10C.decdb";

        private const string ProductXsd = "decoder.xsd";
        private const string CommonTypesXsd = "commonTypes.xsd";

        [TestMethod]
        public void TestFile1_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition0, Namespaces.Decoder.Url, Namespaces.Decoder.XsdName);
        }

        [TestMethod]
        public void TestFile2_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition13, Namespaces.Decoder.Url, Namespaces.Decoder.XsdName);
        }

        [TestMethod]
        public void TestFile3_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition157, Namespaces.Decoder.Url, Namespaces.Decoder.XsdName);
        }

        [TestMethod]
        public void TestFile4_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition971, Namespaces.Decoder.Url, Namespaces.Decoder.XsdName);
        }

        [TestMethod]
        public void TestFile5_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ProductDefinition971, Namespaces.Decoder.Url, Namespaces.Decoder.XsdName);
        }

        [TestMethod]
        public void TestFile1_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            DecoderDefinition definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition0);

            // Assert
            definition.Should().NotBeNull();

            definition.Decoder.Should().NotBeNull();
            Decoder decoder = definition.Decoder;

            decoder.Name.Should().Be("NMRA-Standard");
            decoder.ManufacturerId.Should().Be(0);
            //product.ManufacturerName.Should().Be("NMRA");
            decoder.ManufacturerShortName.Should().Be("Standard");
            //product.ManufacturerUrl.Should().Be("www.nmra.org");
        }

        [TestMethod]
        public void TestFile2_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            DecoderDefinition definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition13);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile3_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            DecoderDefinition definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition157);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile4_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            DecoderDefinition definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition971);

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
            DecoderDefinition definition = LoadFromXmlFile<DecoderDefinition>(ProductDefinition972);

            // Assert
            definition.Should().NotBeNull();
        }
    }
}