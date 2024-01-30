using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using org.bidib.Net.Core.Models.Common;
using org.bidib.Net.Core.Models.Firmware;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class CvReferenceTests : TestClass<CvReference>
{
    protected override void OnTestInitialize()
    {
        base.OnTestInitialize();
        Target = new CvReference();
    }

    [TestMethod]
    public void AllCvs_ShouldReturnCollectedItems_WhenGroup()
    {
        // Arrange
        var cv1 = new Cv();
        var cv2 = new Cv();

        Target.CvItem = new CvGroup { Cvs = [cv1, cv2] };

        // Act
        var items = Target.AllCvs.ToList();

        // Assert
        items.Should().HaveCount(2);
        items.Should().BeEquivalentTo([cv1, cv2]);
    }

    [TestMethod]
    public void AllCvs_ShouldReturnCollectedItems_WhenCv()
    {
        // Arrange
        var cv1 = new Cv();

        Target.CvItem = cv1;
        // Act
        var items = Target.AllCvs.ToList();

        // Assert
        items.Should().HaveCount(1);
        items.Should().BeEquivalentTo([cv1]);
    }

    [TestMethod]
    public void AllCvs_ShouldReturnEmptyList_WhenNoItems()
    {
        // Arrange
        Target.CvItem = null;

        // Act
        var items = Target.AllCvs.ToList();

        // Assert
        items.Should().HaveCount(0);
    }
}