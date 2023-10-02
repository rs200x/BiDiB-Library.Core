using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Helpers;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class ValueCalculationExtensionTests : TestClass
    {
        private ValueCalculation target;

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            target = new ValueCalculation();
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldReturnValue_WhenNoItems()
        {
            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(10.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldReturnValue_WhenEmptyItems()
        {
            // Arrange
            target.Items = Array.Empty<ValueCalculationItem>();

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(10.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldCalculateForward()
        {
            // Arrange
            target.Items = new[]
            {
                new ValueCalculationItem {Type = ValueCalculationItemType.Self},
                new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "+"},
                new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value = "1"}
            };

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(11.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldCalculateBracketValues()
        {
            // Arrange
            target.Items = new[]
            {
                new ValueCalculationItem {Type = ValueCalculationItemType.Bracket, Items = new []
                    {
                    new ValueCalculationItem {Type = ValueCalculationItemType.Self},
                    new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "*"},
                    new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value = "2"}
                    }},
                new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "-"},
                new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value = "3"}
            };

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(17.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldNotChange_WhenTypeCv()
        {
            // Arrange
            target.Items = new[]
            {
                new ValueCalculationItem {Type = ValueCalculationItemType.Self},
                new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "+"},
                new ValueCalculationItem {Type = ValueCalculationItemType.CvValue, Number = 1, IndexHigh = 0, IndexLow = 0}
            };

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(10.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldUseConstant_WhenNoOperator()
        {
            // Arrange
            target.Items = new[]
            {
                new ValueCalculationItem {Type = ValueCalculationItemType.Self},
                new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value= "100"}
            };

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(100.0);
        }

        [TestMethod]
        public void GetCalculatedValue_ShouldUseConstant_WhenUnknownOperator()
        {
            // Arrange
            target.Items = new[]
            {
                new ValueCalculationItem {Type = ValueCalculationItemType.Self},
                new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "/"},
                new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value= "2"},
                new ValueCalculationItem {Type = ValueCalculationItemType.Operator, Value = "--"},
                new ValueCalculationItem {Type = ValueCalculationItemType.Constant, Value= "4"},
            };

            // Act
            double value = target.GetCalculatedValue(10);

            // Assert
            value.Should().Be(4.0);
        }
    }
}
