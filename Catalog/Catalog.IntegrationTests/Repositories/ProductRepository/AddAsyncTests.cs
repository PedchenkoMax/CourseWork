using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.ProductRepository;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        productRepository = new Infrastructure.Database.Repositories.ProductRepository(context);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenProductIsAdded()
    {
        // Arrange
        var productToAdd = ProductTestHelper.Default();

        // Act
        var isAdded = await productRepository.AddAsync(productToAdd);
        var retrievedProduct = await productRepository.GetByIdAsync(productToAdd.Id);

        // Assert
        isAdded.Should().BeTrue();
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Should().BeEquivalentTo(productToAdd);
    }
}