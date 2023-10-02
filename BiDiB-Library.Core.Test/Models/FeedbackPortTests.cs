using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.BiDiB;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class FeedbackPortTests : TestClass<FeedbackPort>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new FeedbackPort();
        }

        [TestMethod]
        public void ClearOccupancies_ShouldRemoveOccupancies()
        {
            // Arrange
            Target.AddOccupancy(new OccupancyInfo { Address = 1 });

            // Act
            Target.ClearOccupancies();

            // Assert
            Target.Occupancies.Should().BeNull();
        }

        [TestMethod]
        public void ClearOccupancies_ShouldRaiseIsOccupiedChanged()
        {
            // Arrange
            Target.AddOccupancy(new OccupancyInfo { Address = 1 });
            bool occupiedUpdated = false;
            Target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(FeedbackPort.IsOccupied))
                {
                    occupiedUpdated = true;
                }
            };

            // Act
            Target.ClearOccupancies();

            // Assert
            Target.IsOccupied.Should().BeFalse();
            occupiedUpdated.Should().BeTrue();
        }

        [TestMethod]
        public void AddOccupancy_ShouldAddItem()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };

            // Act
            Target.AddOccupancy(occupancy);

            // Assert
            Target.Occupancies.Should().HaveCount(1);
            Target.Occupancies[0].Should().Be(occupancy);
        }

        [TestMethod]
        public void AddOccupancy_ShouldRaiseIsOccupiedChanged()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };
            bool occupiedUpdated = false;
            Target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(FeedbackPort.IsOccupied))
                {
                    occupiedUpdated = true;
                }
            };

            // Act
            Target.AddOccupancy(occupancy);

            // Assert
            Target.IsOccupied.Should().BeTrue();
            occupiedUpdated.Should().BeTrue();
        } 
        
        [TestMethod]
        public void RemoveOccupancy_ShouldRemoveItem()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };
            Target.AddOccupancy(occupancy);

            // Act
            Target.RemoveOccupancy(occupancy);

            // Assert
            Target.Occupancies.Should().HaveCount(0);
        }

        [TestMethod]
        public void RemoveOccupancy_ShouldRaiseIsOccupiedChanged()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };
            Target.AddOccupancy(occupancy);
            bool occupiedUpdated = false;
            Target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(FeedbackPort.IsOccupied))
                {
                    occupiedUpdated = true;
                }
            };

            // Act
            Target.RemoveOccupancy(occupancy);

            // Assert
            Target.IsOccupied.Should().BeFalse();
            occupiedUpdated.Should().BeTrue();
        }

        [TestMethod]
        public void GetOccupancy_ShouldReturnItem()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };
            Target.AddOccupancy(occupancy);

            // Act
            var result =Target.GetOccupancy(1);

            // Assert
            result.Should().Be(occupancy);
        }

        [TestMethod]
        public void GetOccupanciesByFilter_ShouldReturnMatchingItems()
        {
            // Arrange
            var occupancy = new OccupancyInfo { Address = 1 };
            Target.AddOccupancy(occupancy);
            Target.AddOccupancy(new OccupancyInfo {Address = 2});

            // Act
            var result =Target.GetOccupanciesByFilter(x => x.Address == 1);

            // Assert
            result.Should().HaveCount(1);
            result.ElementAt(0).Should().Be(occupancy);
        }
    }
}
