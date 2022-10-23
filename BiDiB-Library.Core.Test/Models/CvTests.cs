using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class CvTests : TestClass
    {
        private Cv target;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            target = new Cv();
        }

        [TestMethod]
        public void TypeShouldReturnByteWhenTypeStringSetToUnknownType()
        {
            TestType("Test", CvType.Byte);
        }

        [TestMethod]
        public void TypeShouldReturnByteWhenTypeStringSetToByte()
        {
            TestType("byte", CvType.Byte);
        }

        [TestMethod]
        public void TypeShouldReturnSelectWhenTypeStringSetToSelect()
        {
            TestType("select", CvType.Select);
        }

        [TestMethod]
        public void TypeShouldReturnSignedByteWhenTypeStringSetTosignedByte()
        {
            TestType("signedByte", CvType.SignedByte);
        }

        private void TestType(string typeValue, CvType expectedType)
        {
            // Arrange
            target.TypeString = typeValue;

            // Act
            CvType type = target.Type;

            // Assert
            type.Should().Be(expectedType);
        }
    }
}