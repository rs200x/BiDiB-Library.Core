using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models;

[TestClass]
[TestCategory(TestCategory.UnitTest)]
public class NodeExtensionTests : TestClass
{
    [TestMethod]
    public void SetUniqueIdExtension_ShouldSetUniqueIdAndUpdateNodeProperties()
    {
        // Arrange
        var node = new BiDiBNode();
        var bytes = GetBytes("90-00-0D-84-00-24-00");

        // Act
        node.SetUniqueId(bytes);

        // Assert
        node.UniqueId.Should().Be(40532454695511040);
    }

    [TestMethod]
    public void GetUniqueIdBytes_ShouldReturnUniqueIdByteArray()
    {
        // Arrange
        var node = new BiDiBNode { NumberOfProductIdBits = 1, UniqueId = 40532454695511040 };
        var bytes = GetBytes("90-00-0D-84-00-24-00");

        // Act
        var uniqueIdBytes = node.GetUniqueIdBytes();

        // Assert
        uniqueIdBytes.Should().BeEquivalentTo(bytes);
    }

    [TestMethod]
    public void GetAddress_ShouldReturnIntValueOfAddressArray()
    {
        // Arrange
        var node0 = new BiDiBNode { Address = [0] };
        var node1 = new BiDiBNode { Address = [1] };
        var node2 = new BiDiBNode { Address = [2] };
        var node11 = new BiDiBNode { Address = [1, 1] };
        var node13 = new BiDiBNode { Address = [1, 3] };
        var node111 = new BiDiBNode { Address = [1, 1, 1] };
        var node1111 = new BiDiBNode { Address = [1, 1, 1, 1] };

        // Act
        var address0 = node0.GetAddress();
        var address1 = node1.GetAddress();
        var address2 = node2.GetAddress();
        var address11 = node11.GetAddress();
        var address13 = node13.GetAddress();
        var address111 = node111.GetAddress();
        var address1111 = node1111.GetAddress();

        // Assert
        address0.Should().Be(0);
        address1.Should().Be(1);
        address2.Should().Be(2);
        address11.Should().Be(257);
        address13.Should().Be(769);
        address111.Should().Be(65793);
        address1111.Should().Be(16843009);
    }

    [TestMethod]
    public void GetParentAddress_ShouldReturn0WhenNodeIsRoot()
    {
        // Arrange
        var node0 = new BiDiBNode { Address = [0] };

        // Act
        var address0 = node0.GetParentAddress();

        // Assert
        address0.Should().Be(0);
    }

    [TestMethod]
    public void GetParentAddress_ShouldReturn0WhenNodeIsChildOfRoot()
    {
        // Arrange
        var node0 = new BiDiBNode { Address = [5] };

        // Act
        var address0 = node0.GetParentAddress();

        // Assert
        address0.Should().Be(0);
    }

    [TestMethod]
    public void GetParentAddress_ShouldReturn1WhenNodeIsChildOfSubInterface()
    {
        // Arrange
        var node0 = new BiDiBNode { Address = [1, 1] };

        // Act
        var address0 = node0.GetParentAddress();

        // Assert
        address0.Should().Be(1);
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenParentIsRoot()
    {
        // Arrange
        var root = new BiDiBNode { Address = [0] };
        var sub = new BiDiBNode { Address = [1] };

        // Act & Assert
        sub.IsSubNodeOf(root).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeIsRoot()
    {
        // Arrange
        var root = new BiDiBNode { Address = [0] };
        var sub = new BiDiBNode { Address = [1] };

        // Act & Assert
        root.IsSubNodeOf(sub).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeSecondLevelOfFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeThirdLevelOfFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [1, 1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeThirdLevelOfSecondLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfSecondLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfThirdLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeTrue();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [2, 1, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherSecondLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1] };
        var sub = new BiDiBNode { Address = [1, 2, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherThirdLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 2, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameSecondLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1] };
        var sub = new BiDiBNode { Address = [1, 2] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameThirdLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 2] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameFourthLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1, 1, 1] };
        var sub = new BiDiBNode { Address = [1, 1, 1, 2] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeSecondLevelOfOtherFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [2, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeThirdLevelOfOtherFirstLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1] };
        var sub = new BiDiBNode { Address = [2, 1, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void IsSubNodeOf_ShouldReturnFalse_WhenNodeThirdLevelOfOtherSecondLevel()
    {
        // Arrange
        var parent = new BiDiBNode { Address = [1, 1] };
        var sub = new BiDiBNode { Address = [1, 2, 1] };

        // Act & Assert
        sub.IsSubNodeOf(parent).Should().BeFalse();
    }

    [TestMethod]
    public void GetUniqueIdWithoutClass_ShouldReturn0_WhenNoNode()
    {
        // Arrange

        // Act & Assert
        NodeExtensions.GetUniqueIdWithoutClass(null).Should().Be(0);
    }

    [TestMethod]
    public void GetUniqueIdWithoutClass_ShouldReturnValue()
    {
        // Arrange
        var node = new BiDiBNode { UniqueIdBytes = [1, 1, 1, 1, 1, 1, 1] };

        // Act & Assert
        node.GetUniqueIdWithoutClass().Should().Be(4311810305L);
    }

    [TestMethod]
    public void GetUniqueIdBytesWithoutClass_ShouldReturnEmpty_WhenNoNode()
    {
        // Arrange

        // Act & Assert
        NodeExtensions.GetUniqueIdBytesWithoutClass(null).Should().HaveCount(0);
    }

    [TestMethod]
    public void GetUniqueIdBytesWithoutClass_ShouldReturnValue()
    {
        // Arrange
        var node = new BiDiBNode { UniqueIdBytes = [1, 2, 3, 4, 5, 6, 7] };

        // Act & Assert
        node.GetUniqueIdBytesWithoutClass().Should().BeEquivalentTo([0, 0, 3, 4, 5, 6, 7]);
    }

    [TestMethod]
    public void AddOrUpdateFeature_ShouldNotChange_WhenNoFeature()
    {
        // Arrange
        var node = new BiDiBNode();

        // Act
        node.AddOrUpdateFeature(null);

        // Assert
        node.Features.Should().BeNull();
    }

    [TestMethod]
    public void AddOrUpdateFeature_ShouldAddFeature()
    {
        // Arrange
        var node = new BiDiBNode();
        var feature = new Feature();

        // Act 
        node.AddOrUpdateFeature(feature);

        // Assert
        node.Features.Should().HaveCount(1);
        node.Features[0].Should().Be(feature);
    }

    [TestMethod]
    public void AddOrUpdateFeature_ShouldUpdateFeature()
    {
        // Arrange
        var node = new BiDiBNode { Features = [new Feature { Value = 1, FeatureId = 1 }] };
        var feature = new Feature { FeatureId = 1, Value = 2 };

        // Act 
        node.AddOrUpdateFeature(feature);

        // Assert
        node.Features.Should().HaveCount(1);
        node.Features[0].Value.Should().Be(2);
    }

    [TestMethod]
    public void IsFeatureActive_ShouldBeTrue()
    {
        // Arrange
        var node = new BiDiBNode { Features = [new Feature { Value = 1, FeatureId = 1 }] };

        // Act & Assert
        node.IsFeatureActive(BiDiBFeature.FEATURE_BM_ON).Should().BeTrue();
    }

    [TestMethod]
    public void IsFeatureActive_ShouldBeFalse()
    {
        // Arrange
        var node = new BiDiBNode { Features = [new Feature { Value = 0, FeatureId = 1 }] };

        // Act & Assert
        node.IsFeatureActive(BiDiBFeature.FEATURE_BM_ON).Should().BeFalse();
    }

    [TestMethod]
    public void IsFeatureActive_ShouldBeFalse_WhenNotExist()
    {
        // Arrange
        var node = new BiDiBNode { Features = [new Feature { Value = 0, FeatureId = 2 }] };

        // Act & Assert
        node.IsFeatureActive(BiDiBFeature.FEATURE_BM_ON).Should().BeFalse();
    }
}