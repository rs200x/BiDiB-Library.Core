using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models.VendorCv;
using org.bidib.netbidibc.core.Services;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(SampleCvFile, "TestData")]
    public class XmlServiceTests : TestClass<XmlService>
    {
        private const string SampleCvFile = "TestData/BiDiBCV-13-104.xml";

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new XmlService(NullLoggerFactory.Instance);
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
    }
}