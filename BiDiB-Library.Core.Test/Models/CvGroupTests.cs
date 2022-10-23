using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using org.bidib.netbidibc.core.Models.Common;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Models
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
            TestType("Test", CvGroupType.List);
        }

        [TestMethod]
        public void TypeShouldReturnCentesimalIntWhenTypeStringSetToCentesimalInt()
        {
            TestType("centesimalInt", CvGroupType.CentesimalInt);
        }

        [TestMethod]
        public void TypeShouldReturnDccAccAddrWhenTypeStringSetToDccAccAddr()
        {
            TestType("dccAccAddr", CvGroupType.DccAccAddr);
        }

        [TestMethod]
        public void TypeShouldReturndccAddrRgWhenTypeStringSetToDccAddrRg()
        {
            TestType("dccAddrRg", CvGroupType.DccAddrRg);
        }

        [TestMethod]
        public void TypeShouldReturnDccLongAddrWhenTypeStringSetToDccLongAddr()
        {
            TestType("dccLongAddr", CvGroupType.DccLongAddr);
        }

        [TestMethod]
        public void TypeShouldReturnDccLongConsistWhenTypeStringSetToDccLongConsist()
        {
            TestType("dccLongConsist", CvGroupType.DccLongConsist);
        }

        [TestMethod]
        public void TypeShouldReturnDccSpeedCurveWhenTypeStringSetToDccSpeedCurve()
        {
            TestType("dccSpeedCurve", CvGroupType.DccSpeedCurve);
        }

        [TestMethod]
        public void TypeShouldReturnIntWhenTypeStringSetToInt()
        {
            TestType("int", CvGroupType.Int);
        }

        [TestMethod]
        public void TypeShouldReturnLongWhenTypeStringSetToLong()
        {
            TestType("long", CvGroupType.Long);
        }

        [TestMethod]
        public void TypeShouldReturnMatrixWhenTypeStringSetToMatrix()
        {
            TestType("matrix", CvGroupType.Matrix);
        }

        [TestMethod]
        public void TypeShouldReturnRgbColorWhenTypeStringSetToRgbColor()
        {
            TestType("rgbColor", CvGroupType.RgbColor);
        }

        [TestMethod]
        public void TypeShouldReturnStringColorWhenTypeStringSetToString()
        {
            TestType("string", CvGroupType.String);
        }

        private void TestType(string typeValue, CvGroupType expectedType)
        {
            // Arrange
            target.TypeString = typeValue;

            // Act
            CvGroupType type = target.Type;

            // Assert
            type.Should().Be(expectedType);
        }
    }
}