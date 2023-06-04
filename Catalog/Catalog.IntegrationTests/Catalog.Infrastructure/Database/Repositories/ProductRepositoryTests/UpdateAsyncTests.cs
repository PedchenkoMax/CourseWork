using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.ProductRepositoryTests;

public class UpdateAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public UpdateAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        productRepository = new ProductRepository(new NullLogger<ProductRepository>(), context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenProductExistsAndIsUpdated()
    {
        // Arrange
        var productToUpdate = ProductTestHelper.Default();
        await productRepository.AddAsync(productToUpdate);

        productToUpdate.UpdateTo(ProductTestHelper.Modified());

        // Act
        var isUpdated = await productRepository.UpdateAsync(productToUpdate);
        var retrievedProduct = await productRepository.GetByIdAsync(productToUpdate.Id);

        // Assert
        isUpdated.Should().BeTrue();
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Should().BeEquivalentTo(productToUpdate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProduct = ProductTestHelper.Default();

        // Act
        var isUpdated = await productRepository.UpdateAsync(nonExistentProduct);

        // Assert
        isUpdated.Should().BeFalse();
    }
}