using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.CategoryRepositoryTests;

public class RemoveByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public RemoveByIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new CategoryRepository(context);
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