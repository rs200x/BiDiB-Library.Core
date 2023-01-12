using System;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models.VendorCv;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem("TestData/BiDiBCV-13-120.xml", "TestData")]
    [DeploymentItem("TestData/BiDiBCV-13-104.xml", "TestData")]
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
            string fileName = "TestData/BiDiBCV-13-120.xml";
            VendorCv vendorCv = LoadVendorCv(fileName);

            // Act
            Target.ConvertToV2Structure(vendorCv);
            
            // Assert
            vendorCv.Cvs.Should().HaveCountGreaterThan(400);
        }

        [TestMethod]
        public void ConvertToV2Structure_ShouldResolveTemplates()
        {
            // Arrange
            string fileName = "TestData/BiDiBCV-13-104.xml";
            VendorCv vendorCv = LoadVendorCv(fileName);

            // Act
            Target.ConvertToV2Structure(vendorCv);


            // Assert
            vendorCv.CvDefinition[3].Nodes.Should().HaveCount(3);
        }

        private static VendorCv LoadVendorCv(string file)
        {
            VendorCv vendorCv = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(VendorCv));
                using (XmlReader reader = XmlReader.Create(file))
                {
                    vendorCv = xmlSerializer.Deserialize(reader) as VendorCv;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            return vendorCv;
        }
    }
}