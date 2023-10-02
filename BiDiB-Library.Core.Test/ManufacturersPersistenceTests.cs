using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.Manufacturers;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(ManufacturersFileName, "TestData")]
    [DeploymentItem(ManufacturersXsd, "TestData")]
    [DeploymentItem("TestData/commonTypes.xsd", "TestData")]
    public class ManufacturersPersistenceTests : TestClass
    {
        private const string ManufacturersFileName = "TestData/Manufacturers.decdb";
        private const string ManufacturersXsd = "TestData/manufacturers.xsd";

        [TestMethod]
        public void TestFile1_ShouldBeSchemaValid()
        {
            // Act & Assert
            ValidateFile(ManufacturersFileName, Namespaces.ManufacturersNamespaceUrl, ManufacturersXsd);
        }

        [TestMethod]
        public void TestFile1_ShouldBeDeserializable()
        {
            // Arrange

            // Act
            ManufacturersList manufacturers = LoadFromXmlFile<ManufacturersList>(ManufacturersFileName);

            // Assert
            manufacturers.Should().NotBeNull();

            ManufacturersListVersion version = manufacturers.Version;
            version.Should().NotBeNull();
            version.CreatedBy.Should().Be("DecoderDB");

            manufacturers.Manufacturers.Should().NotBeNull();
            manufacturers.Manufacturers.Length.Should().BeGreaterThan(50);

            Manufacturer manufacturer = manufacturers.Manufacturers.FirstOrDefault(x => x.Id == 97);
            manufacturer.Should().NotBeNull();
            manufacturer.Name.Should().Be("Doehler & Haass");
            manufacturer.ShortName.Should().Be("D&H");
            manufacturer.Country.Should().Be("DE");
        }

        [TestMethod]
        public void Manufacturers_ShouldBeSerializable()
        {
            // Arrange
            ManufacturersList manufacturers = new ManufacturersList();
            ManufacturersListVersion version = new ManufacturersListVersion
            {
                LastUpdate = DateTime.Today,
                ListDate = DateTime.Today
            };

            manufacturers.Version = version;

            Manufacturer manufacturer1 = new Manufacturer {Id = 1, Name = "AB", ShortName = "AB"};
            Manufacturer manufacturer2 = new Manufacturer {Id = 2, Name = "ABC", ShortName = "ABC"};

            manufacturers.Manufacturers = new[] {manufacturer1, manufacturer2};

            // Act
            SaveToXmlFile(manufacturers, "ManufacturersTest.xml");

            // Assert
            ManufacturersList loadedManufacturers = LoadFromXmlFile<ManufacturersList>("ManufacturersTest.xml");
            loadedManufacturers.Should().NotBeNull();
        }
    }
}