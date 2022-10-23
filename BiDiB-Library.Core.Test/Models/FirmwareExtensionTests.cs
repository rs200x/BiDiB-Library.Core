using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.core.Models.Firmware;
using org.bidib.netbidibc.core.Models.Helpers;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class FirmwareExtensionTests : TestClass
    {
        private Mock<ILogger<FirmwareProtocol>> logger;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            logger = new Mock<ILogger<FirmwareProtocol>>();
            typeof(FirmwareExtensions).GetField("Logger", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, logger.Object);
        }

        [TestMethod]
        public void Restructure_ShouldAssignCvItemsToCvReferences()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv15 = new Cv { Id = "15", Number = 15 };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { new Cv { Id = "1", Number = 1 }, cv15, new Cv { Id = "20", Number = 20 } }
            };

            CvReference cvReference = new CvReference { Id = "15" };

            Category category = new Category { Items = new object[] { cvReference } };

            protocol.CvStructure = new[] { category };
            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            definition.Firmware.Restructure();

            // Assert
            cvReference.CvItem.Should().Be(cv15);
        }

        [TestMethod]
        public void Restructure_ShouldAssignCvItemsToCvReferencesOfSubCategory()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv15 = new Cv { Id = "15", Number = 15 };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { new Cv { Id = "1", Number = 1 }, cv15, new Cv { Id = "20", Number = 20 } }
            };

            CvReference cvReference = new CvReference { Id = "15" };

            Category subCategory = new Category { Items = new object[] { cvReference } };
            Category category = new Category { Items = new object[] { subCategory } };

            protocol.CvStructure = new[] { category };

            firmware.Protocols = new[] { protocol };

            definition.Firmware = firmware;

            // Act
            definition.Firmware.Restructure();

            // Assert
            cvReference.CvItem.Should().Be(cv15);
        }

        [TestMethod]
        public void Restructure_ShouldAssignCvGroupItemsToCvReferences()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            CvGroup cvGroup15 = new CvGroup { Id = "15x" };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { new CvGroup { Id = "1" }, cvGroup15, new Cv { Id = "20", Number = 20 } }
            };

            CvReference cvReference = new CvReference { Id = "15x" };

            Category category = new Category { Items = new object[] { cvReference } };

            protocol.CvStructure = new[] { category };

            firmware.Protocols = new[] { protocol };

            definition.Firmware = firmware;

            // Act
            definition.Firmware.Restructure();

            // Assert
            cvReference.CvItem.Should().Be(cvGroup15);
        }

        [TestMethod]
        public void Restructure_ShouldAssignCvGroupDescriptionToCvItems()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv1 = new Cv { Id = "20" };
            Cv cv2 = new Cv { Id = "21" };
            Description description = new Description();
            CvGroup cvGroup15 = new CvGroup { Id = "15x", Cvs = new[] { cv1, cv2 }, Descriptions = new[] { description } };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cvGroup15 }
            };

            CvReference cvReference = new CvReference { Id = "15x" };

            Category category = new Category { Items = new object[] { cvReference } };

            protocol.CvStructure = new[] { category };

            firmware.Protocols = new[] { protocol };

            definition.Firmware = firmware;

            // Act
            definition.Firmware.Restructure();

            // Assert
            cv1.GroupDescriptions.Should().Contain(description);
            cv2.GroupDescriptions.Should().Contain(description);
        }

        [TestMethod]
        public void Validate_ShouldThrow_WhenFirmwareNull()
        {
            // Act
            Action action = () => FirmwareExtensions.Validate(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "firmware");
        }

        [TestMethod]
        public void Validate_ShouldLog_WhenMultipleCvWithSameId()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv1 = new Cv { Id = "20" };
            Cv cv2 = new Cv { Id = "20" };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cv1, cv2 }
            };

            firmware.Protocols = new[] { protocol };

            definition.Firmware = firmware;

            // Act
            firmware.Validate();

            // Assert
            logger.Verify(x => x.LogWarning(It.Is<string>(m => m.Contains("is defined more than once!"))));
        }

        [TestMethod]
        public void Validate_ShouldLog_WhenCvIsAlsoInGroup()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv1 = new Cv { Id = "20" };
            CvGroup cvGroup = new CvGroup { Id = "G1", Cvs = new[] { cv1 } };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cv1, cvGroup }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.Validate();

            // Assert
            logger.Verify(x => x.LogWarning(It.Is<string>(m => m.Contains("is also defined within a CvGroup!"))));
        }

        [TestMethod]
        public void Validate_ShouldLog_WhenCvTypeSelectHasNoItems()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv1 = new Cv { Id = "20", Type = CvType.Select };
            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cv1 }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.Validate();

            // Assert
            logger.Verify(x => x.LogWarning(It.Is<string>(m => m.Contains("is of type 'Select' but has no items defined!"))));
        }

        [TestMethod]
        public void Validate_ShouldLog_WhenCvOfCvReferenceNotExists()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();
            CvReference cvReference = new CvReference { Id = "15" };
            Category category = new Category { Items = new object[] { cvReference } };

            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[0],
                CvStructure = new[] { category }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.Validate();

            // Assert
            logger.Verify(x => x.LogWarning(It.Is<string>(m => m.Contains("points to a not defined CV!"))));
        }

        [TestMethod]
        public void Validate_ShouldLog_WhenCvOfCvReferenceIsInGroup()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv1 = new Cv { Id = "15" };
            CvGroup cvGroup = new CvGroup { Id = "G1", Cvs = new[] { cv1 } };

            CvReference cvReference = new CvReference { Id = "15" };
            Category category = new Category { Items = new object[] { cvReference } };

            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cvGroup },
                CvStructure = new[] { category }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.Validate();

            // Assert
            logger.Verify(x => x.LogWarning(It.Is<string>(m => m.Contains("points to a CV which is defined within a CvGroup!"))));
        }

        [TestMethod]
        public void FilterDuplicated_ShouldRemoveDuplicatedItems()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv09 = new Cv { Id = "9", Number = 9 };
            Cv cv101 = new Cv { Id = "15", Number = 10 };
            Cv cv102 = new Cv { Id = "20", Number = 10 };
            Cv cv11 = new Cv { Id = "21", Number = 11 };

            CvReference cvReference9 = new CvReference { Id = cv09.Id, CvItem = cv09 };
            CvReference cvReference101 = new CvReference { Id = cv101.Id, CvItem = cv101 };
            CvReference cvReference102 = new CvReference { Id = cv102.Id, CvItem = cv102 };
            CvReference cvReference11 = new CvReference { Id = cv11.Id, CvItem = cv11 };
            Category category1 = new Category { Items = new object[] { cvReference9, cvReference101 } };
            Category category2 = new Category { Items = new object[] { cvReference102, cvReference11, category1 } };

            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { cv09, cv101, cv102, cv11 },
                CvStructure = new[] { category2 }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.FilterDuplicated();

            // Assert
            protocol.CVs.Should().HaveCount(3);
            category1.Items.Should().HaveCount(2);
            category2.Items.Should().BeEquivalentTo(new object[] { cvReference11, category1 });
        }

        [TestMethod]
        public void FilterDuplicated_ShouldRemoveDuplicatedCvGroups()
        {
            // Arrange
            FirmwareDefinition definition = new FirmwareDefinition();

            Firmware firmware = new Firmware();

            Cv cv09 = new Cv { Id = "9", Number = 9 };
            Cv cv101 = new Cv { Id = "15", Number = 10 };
            Cv cv102 = new Cv { Id = "20", Number = 10 };

            CvGroup group1 = new CvGroup { Cvs = new[] { cv09, cv101, cv102 } };
            CvGroup group2 = new CvGroup { Cvs = new[] { cv09, cv101, cv102 } };
            CvGroup group3 = new CvGroup { Cvs = new[] { cv101, cv102 } };

            CvReference cvReferenceG1 = new CvReference { Id = "G1", CvItem = group1 };
            CvReference cvReferenceG2 = new CvReference { Id = "G2", CvItem = group2 };
            CvReference cvReferenceG3 = new CvReference { Id = "G3", CvItem = group3 };
            Category category1 = new Category { Items = new object[] { cvReferenceG1, cvReferenceG3 } };
            Category category2 = new Category { Items = new object[] { category1, cvReferenceG2, } };

            FirmwareProtocol protocol = new FirmwareProtocol
            {
                Type = ProtocolType.DCC,
                CVs = new CvBase[] { group1, group2, group3 },
                CvStructure = new[] { category2 }
            };

            firmware.Protocols = new[] { protocol };
            definition.Firmware = firmware;

            // Act
            firmware.FilterDuplicated();

            // Assert
            protocol.CVs.Should().HaveCount(2);
            category1.Items.Should().HaveCount(2);
            category1.Items.Should().BeEquivalentTo(new object[] { cvReferenceG1, cvReferenceG3 });
            category2.Items.Should().HaveCount(1);
            category2.Items.Should().BeEquivalentTo(new object[] { category1 });
        }
    }
}