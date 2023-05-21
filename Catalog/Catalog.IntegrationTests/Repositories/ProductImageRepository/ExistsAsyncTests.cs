using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductImageRepository;

public class ExistsAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;

    public ExistsAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productImageRepository = new Infrastructure.Database.Repositories.ProductImageRepository(context);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenProductImageExists()
    {
        // Arrange
        var productEntity = ProductTestHelper.Default();
        await productRepository.AddAsync(productEntity);
        var productImageToCheck = ProductImageTestHelper.Default(productEntity);
        await productImageRepository.AddAsync(productImageToCheck);

        // Act
        var isExist = await productImageRepository.ExistsAsync(productImageToCheck.Id);

        // Assert
        isExist.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenProductImageDoesNotExist()
    {
        // Arrange
        var nonExistentProductImageId = Guid.Empty;

        // Act
        var isExist = await productImageRepository.ExistsAsync(nonExistentProductImageId);

        // Assert
        isExist.Should().BeFalse();
    }
}