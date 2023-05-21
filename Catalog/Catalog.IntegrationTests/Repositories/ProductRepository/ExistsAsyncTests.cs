using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductRepository;

public class ExistsAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public ExistsAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        var productToCheck = ProductTestHelper.Default();
        await productRepository.AddAsync(productToCheck);

        // Act
        var isExist = await productRepository.ExistsAsync(productToCheck.Id);

        // Assert
        isExist.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.Empty;

        // Act
        var isExist = await productRepository.ExistsAsync(nonExistentProductId);

        // Assert
        isExist.Should().BeFalse();
    }
}