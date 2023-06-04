using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.ProductRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly IProductRepository productRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        productRepository = new ProductRepository(new NullLogger<ProductRepository>(), context);
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