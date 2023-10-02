using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Services;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Services
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    [DeploymentItem(SampleFile, "TestData")]
    public class JsonServiceTests : TestClass<JsonService>
    {
        private const string SampleFile = "TestData/TestObjects.json";

        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new JsonService(NullLoggerFactory.Instance);
        }

        [TestMethod]
        public void LoadFromFile_ShouldDeserializeFile()
        {
            // Arrange

            // Act
            var result = Target.LoadFromFile<TestObject[]>(SampleFile);

            // Assert
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("A");
            result[0].Number.Should().Be(1);
        } 
        
        [TestMethod]
        public void SaveToFile_ShouldSerializeObject()
        {
            // Arrange
            var sampleData = new[]
            {
                new TestObject { Name = "A", Number = 1 }, 
                new TestObject { Name = "B", Number = 2 }
            };

            var fileName = "TestData/WriteFile.json";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Act
            var result = Target.SaveToFile(sampleData, "TestData/WriteFile.json");

            // Assert
            result.Should().BeTrue();
            File.Exists(fileName).Should().BeTrue();
            var lines = File.ReadAllLines(fileName);
            lines.Should().HaveCount(10);
        }

 [TestMethod]
        public void SaveToFile_ShouldReturnFalse_whenError()
        {
            // Arrange
            var sampleData = new[]
            {
                new TestObject { Name = "A", Number = 1 }, 
                new TestObject { Name = "B", Number = 2 }
            };


            // Act
            var result = Target.SaveToFile(sampleData, "");

            // Assert
            result.Should().BeFalse();
        }



        private class TestObject
        {
            public string Name { get; set; }

            public int Number { get; set; }
        }
    }
}