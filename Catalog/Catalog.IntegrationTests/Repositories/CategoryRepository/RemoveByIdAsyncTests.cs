using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.CategoryRepository;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new Infrastructure.Database.Repositories.CategoryRepository(context);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnTrue_WhenCategoryExistsAndIsRemoved()
    {
        // Arrange
        var categoryToRemove = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(categoryToRemove);

        // Act
        var isRemoved = await categoryRepository.RemoveByIdAsync(categoryToRemove.Id);
        var retrievedCategory = await categoryRepository.GetByIdAsync(categoryToRemove.Id);

        // Assert
        isRemoved.Should().BeTrue();
        retrievedCategory.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentCategoryId = Guid.Empty;

        // Act
        var isRemoved = await categoryRepository.RemoveByIdAsync(nonExistentCategoryId);

        // Assert
        isRemoved.Should().BeFalse();
    }
}