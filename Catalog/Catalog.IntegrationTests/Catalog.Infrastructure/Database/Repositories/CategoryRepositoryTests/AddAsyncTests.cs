using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.CategoryRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        categoryRepository = new CategoryRepository(context);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTrue_WhenCategoryIsAdded()
    {
        // Arrange
        var categoryToAdd = CategoryTestHelper.Default();

        // Act
        var isAdded = await categoryRepository.AddAsync(categoryToAdd);
        var retrievedCategory = await categoryRepository.GetByIdAsync(categoryToAdd.Id);

        // Assert
        isAdded.Should().BeTrue();
        retrievedCategory.Should().NotBeNull();
        retrievedCategory.Should().BeEquivalentTo(categoryToAdd);
    }
}