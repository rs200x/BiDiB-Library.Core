using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Firmware;
using Version = org.bidib.netbidibc.core.Models.Common.Version;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(TestDataFolder + "13/"+ TestFileName13)]
    [DeploymentItem(TestDataFolder + "145/" + TestFileName145)]
    [DeploymentItem(TestDataFolder + "157/" + TestFileName157)]
    [DeploymentItem(TestDataFolder + "97/" + TestFileName971)]
    [DeploymentItem(TestDataFolder + "97/" + TestFileName972)]
    [DeploymentItem(TestDataFolder + "0/" +TestFileName0)]
    [DeploymentItem("data/Schema/DecoderDB/" + FirmwareXsd)]
    [DeploymentItem("data/Schema/DecoderDB/" + CommonTypesXsd)]
    public class FirmwarePersistenceTests : TestClass
    {
        private const string TestDataFolder = "data/Firmware/";
        private const string TestFileName13 = "Firmware_13-257_3.20.decdb";
        private const string TestFileName145 = "Firmware_145_31.57.decdb";
        private const string TestFileName157 = "Firmware_157_35.1.decdb";
        private const string TestFileName971 = "Firmware_97_3.06.decdb";
        private const string TestFileName972 = "Firmware_97_1.06_sound.decdb";
        private const string TestFileName0 = "Firmware_0_1.0.decdb";

        private const string FirmwareXsd = "decoderFirmware.xsd";
        private const string CommonTypesXsd = "commonTypes.xsd";

        [TestMethod]
        public void TestFile1_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName13, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }

        [TestMethod]
        public void TestFile2_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName145, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }

        [TestMethod]
        public void TestFile3_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName157, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }

        [TestMethod]
        public void TestFile4_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName971, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }

        [TestMethod]
        public void TestFile5_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName972, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }

        [TestMethod]
        public void TestFile6_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(TestFileName0, Namespaces.DecoderFirmwareNamespaceUrl, FirmwareXsd);
        }


        [TestMethod]
        public void TestFile1_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName13);

            // Assert
            definition.Should().NotBeNull();

            Version version = definition.Version;
            version.Should().NotBeNull();
            version.CreatedBy.Should().Be("DecoderDB");

            Firmware firmware = definition.Firmware;
            firmware.Should().NotBeNull();
            firmware.Version.Should().Be("3.20");

            firmware.Decoders.Should().NotBeNull();
            Decoder decoder = firmware.Decoders[0];
            decoder.Name.Should().Be("Car Decoder V3");
            decoder.Type.Should().Be(DecoderType.Car);

            FirmwareProtocol protocol = firmware.Protocols.FirstOrDefault(x => x.Type == ProtocolType.DCC);
            protocol.Should().NotBeNull();

            protocol.CVs.Should().NotBeNull();
            protocol.CvStructure.Should().NotBeNull();
            Category category = protocol.CvStructure[0];
            category.Descriptions.Should().NotBeNull();
            category.Items.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile2_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName145);

            // Assert
            definition.Should().NotBeNull();

            Version version = definition.Version;
            version.Should().NotBeNull();
            version.CreatedBy.Should().Be("DecoderDB");

            Firmware firmware = definition.Firmware;
            firmware.Should().NotBeNull();
            firmware.Version.Should().Be("31.57");
            firmware.ManufacturerId.Should().Be(145);

            FirmwareProtocol protocol = firmware.Protocols.FirstOrDefault(x => x.Type == ProtocolType.DCC);
            protocol.Should().NotBeNull();

            Cv cv49 = protocol.CVs.OfType<Cv>().FirstOrDefault(x => x.Number == 49);
            cv49.Should().NotBeNull();

            cv49.ValueCalculation.Should().NotBeNull();
            cv49.ValueCalculation.Items.Should().HaveCount(5);

            var items = cv49.ValueCalculation.Items;
            items[0].Type.Should().Be(ValueCalculationItemType.Self);
            items[1].Type.Should().Be(ValueCalculationItemType.Operator);
            items[2].Type.Should().Be(ValueCalculationItemType.Constant);
            items[3].Type.Should().Be(ValueCalculationItemType.Operator);
            items[4].Type.Should().Be(ValueCalculationItemType.Constant);
        }

        [TestMethod]
        public void TestFile3_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName157);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile4_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName971);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile5_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName972);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void TestFile6_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            FirmwareDefinition definition = LoadFromXmlFile<FirmwareDefinition>(TestFileName0);

            // Assert
            definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FirmwareDefinition_ShouldBeSerializable()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition
            {
                Version = new Version { LastUpdate = DateTime.Today }
            };

            Description[] descriptions = { new Description { Language = "de", Text = "xx" } };

            Cv cv1 = new Cv { Type = CvType.Byte, Number = 1, Descriptions = descriptions };
            Cv cv2 = new Cv { Type = CvType.Byte, Number = 2, Descriptions = descriptions };
            Cv cv3 = new Cv { Type = CvType.Byte, Number = 3, Descriptions = descriptions };


            Detection detection = new Detection
            {
                Type = DetectionType.FirmwareVersion,
                Items = new object[] { cv1, cv3 }
            };


            CvGroup cvGroup = new CvGroup
            {
                Id = "g1",
                Descriptions = descriptions,
                Cvs = new[] { cv1, cv2 }
            };


            Detection detection2 = new Detection
            {
                Type = DetectionType.FirmwareVersion,
                Items = new object[] { cvGroup }
            };

            Category category = new Category
            {
                Descriptions = descriptions,
                Items = new object[] { new CvReference { Id = "1" } }
            };

            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                DecoderDetection = new[] { detection, detection2 },
                CVs = new CvBase[] { cv1, cvGroup },
                CvStructure = new[] { category }
            };


            Firmware firmware = new Firmware
            {
                Version = "27.7",
                ManufacturerId = 13,
                Decoders = new[] { new Decoder { Name = "Test" } },
                Protocols = new[] { protocol }
            };

            definition.Firmware = firmware;

            // Act
            SaveToXmlFile(definition, "TestFirmware.xml");

            // Assert
            FirmwareDefinition loadedDefinition = LoadFromXmlFile<FirmwareDefinition>("TestFirmware.xml");
            loadedDefinition.Should().NotBeNull();
            loadedDefinition.Version.LastUpdate.Should().Be(DateTime.Today);
            loadedDefinition.Firmware.Version.Should().Be("27.7");
            loadedDefinition.Firmware.ManufacturerId.Should().Be(13);

        }
    }
}