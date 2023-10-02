using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Services;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(SampleCvFile, "TestData")]
    [DeploymentItem(SampleArchive, "TestData")]
    public class IoServiceTests : TestClass<IoService>
    {

        private const string SampleCvFile = "TestData/BiDiBCV-13-104.xml";
        private const string SampleArchive = "TestData/bidib_if2_v2.04.03.zip";
        private const string TestArchive = "TestData/Test.zip";

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new IoService(NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void DirectoryExists_ShouldReturnExistence()
        {
            // Arrange

            // Act
            var result = Target.DirectoryExists(TestContext.DeploymentDirectory);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void CreateDirectory_ShouldCreate()
        {
            // Arrange

            // Act
            Target.CreateDirectory(TestContext.DeploymentDirectory + "/Test");

            // Assert
            Target.DirectoryExists(TestContext.DeploymentDirectory + "/Test").Should().BeTrue();
        }

        [TestMethod]
        public void GetDataLines_ShouldReturnEmpty_WhenFileNotExists()
        {
            // Arrange

            // Act
            var lines = Target.GetDataLines("Data.xml");

            // Assert
            lines.Should().HaveCount(0);
        }

        [TestMethod]
        public void GetDataLines_ShouldReturnEmpty_WhenSourceFileNotExists()
        {
            // Arrange

            // Act
            var lines = Target.GetDataLines("firmware.xml", "bidib_if2_v2.zip");

            // Assert
            lines.Should().HaveCount(0);
        }

        [TestMethod]
        public void GetDataLines_ShouldReturnEmpty_WhenSourceFileNotZip()
        {
            // Arrange

            // Act
            var lines = Target.GetDataLines("firmware.xml", SampleCvFile);

            // Assert
            lines.Should().HaveCount(0);
        }


        [TestMethod]
        public void GetDataLines_ShouldReturnLinesOfFile()
        {
            // Arrange

            // Act
            var lines = Target.GetDataLines(SampleCvFile);

            // Assert
            lines.Should().HaveCount(295);
        }

        [TestMethod]
        public void GetDataLines_ShouldReturnLinesOfFileFromZip()
        {
            // Arrange

            // Act
            var lines = Target.GetDataLines("firmware.xml", SampleArchive);

            // Assert
            lines.Should().HaveCount(22);
        }

        [TestMethod]
        public void GetFileStreamFromArchive_ShouldReturnNull_WhenArchiveNotExists()
        {
            // Arrange

            // Act
            var stream = Target.GetFileStreamFromArchive("Archive", "firmware.xml");

            // Assert
            stream.Should().BeNull();
        }

        [TestMethod]
        public void GetFileStreamFromArchive_ShouldReturnNull_WhenEntryNotExists()
        {
            // Arrange

            // Act
            var stream = Target.GetFileStreamFromArchive(SampleArchive, "entry.xml");

            // Assert
            stream.Should().BeNull();
        }

        [TestMethod]
        public void GetFileStreamFromArchive_ShouldReturnEntryStream()
        {
            // Arrange

            // Act
            var stream = Target.GetFileStreamFromArchive(SampleArchive, "firmware.xml");

            // Assert
            stream.Should().NotBeNull();
        }

        [TestMethod]
        public void GetFileExtension_ShouldReturnNull_WhenFileNotExist()
        {
            // Arrange

            // Act
            var result = Target.GetFileExtension("ABC.xml");

            // Assert
            result.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GetFileExtension_ShouldReturnExtension()
        {
            // Arrange

            // Act
            var result = Target.GetFileExtension(SampleCvFile);

            // Assert
            result.Should().Be(".xml");
        }
        
        [TestMethod]
        public void SaveToZip_ShouldSaveToArchive()
        {
            // Arrange

            // Act
            Target.SaveToZip(TestArchive, new List<string>{SampleCvFile});

            // Assert
            File.Exists(TestArchive).Should().BeTrue();
            using var zip = ZipFile.OpenRead(TestArchive);
            zip.Entries.Should().HaveCount(1);
            zip.Entries.FirstOrDefault(x => x.Name == "BiDiBCV-13-104.xml").Should().NotBeNull();
        }

        protected override void OnTestCleanup()
        {
            base.OnTestCleanup();

            if (File.Exists(TestArchive))
            {
                File.Delete(TestArchive);
            }
        }
    }
}