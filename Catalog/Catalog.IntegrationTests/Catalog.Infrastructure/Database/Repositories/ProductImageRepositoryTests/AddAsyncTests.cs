using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.ProductImageRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        productImageRepository = new ProductImageRepository(context);
        productRepository = new ProductRepository(context);
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