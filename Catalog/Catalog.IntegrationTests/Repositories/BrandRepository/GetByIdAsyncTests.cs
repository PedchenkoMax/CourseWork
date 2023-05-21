using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.BrandRepository;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        brandRepository = new Infrastructure.Database.Repositories.BrandRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBrand_WhenBrandExists()
    {
        // Arrange
        var brandToGet = BrandTestHelper.Default();
        await brandRepository.AddAsync(brandToGet);

        // Act
        var retrievedBrand = await brandRepository.GetByIdAsync(brandToGet.Id);

        // Assert
        retrievedBrand.Should().NotBeNull();
        retrievedBrand.Should().BeEquivalentTo(brandToGet);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
    {
        // Arrange
        var nonExistentBrandId = Guid.Empty;
        
        // Act
        var retrievedBrand = await brandRepository.GetByIdAsync(nonExistentBrandId);

        // Assert
        retrievedBrand.Should().BeNull();
    }
}