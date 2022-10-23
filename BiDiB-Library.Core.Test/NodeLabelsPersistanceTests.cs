using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Models.BiDiB.Base;
using org.bidib.netbidibc.core.Models.BiDiB.Labels;
using org.bidib.netbidibc.Testing;

namespace org.bidib.netbidibc.core.Test
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem("data/Schema/BiDiB/" + Bidib2BaseTypesXsd)]
    [DeploymentItem("data/Schema/BiDiB/" + Bidib2CommonXsd)]
    [DeploymentItem("data/Schema/BiDiB/" + Bidib2LabelsXsd)]
    public class NodeLabelsPersistenceTests : TestClass
    {
        private const string Bidib2LabelsXsd = "bidib2Labels.xsd";
        private const string Bidib2BaseTypesXsd = "bidib2BaseTypes.xsd";
        private const string Bidib2CommonXsd = "bidib2Common.xsd";

        [TestMethod]
        public void NodeLabels_ShouldBeSerializable()
        {
            // Arrange
            NodeLabels nodeLabels = new NodeLabels
            {
                DefaultLabelsApplied = DefaultLabelsAction.Applied,
                NodeLabel = new NodeLabel {UniqueId = 12345, UserName = "TestNode", ProductName = "IF2"},
                PortLabels = new[]
                {
                    new PortLabel {Index = 0, Label = "PL1", Type = PortType.AnalogOut},
                    new PortLabel {Index = 1, Label = "PL2", Type = PortType.Backlight},
                    new PortLabel {Index = 2, Label = "PL3", Type = PortType.Input},
                    new PortLabel {Index = 3, Label = "PL4", Type = PortType.Servo},
                    new PortLabel {Index = 4, Label = "PL5", Type = PortType.Sound},
                    new PortLabel {Index = 5, Label = "PL6", Type = PortType.Switchpair},
                    new PortLabel {Index = 6, Label = "PL7", Type = PortType.Switch},
                },
                FeedbackPortLabels = new[]
                {
                    new BaseLabel {Index = 0, Label = "FPL1"},
                    new BaseLabel {Index = 1, Label = "FPL2"},
                    new BaseLabel {Index = 2, Label = "FPL3"}
                },
                MacroLabels = new[]
                {
                    new BaseLabel {Index = 0, Label = "ML1"},
                    new BaseLabel {Index = 1, Label = "ML2"},
                    new BaseLabel {Index = 2, Label = "ML3"},
                    new BaseLabel {Index = 3, Label = "ML4"}
                },
                AccessoryLabels = new[]
                {
                    new AccessoryLabel
                    {
                        Index = 0,
                        Label = "ACL1",
                        AspectLabels = new[]
                        {
                            new BaseLabel {Index = 0, Label = "APL1"},
                            new BaseLabel {Index = 1, Label = "APL2"},
                            new BaseLabel {Index = 2, Label = "APL3"},
                            new BaseLabel {Index = 3, Label = "APL4"},
                        }
                    },
                    new AccessoryLabel
                    {
                        Index = 0,
                        Label = "ACL2",
                        AspectLabels = new[]
                        {
                            new BaseLabel {Index = 0, Label = "APL5"},
                            new BaseLabel {Index = 1, Label = "APL6"},
                            new BaseLabel {Index = 2, Label = "APL7"},
                            new BaseLabel {Index = 3, Label = "APL8"},
                        }
                    },
                }
            };

            // Act
            SaveToXmlFile(nodeLabels, "LabelsTest.xml");

            // Assert
            NodeLabels readNodeLabels = LoadFromXmlFile<NodeLabels>("LabelsTest.xml");
            readNodeLabels.DefaultLabelsApplied.Should().Be(DefaultLabelsAction.Applied);
            readNodeLabels.Should().NotBeNull();
            readNodeLabels.NodeLabel.Should().NotBeNull();
            readNodeLabels.NodeLabel.UserName.Should().Be("TestNode");
            readNodeLabels.NodeLabel.UniqueId.Should().Be(12345);
            readNodeLabels.AccessoryLabels.Should().HaveCount(2);
            readNodeLabels.AccessoryLabels[0].AspectLabels.Should().HaveCount(4);
            readNodeLabels.FeedbackPortLabels.Should().HaveCount(3);
            readNodeLabels.MacroLabels.Should().HaveCount(4);
            readNodeLabels.PortLabels.Should().HaveCount(7);
        }
    }
}