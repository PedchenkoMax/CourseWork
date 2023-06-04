using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.BrandRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IBrandRepository brandRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        brandRepository = new BrandRepository(new NullLogger<BrandRepository>(), context);
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