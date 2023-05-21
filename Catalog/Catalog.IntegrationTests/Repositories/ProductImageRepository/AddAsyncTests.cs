using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenProductImageIsAdded()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImageToAdd = ProductImageTestHelper.Default(productEntity);

        // Act
        var isAdded = await productImageRepository.AddAsync(productImageToAdd);
        var retrievedProductImage = await productImageRepository.GetByIdAsync(productImageToAdd.Id);

        // Assert
        isAdded.Should().BeTrue();
        retrievedProductImage.Should().NotBeNull();
        retrievedProductImage.Should().BeEquivalentTo(productImageToAdd);
    }
}