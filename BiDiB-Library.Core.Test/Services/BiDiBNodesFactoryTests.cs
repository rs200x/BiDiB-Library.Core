using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models;
using org.bidib.netbidibc.core.Services;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class BiDiBNodesFactoryTests : TestClass<BiDiBNodesFactory>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new BiDiBNodesFactory(NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void RemoveNode_ShouldRemoveAllNodes_WhenRoot()
        {
            // Arrange
            GenerateSamples();

            // Act
            Target.RemoveNode(new byte[] { 0 });

            // Assert
            Target.Nodes.Should().HaveCount(0);
        }

        [TestMethod]
        public void RemoveNode_ShouldRemoveNode_WhenNoChildren()
        {
            // Arrange
            GenerateSamples();

            // Act
            Target.RemoveNode(new byte[] { 2 });

            // Assert
            Target.Nodes.Should().HaveCount(8);
            var nodes = Target.Nodes.ToList();
            nodes.Should().Contain(x=>x.FullAddress == "0.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.1.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.2.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.2.1.0");
            nodes.Should().Contain(x => x.FullAddress == "3.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "3.1.0.0");
            nodes.Should().Contain(x => x.FullAddress == "3.2.0.0");
            
        }

        [TestMethod]
        public void RemoveNode_ShouldRemoveNodes_WhenFirstLevelWithChildren()
        {
            // Arrange
            GenerateSamples();

            // Act
            Target.RemoveNode(new byte[] { 3 });

            // Assert
            Target.Nodes.Should().HaveCount(6);
            var nodes = Target.Nodes.ToList();
            nodes.Should().Contain(x => x.FullAddress == "0.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.1.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.2.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.2.1.0");
            nodes.Should().Contain(x => x.FullAddress == "2.0.0.0");
        }

        [TestMethod]
        public void RemoveNode_ShouldRemoveNodes_WhenSubLevelWithChildren()
        {
            // Arrange
            GenerateSamples();

            // Act
            Target.RemoveNode(new byte[] { 1,2 });

            // Assert
            Target.Nodes.Should().HaveCount(7);
            var nodes = Target.Nodes.ToList();
            nodes.Should().Contain(x => x.FullAddress == "0.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "1.1.0.0");
            nodes.Should().Contain(x => x.FullAddress == "2.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "3.0.0.0");
            nodes.Should().Contain(x => x.FullAddress == "3.1.0.0");
            nodes.Should().Contain(x => x.FullAddress == "3.2.0.0");
        }

        [TestMethod]
        public void RemoveNode_ShouldInvokeNodeRemoved()
        {
            // Arrange

            BiDiBNode newNode = Target.CreateNode(new byte[] { 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 });
            BiDiBNode removedNode = null;
            Target.NodeRemoved = delegate (BiDiBNode node) { removedNode = node; };

            // Act
            Target.RemoveNode(new byte[] { 1 });

            // Assert
            removedNode.Should().NotBe(null);
            removedNode.Should().Be(newNode);
        }

        [TestMethod]
        public void Reset_ShouldRemoveAllNodes()
        {
            // Arrange
            Target.CreateNode(new byte[] { 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 });
            Target.CreateNode(new byte[] { 1, 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 2 });

            // Act
            Target.Reset();

            // Assert
            Target.Nodes.Should().HaveCount(0);
        }

        private void GenerateSamples()
        {
            Target.CreateNode(new byte[] { 0 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 });
            Target.CreateNode(new byte[] { 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 2 });
            Target.CreateNode(new byte[] { 1, 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 3 });
            Target.CreateNode(new byte[] { 1, 2 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 4 });
            Target.CreateNode(new byte[] { 1, 2, 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 5 });
            Target.CreateNode(new byte[] { 2 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 6 });
            Target.CreateNode(new byte[] { 3 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 7 });
            Target.CreateNode(new byte[] { 3, 1 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 8 });
            Target.CreateNode(new byte[] { 3, 2 }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 9 });
        }
    }
}