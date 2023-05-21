using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class UpdateAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public UpdateAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenProductImageExistsAndIsUpdated()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImageToUpdate = ProductImageTestHelper.Default(productEntity);
        await productImageRepository.AddAsync(productImageToUpdate);

        productImageToUpdate.UpdateTo(ProductImageTestHelper.Modified(productEntity));

        // Act
        var isUpdated = await productImageRepository.UpdateAsync(productImageToUpdate);
        var retrievedProductImage = await productImageRepository.GetByIdAsync(productImageToUpdate.Id);

        // Assert
        isUpdated.Should().BeTrue();
        retrievedProductImage.Should().NotBeNull();
        retrievedProductImage.Should().BeEquivalentTo(productImageToUpdate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductImageDoesNotExist()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var nonExistentProductImage = ProductImageTestHelper.Default(productEntity);

        // Act
        var isUpdated = await productImageRepository.UpdateAsync(nonExistentProductImage);

        // Assert
        isUpdated.Should().BeFalse();
    }
}