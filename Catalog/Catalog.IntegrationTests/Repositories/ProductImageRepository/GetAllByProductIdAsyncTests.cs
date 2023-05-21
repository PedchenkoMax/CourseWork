using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class GetAllByProductIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public GetAllByProductIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task GetAllByProductIdAsync_ShouldReturnListOfProductImages_WhenProductIdExists()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImage1 = ProductImageTestHelper.Default(productEntity);
        var productImage2 = ProductImageTestHelper.Default(productEntity);
        await productImageRepository.AddAsync(productImage1);
        await productImageRepository.AddAsync(productImage2);

        // Act
        var retrievedProductImages = await productImageRepository.GetAllByProductIdAsync(productEntity.Id);

        // Assert
        retrievedProductImages.Should().NotBeEmpty();
        retrievedProductImages.Should().HaveCount(2);
        retrievedProductImages.Should().ContainEquivalentOf(productImage1);
        retrievedProductImages.Should().ContainEquivalentOf(productImage2);
    }

    [Fact]
    public async Task GetAllByProductIdAsync_ShouldReturnEmptyList_WhenProductIdDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.Empty;

        // Act
        var retrievedProductImages = await productImageRepository.GetAllByProductIdAsync(nonExistentProductId);

        // Assert
        retrievedProductImages.Should().BeEmpty();
    }
}