using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductRepository;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productToGet = ProductTestHelper.Default();
        await productRepository.AddAsync(productToGet);

        // Act
        var retrievedProduct = await productRepository.GetByIdAsync(productToGet.Id);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Should().BeEquivalentTo(productToGet);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Act
        var retrievedProduct = await productRepository.GetByIdAsync(Guid.Empty);

        // Assert
        retrievedProduct.Should().BeNull();
    }
}