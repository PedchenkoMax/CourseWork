using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.BrandRepositoryTests;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        brandRepository = new BrandRepository(context);
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