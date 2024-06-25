using System;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.VendorCv;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem("TestData/BiDiBCV-13-120.xml", "TestData")]
    [DeploymentItem("TestData/BiDiBCV-13-104.xml", "TestData")]
    [DeploymentItem("TestData/BiDiBCV-251-222.xml", "TestData")]
    [DeploymentItem("TestData/commonTypes.xsd", "TestData")]
    public class VendorCvConverterTests : TestClass<VendorCvConverter>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new VendorCvConverter();
        }

        [TestMethod]
        public void ConvertToV2Structure_ShouldResolveCvList()
        {
            // Arrange
            var fileName = "TestData/BiDiBCV-13-120.xml";
            var vendorCv = LoadVendorCv(fileName);

            // Act
            Target.ConvertToV2Structure(vendorCv);
            
            // Assert
            vendorCv.Cvs.Should().HaveCountGreaterThan(400);
        }

        [TestMethod]
        public void ConvertToV2Structure_ShouldResolveTemplates()
        {
            // Arrange
            var fileName = "TestData/BiDiBCV-13-104.xml";
            var vendorCv = LoadVendorCv(fileName);

            // Act
            Target.ConvertToV2Structure(vendorCv);


            // Assert
            vendorCv.CvDefinition[3].Nodes.Should().HaveCount(3);
        }
        
        [TestMethod]
        public void ConvertToV2Structure_ShouldResolveWithNodeRepeaterOffsets()
        {
            // Arrange
            var fileName = "TestData/BiDiBCV-251-222.xml";
            var vendorCv = LoadVendorCv(fileName);

            // Act
            Target.ConvertToV2Structure(vendorCv);


            // Assert
            vendorCv.CvDefinition[3].Nodes.Should().HaveCount(2);
            vendorCv.CvDefinition[3].Nodes[0].Offset.Should().Be(329);
            vendorCv.CvDefinition[3].Nodes[0].CVs[0].Number.Should().Be("329");
            vendorCv.CvDefinition[3].Nodes[1].Offset.Should().Be(379);
            vendorCv.CvDefinition[3].Nodes[1].CVs[0].Number.Should().Be("379");
        }

        private static VendorCv LoadVendorCv(string file)
        {
            VendorCv vendorCv = null;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(VendorCv));
                using var reader = XmlReader.Create(file);
                vendorCv = xmlSerializer.Deserialize(reader) as VendorCv;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            return vendorCv;
        }
    }
}