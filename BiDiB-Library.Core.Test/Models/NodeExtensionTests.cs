using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Models.BiDiB;
using org.bidib.netbidibc.core.Models.BiDiB.Extensions;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class NodeExtensionTests
    {
        [TestMethod]
        public void SetUniqueIdExtension_ShouldSetUniqueIdAndUpdateNodeProperties()
        {
            // Arrange
            BiDiBNode node = new BiDiBNode();
            string messageString = "90-00-0D-84-00-24-00";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            node.SetUniqueId(bytes);

            // Assert
            node.UniqueId.Should().Be(40532454695511040);
        }

        [TestMethod]
        public void GetUniqueIdBytes_ShouldReturnUniqueIdByteArray()
        {
            // Arrange
            BiDiBNode node = new BiDiBNode { NumberOfProductIdBits = 1, UniqueId = 40532454695511040 };
            string messageString = "90-00-0D-84-00-24-00";
            byte[] bytes = Array.ConvertAll(messageString.Split('-'), s => Convert.ToByte(s, 16));

            // Act
            byte[] uniqueIdBytes = node.GetUniqueIdBytes();

            // Assert
            uniqueIdBytes.Should().BeEquivalentTo(bytes);
        }

        [TestMethod]
        public void GetAddress_ShouldReturnIntValueOfAddressArray()
        {
            // Arrange
            BiDiBNode node0 = new BiDiBNode { Address = new byte[] { 0 } };
            BiDiBNode node1 = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode node2 = new BiDiBNode { Address = new byte[] { 2 } };
            BiDiBNode node11 = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode node13 = new BiDiBNode { Address = new byte[] { 1, 3 } };
            BiDiBNode node111 = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };
            BiDiBNode node1111 = new BiDiBNode { Address = new byte[] { 1, 1, 1, 1 } };

            // Act
            int address0 = node0.GetAddress();
            int address1 = node1.GetAddress();
            int address2 = node2.GetAddress();
            int address11 = node11.GetAddress();
            int address13 = node13.GetAddress();
            int address111 = node111.GetAddress();
            int address1111 = node1111.GetAddress();

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
            BiDiBNode node0 = new BiDiBNode { Address = new byte[] { 0 } };

            // Act
            int address0 = node0.GetParentAddress();

            // Assert
            address0.Should().Be(0);
        }

        [TestMethod]
        public void GetParentAddress_ShouldReturn0WhenNodeIsChildOfRoot()
        {
            // Arrange
            BiDiBNode node0 = new BiDiBNode { Address = new byte[] { 5 } };

            // Act
            int address0 = node0.GetParentAddress();

            // Assert
            address0.Should().Be(0);
        }

        [TestMethod]
        public void GetParentAddress_ShouldReturn1WhenNodeIsChildOfSubInterface()
        {
            // Arrange
            BiDiBNode node0 = new BiDiBNode { Address = new byte[] { 1, 1 } };

            // Act
            int address0 = node0.GetParentAddress();

            // Assert
            address0.Should().Be(1);
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenParentIsRoot()
        {
            // Arrange
            BiDiBNode root = new BiDiBNode { Address = new byte[] { 0 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1 } };

            // Act & Assert
            sub.IsSubNodeOf(root).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeIsRoot()
        {
            // Arrange
            BiDiBNode root = new BiDiBNode { Address = new byte[] { 0 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1 } };

            // Act & Assert
            root.IsSubNodeOf(sub).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeSecondLevelOfFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeThirdLevelOfFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeThirdLevelOfSecondLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfSecondLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnTrue_WhenNodeFourthLevelOfThirdLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeTrue();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 2, 1, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherSecondLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 2, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeFourthLevelOfOtherThirdLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 2, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameSecondLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 2 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameThirdLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 2 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeOfSameFourthLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1, 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 1, 1, 2 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeSecondLevelOfOtherFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 2, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeThirdLevelOfOtherFirstLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 2, 1, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }

        [TestMethod]
        public void IsSubNodeOf_ShouldReturnFalse_WhenNodeThirdLevelOfOtherSecondLevel()
        {
            // Arrange
            BiDiBNode parent = new BiDiBNode { Address = new byte[] { 1, 1 } };
            BiDiBNode sub = new BiDiBNode { Address = new byte[] { 1, 2, 1 } };

            // Act & Assert
            sub.IsSubNodeOf(parent).Should().BeFalse();
        }
    }
}