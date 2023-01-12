using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.VendorCv;
using org.bidib.netbidibc.core.Models.Xml;
using org.bidib.netbidibc.core.Services;
using org.bidib.netbidibc.core.Services.Interfaces;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(SampleCvFile, "TestData")]
    [DeploymentItem(XsdFile, "TestData")]
    [DeploymentItem("TestData/commonTypes.xsd", "TestData")]
    public class XmlServiceTests : TestClass<XmlService>
    {
        private Mock<IIoService> ioService;
        private const string SampleCvFile = "TestData/BiDiBCV-13-104.xml";
        private const string XsdFile = "TestData/decoder.xsd";

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            ioService = new Mock<IIoService>();
            Target = new XmlService(ioService.Object, NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void LoadFromFile_ShouldDeserializeFile()
        {
            // Arrange

            // Act
            var vendorCv = Target.LoadFromFile<VendorCv>(SampleCvFile);

            // Assert
            vendorCv.Should().NotBeNull();
            vendorCv.Templates.Should().HaveCount(6);
        }

        [TestMethod]
        public void LoadFromFile_ShouldReturnNull_WhenFileNotExist()
        {
            // Arrange

            // Act
            var vendorCv = Target.LoadFromFile<VendorCv>("Data.xml");

            // Assert
            vendorCv.Should().BeNull();
        }

        [TestMethod]
        public void LoadFromStream_ShouldDeserializeStream()
        {
            // Arrange
            var fileStream = new FileStream(SampleCvFile, FileMode.Open);

            // Act
            var vendorCv = Target.LoadFromStream<VendorCv>(fileStream);

            // Assert
            vendorCv.Should().NotBeNull();
            vendorCv.Templates.Should().HaveCount(6);
            fileStream.Dispose();
        }

        [TestMethod]
        public void LoadFromStream_ShouldReturnNull_WhenNoStream()
        {
            // Arrange

            // Act
            var vendorCv = Target.LoadFromStream<VendorCv>(null);

            // Assert
            vendorCv.Should().BeNull();
        }

        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenFileNull()
        {
            // Act
            var info = Target.ValidateFile(null, null, null);

            // Assert
            info.Result.Should().Be(XmlValidationResult.FileError);
        }

        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenFileNotExists()
        {
            // Act
            ioService.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);
            var info = Target.ValidateFile("file", null, null);

            // Assert
            info.Result.Should().Be(XmlValidationResult.FileError);
        }

        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenSchemaNull()
        {
            // Arrange
            File.WriteAllText("test.xml", "Test");
            ioService.Setup(x => x.FileExists("test.xml")).Returns(true);

            // Act
            var info = Target.ValidateFile("test.xml", null, null);

            // Assert
            info.Result.Should().Be(XmlValidationResult.NoSchema);
        }

        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenXsdNull()
        {
            // Arrange
            File.WriteAllText("test.xml", "Test");

            // Act
            var info = Target.ValidateFile("test.xml", "http://www", null);

            // Assert
            info.Result.Should().Be(XmlValidationResult.FileError);
        }

        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenXsdNotExists()
        {
            // Arrange
            File.WriteAllText("test.xml", "Test");
            ioService.Setup(x => x.FileExists("test.xml")).Returns(true);
            ioService.Setup(x => x.FileExists("xsd")).Returns(false);

            // Act
            var info = Target.ValidateFile("test.xml", "http://www", "xsd");

            // Assert
            info.Result.Should().Be(XmlValidationResult.FileError);
        }


        [TestMethod]
        public void ValidateFile_ShouldBeFalse_WhenSchemaNotValid()
        {
            // Arrange
            File.WriteAllText("test.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><DccUserDevices></DccUserDevices>");
            ioService.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            ioService.Setup(x => x.GetDirectory(XsdFile)).Returns("TestData");
            ioService.Setup(x => x.GetPath(It.IsAny<string[]>())).Returns("TestData/commonTypes.xsd");

            // Act
            var info = Target.ValidateFile("test.xml", Namespaces.DecoderNamespaceUrl, XsdFile);

            // Assert
            info.Result.Should().Be(XmlValidationResult.NoSchema);
        }
    }
}