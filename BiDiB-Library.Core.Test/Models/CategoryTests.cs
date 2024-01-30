using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Firmware;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class CategoryTests : TestClass<Category>
{
    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();
        Target = new Category();
    }

    [TestMethod]
    public void AllCvs_ShouldReturnCollectedItems()
    {
        // Arrange
        var cv1 = new Cv();
        var ref1 = new CvReference { CvItem = cv1 };
        var cv2 = new Cv();
        var ref2 = new CvReference { CvItem = cv2 };
        var subCategory = new Category { Items = [ref2] };

        Target.Items = [ref1, subCategory];

        // Act
        var items = Target.AllCvs.ToList();

        // Assert
        items.Should().HaveCount(2);
        items.Should().BeEquivalentTo([cv1, cv2]);
    }

    [TestMethod]
    public void AllCvs_ShouldReturnEmptyList_WhenNoItems()
    {
        // Arrange
        Target.Items = null;

        // Act
        var items = Target.AllCvs.ToList();

        // Assert
        items.Should().HaveCount(0);
    }
}