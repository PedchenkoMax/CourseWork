using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.BrandRepository;

public class UpdateAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public UpdateAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        brandRepository = new Infrastructure.Database.Repositories.BrandRepository(context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenBrandExistsAndIsUpdated()
    {
        // Arrange
        var brandToUpdate = BrandTestHelper.Default();
        await brandRepository.AddAsync(brandToUpdate);

        brandToUpdate.UpdateTo(BrandTestHelper.Modified());

        // Act
        var isUpdated = await brandRepository.UpdateAsync(brandToUpdate);
        var retrievedBrand = await brandRepository.GetByIdAsync(brandToUpdate.Id);

        // Assert
        isUpdated.Should().BeTrue();
        retrievedBrand.Should().NotBeNull();
        retrievedBrand.Should().BeEquivalentTo(brandToUpdate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
    {
        // Arrange
        var nonExistentBrand = BrandTestHelper.Default();

        // Act
        var isUpdated = await brandRepository.UpdateAsync(nonExistentBrand);

        // Assert
        isUpdated.Should().BeFalse();
    }
}