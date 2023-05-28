using Catalog.Api.ValidationAttributes;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Api.ValidationAttributes;

public class NonZeroGuidAttributeTests
{
    private readonly NonZeroGuidAttribute attribute = new();

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenValueIsNonEmptyGuid()
    {
        var isValid = attribute.IsValid(Guid.NewGuid());
        isValid.Should().Be(true);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenValueIsNull()
    {
        var isValid = attribute.IsValid(null);
        isValid.Should().Be(false);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenValueIsEmptyGuid()
    {
        var isValid = attribute.IsValid(Guid.Empty);
        isValid.Should().Be(false);
    }
}