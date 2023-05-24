using Catalog.Api.ValidationAttributes;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Api.ValidationAttributes;

public class NonZeroNullableGuidAttributeTests
{
    private readonly NonZeroNullableGuidAttribute attribute = new();

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenValueIsNonEmptyGuid()
    {
        var isValid = attribute.IsValid(Guid.NewGuid());
        isValid.Should().Be(true);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenValueIsNull()
    {
        var isValid = attribute.IsValid(null);
        isValid.Should().Be(true);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenValueIsEmptyGuid()
    {
        var isValid = attribute.IsValid(Guid.Empty);
        isValid.Should().Be(false);
    }
}