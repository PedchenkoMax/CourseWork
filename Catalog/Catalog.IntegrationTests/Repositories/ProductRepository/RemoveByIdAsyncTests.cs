using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductRepository;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnTrue_WhenProductExistsAndIsRemoved()
    {
        // Arrange
        var productToRemove = ProductTestHelper.Default();
        await productRepository.AddAsync(productToRemove);

        // Act
        var isRemoved = await productRepository.RemoveByIdAsync(productToRemove.Id);
        var retrievedProduct = await productRepository.GetByIdAsync(productToRemove.Id);

        // Assert
        isRemoved.Should().BeTrue();
        retrievedProduct.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.Empty;

        // Act
        var result = await productRepository.RemoveByIdAsync(nonExistentProductId);

        // Assert
        result.Should().BeFalse();
    }
}