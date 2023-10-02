using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.Net.Core.Models.VendorCv;
using org.bidib.Net.Core.Models.VendorCv.Extensions;
using org.bidib.Net.Testing;

namespace org.bidib.Net.Core.Test.Models;

[TestClass, TestCategory(TestCategory.UnitTest)]
public class VendorCvExtensionTests : TestClass
{
    [TestMethod]
    public void FindNode_ShouldReturnMatchingNode()
    {
        // Arrange

        var level2Node = new CvNode
        {
            Description = new[] { new CvDescription { Text = "Level2" } },

        };

        var vendorCv = new VendorCv
        {
            CvDefinition = new[]
            {
                new CvNode
                {
                    Description = new[] { new CvDescription { Text = "Level1" } },
                    Nodes = new[]
                    {
                        level2Node
                    }

                }
            }
        };

        // Act
        var result = vendorCv.FindNode("Level1/Level2");

        // Assert
        result.Should().Be(level2Node);
    }

    [TestMethod]
    public void FindCv_ShouldReturnMatchingCv()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1" };
        var cv2 = new Cv { Keyword = "C2" };

        var vendorCv = new VendorCv
        {
            Cvs = new []{ cv1, cv2 }
        };

        // Act
        var result = vendorCv.FindCv("C2");

        // Assert
        result.Should().Be(cv2);
    }   
    
    [TestMethod]
    public void FindCvs_ShouldReturnMatchingCvs()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1" };
        var cv2 = new Cv { Keyword = "C2" };
        var cv3 = new Cv { Keyword = "C2" };

        var vendorCv = new VendorCv
        {
            Cvs = new []{ cv1, cv2, cv3 }
        };

        // Act
        var result = vendorCv.FindCvs("C2").ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(new[] { cv2, cv3 });
    }    
    
    [TestMethod]
    public void GetCvsWithChanges_ShouldReturnMatchingCvs()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "A", NewValue = "A"};
        var cv2 = new Cv { Keyword = "C2", Value = "A", NewValue = "B" };
        var cv3 = new Cv { Keyword = "C3", Value = "A", NewValue = "C" };

        var vendorCv = new VendorCv
        {
            Cvs = new []{ cv1, cv2, cv3 }
        };

        // Act
        var result = vendorCv.GetCvsWithChanges().ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(new[] { cv2, cv3 });
    } 
    
    [TestMethod]
    public void SetValue_ShouldSetNewValueOnMatchingCv_byte()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "A", NewValue = "A"};
        var cv2 = new Cv { Keyword = "C2", Value = "A", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new []{ cv1, cv2 }
        };

        // Act
        vendorCv.SetValue(10, "C1");

        // Assert
        cv1.NewValue.Should().Be("10");
    }

    [TestMethod]
    public void SetValue_ShouldSetNewValueOnMatchingCv_ushort()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "A", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C2", Value = "A", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2 }
        };

        // Act
        vendorCv.SetValue((ushort)10, "C1");

        // Assert
        cv1.NewValue.Should().Be("10");
        cv2.NewValue.Should().Be("0");
    }

    [TestMethod]
    public void SetValue_ShouldSetNewValueOnMatchingCv_uint()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "A", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C1", Value = "A", NewValue = "B" };
        var cv3 = new Cv { Keyword = "C1", Value = "A", NewValue = "C" };
        var cv4 = new Cv { Keyword = "C1", Value = "A", NewValue = "D" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2, cv3, cv4 }
        };

        // Act
        vendorCv.SetValue((uint)10, "C1");

        // Assert
        cv1.NewValue.Should().Be("10");
        cv2.NewValue.Should().Be("0");
    }

    [TestMethod]
    public void SetValue_ShouldReturnValue()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "1", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C2", Value = "1", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2 }
        };

        // Act
        vendorCv.SetValue((ushort)2565, "C1", "C2");

        // Assert
        cv1.NewValue.Should().Be("5");
        cv2.NewValue.Should().Be("10");
    }

    [TestMethod]
    public void GetByteValue_ShouldReturnValue()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "5", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C2", Value = "10", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2 }
        };

        // Act
        var result = vendorCv.GetByteValue( "C1");

        // Assert
        result.Should().Be(5);
    }

    [TestMethod]
    public void GetUShortValue_ShouldReturnValue()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "5", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C1", Value = "10", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2 }
        };

        // Act
        var result = vendorCv.GetUShortValue("C1");

        // Assert
        result.Should().Be(2565);
    }

    [TestMethod]
    public void GetUShortValue_ShouldReturnValue_HighLow()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "5", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C2", Value = "10", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2 }
        };

        // Act
        var result = vendorCv.GetUShortValue("C1", "C2");

        // Assert
        result.Should().Be(2565);
    }

    [TestMethod]
    public void GetUIntValue_ShouldReturnValue()
    {
        // Arrange
        var cv1 = new Cv { Keyword = "C1", Value = "1", NewValue = "A" };
        var cv2 = new Cv { Keyword = "C1", Value = "1", NewValue = "B" };
        var cv3 = new Cv { Keyword = "C1", Value = "1", NewValue = "B" };
        var cv4 = new Cv { Keyword = "C1", Value = "1", NewValue = "B" };

        var vendorCv = new VendorCv
        {
            Cvs = new[] { cv1, cv2, cv3, cv4 }
        };

        // Act
        var result = vendorCv.GetUIntValue("C1");

        // Assert
        result.Should().Be(16_843_009);
    }


}