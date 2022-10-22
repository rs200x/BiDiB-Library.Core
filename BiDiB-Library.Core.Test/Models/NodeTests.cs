using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class NodeTests : TestClass<Node>
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Target = new BiDiBNode { NumberOfProductIdBits = 16, UniqueId = 40532454695511040 };
        }


        [TestMethod]
        public void SetUniqueId_ShouldUpdateNodeProperties()
        {
            // Arrange

            // Act
            Target.UniqueId = 40532454695511040;

            // Assert
            Target.ManufacturerId.Should().Be(13);
            Target.ProductId.Should().Be(132);
        }

        [TestMethod]
        public void SetNumberOfProductIdBits_ShouldUpdateNodeProperties()
        {
            // Arrange
            Target.NumberOfProductIdBits = 1;

            // Act
            Target.NumberOfProductIdBits = 16;

            // Assert
            Target.ManufacturerId.Should().Be(13);
            Target.ProductId.Should().Be(132);
        }

    }
}