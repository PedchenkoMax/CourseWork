using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.CategoryRepository;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new Infrastructure.Database.Repositories.CategoryRepository(context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryToGet = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(categoryToGet);

        // Act
        var retrievedCategory = await categoryRepository.GetByIdAsync(categoryToGet.Id);

        // Assert
        retrievedCategory.Should().NotBeNull();
        retrievedCategory.Should().BeEquivalentTo(categoryToGet);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Act
        var retrievedCategory = await categoryRepository.GetByIdAsync(Guid.Empty);

        // Assert
        retrievedCategory.Should().BeNull();
    }
}