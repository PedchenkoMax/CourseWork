using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductImage_WhenProductImageExists()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImageToGet = ProductImageTestHelper.Default(productEntity);
        await productImageRepository.AddAsync(productImageToGet);

        // Act
        var retrievedProductImage = await productImageRepository.GetByIdAsync(productImageToGet.Id);

        // Assert
        retrievedProductImage.Should().NotBeNull();
        retrievedProductImage.Should().BeEquivalentTo(productImageToGet);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductImageDoesNotExist()
    {
        // Act
        var retrievedProductImage = await productImageRepository.GetByIdAsync(Guid.Empty);

        // Assert
        retrievedProductImage.Should().BeNull();
    }
}