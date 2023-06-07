using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.BrandRepositoryTests;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        brandRepository = new BrandRepository(new NullLogger<BrandRepository>(), context);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnTrue_WhenBrandExistsAndIsRemoved()
    {
        // Arrange
        var brandTeRemove = BrandTestHelper.Default();
        await brandRepository.AddAsync(brandTeRemove);

        // Act
        var isRemoved = await brandRepository.RemoveByIdAsync(brandTeRemove.Id);
        var retrievedBrand = await brandRepository.GetByIdAsync(brandTeRemove.Id);

        // Assert
        isRemoved.Should().BeTrue();
        retrievedBrand.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
    {
        // Arrange
        var nonExistentBrandId = Guid.Empty;

        // Act
        var result = await brandRepository.RemoveByIdAsync(nonExistentBrandId);

        // Assert
        result.Should().BeFalse();
    }
}