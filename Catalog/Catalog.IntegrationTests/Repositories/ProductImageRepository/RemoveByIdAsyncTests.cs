using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnTrue_WhenProductImageExistsAndIsRemoved()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImageToRemove = ProductImageTestHelper.Default(productEntity);
        await productImageRepository.AddAsync(productImageToRemove);

        // Act
        var isRemoved = await productImageRepository.RemoveByIdAsync(productImageToRemove.Id);
        var retrievedProductImage = await productImageRepository.GetByIdAsync(productImageToRemove.Id);

        // Assert
        isRemoved.Should().BeTrue();
        retrievedProductImage.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnFalse_WhenProductImageDoesNotExist()
    {
        // Arrange
        var nonExistentProductImageId = Guid.Empty;

        // Act
        var result = await productImageRepository.RemoveByIdAsync(nonExistentProductImageId);

        // Assert
        result.Should().BeFalse();
    }
}