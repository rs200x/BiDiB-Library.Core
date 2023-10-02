using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models
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
            target.Type = expectedType;

            // Act
            CvType type = target.Type;

            // Assert
            type.Should().Be(expectedType);
            typeValue.Should().NotBeEmpty();
        }
    }
}