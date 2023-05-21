using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.BrandRepository;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        brandRepository = new Infrastructure.Database.Repositories.BrandRepository(context);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenBrandIsAdded()
    {
        // Arrange
        var brandToAdd = BrandTestHelper.Default();

        // Act
        var isAdded = await brandRepository.AddAsync(brandToAdd);
        var retrievedBrand = await brandRepository.GetByIdAsync(brandToAdd.Id);

        // Assert
        isAdded.Should().BeTrue();
        retrievedBrand.Should().NotBeNull();
        retrievedBrand.Should().BeEquivalentTo(brandToAdd);
    }
}