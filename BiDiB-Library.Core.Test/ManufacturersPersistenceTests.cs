using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.Manufacturers;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem("data/" + ManufacturersFileName)]
    [DeploymentItem("data/Schema/DecoderDB/" + ManufacturersXsd)]
    [DeploymentItem("data/Schema/DecoderDB/" + CommonTypesXsd)]
    public class ManufacturersPersistenceTests : TestClass
    {
        private const string ManufacturersFileName = "Manufacturers.decdb";
        private const string ManufacturersXsd = "manufacturers.xsd";
        private const string CommonTypesXsd = "commonTypes.xsd";

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