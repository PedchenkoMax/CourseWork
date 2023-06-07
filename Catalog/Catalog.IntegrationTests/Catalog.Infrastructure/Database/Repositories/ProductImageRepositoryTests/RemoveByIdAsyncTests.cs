using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.ProductImageRepositoryTests;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        productImageRepository = new ProductImageRepository(new NullLogger<ProductImageRepository>(), context);
        productRepository = new ProductRepository(new NullLogger<ProductRepository>(), context);
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