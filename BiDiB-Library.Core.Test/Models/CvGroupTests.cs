using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class CvGroupTests : TestClass
    {
        private CvGroup target;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            target = new CvGroup();
        }

        [TestMethod]
        public void TypeShouldReturnListWhenTypeStringSetToUnknownType()
        {
            TestType("CVG", CvGroupType.List);
        }

        [TestMethod]
        public void TypeShouldReturnCentesimalIntWhenTypeStringSetToCentesimalInt()
        {
            TestType("CET", CvGroupType.CentesimalInt);
        }

        [TestMethod]
        public void TypeShouldReturnDccAccAddrWhenTypeStringSetToDccAccAddr()
        {
            TestType("DAA", CvGroupType.DccAccAddr);
        }

        [TestMethod]
        public void TypeShouldReturndccAddrRgWhenTypeStringSetToDccAddrRg()
        {
            TestType("DAR", CvGroupType.DccAddrRg);
        }

        [TestMethod]
        public void TypeShouldReturnDccLongAddrWhenTypeStringSetToDccLongAddr()
        {
            TestType("DLA", CvGroupType.DccLongAddr);
        }

        [TestMethod]
        public void TypeShouldReturnDccLongConsistWhenTypeStringSetToDccLongConsist()
        {
            TestType("DLC", CvGroupType.DccLongConsist);
        }

        [TestMethod]
        public void TypeShouldReturnDccSpeedCurveWhenTypeStringSetToDccSpeedCurve()
        {
            TestType("DSC", CvGroupType.DccSpeedCurve);
        }

        [TestMethod]
        public void TypeShouldReturnIntWhenTypeStringSetToInt()
        {
            TestType("INT", CvGroupType.Int);
        }

        [TestMethod]
        public void TypeShouldReturnLongWhenTypeStringSetToLong()
        {
            TestType("LNG", CvGroupType.Long);
        }

        [TestMethod]
        public void TypeShouldReturnMatrixWhenTypeStringSetToMatrix()
        {
            TestType("MTX", CvGroupType.Matrix);
        }

        [TestMethod]
        public void TypeShouldReturnRgbColorWhenTypeStringSetToRgbColor()
        {
            TestType("RGB", CvGroupType.RgbColor);
        }

        [TestMethod]
        public void TypeShouldReturnStringColorWhenTypeStringSetToString()
        {
            TestType("STR", CvGroupType.String);
        }

        private void TestType(string typeValue, CvGroupType expectedType)
        {
            // Arrange
           
            // Act
            target.Type = expectedType;

            // Assert
            target.TypeShortName.Should().Be(typeValue);
        }
    }
}