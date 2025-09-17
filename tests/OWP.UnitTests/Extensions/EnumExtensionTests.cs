using FluentAssertions;
using OWP.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace OWP.UnitTests.Extensions;

public class EnumExtensionTests
{
    public enum TestEnum
    {
        [Display(Name = "First Value")]
        First,

        [Display(Name = "Second Value")]
        Second,

        Third
    }

    [Fact]
    public void GetAttribute_WhenAttributeExists_ShouldReturnAttribute()
    {
        // Arrange
        var enumValue = TestEnum.First;

        // Act
        var result = enumValue.GetAttribute<DisplayAttribute>();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("First Value");
    }

    [Fact]
    public void GetAttribute_WhenAttributeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var enumValue = TestEnum.Third;

        // Act
        var result = enumValue.GetAttribute<DisplayAttribute>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetAttribute_WhenMultipleAttributesExist_ShouldReturnFirst()
    {
        // Arrange
        var enumValue = TestEnum.Second;

        // Act
        var result = enumValue.GetAttribute<DisplayAttribute>();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Second Value");
    }
}
