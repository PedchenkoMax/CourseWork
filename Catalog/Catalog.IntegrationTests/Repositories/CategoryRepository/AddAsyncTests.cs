using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.CategoryRepository;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new Infrastructure.Database.Repositories.CategoryRepository(context);
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