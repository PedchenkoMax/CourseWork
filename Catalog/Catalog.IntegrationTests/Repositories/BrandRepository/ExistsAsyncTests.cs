using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.BrandRepository;

public class ExistsAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public ExistsAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        brandRepository = new Infrastructure.Database.Repositories.BrandRepository(context);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenBrandExists()
    {
        // Arrange
        var brandToCheck = BrandTestHelper.Default();
        await brandRepository.AddAsync(brandToCheck);

        // Act
        var isExist = await brandRepository.ExistsAsync(brandToCheck.Id);

        // Assert
        isExist.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
    {
        // Arrange
        var nonExistentBrandId = Guid.Empty;

        // Act
        var isExist = await brandRepository.ExistsAsync(nonExistentBrandId);

        // Assert
        isExist.Should().BeFalse();
    }
}