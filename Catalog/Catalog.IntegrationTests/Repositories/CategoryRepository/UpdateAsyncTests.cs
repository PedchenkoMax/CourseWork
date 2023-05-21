using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.CategoryRepository;

public class UpdateAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public UpdateAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new Infrastructure.Database.Repositories.CategoryRepository(context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenCategoryExistsAndIsUpdated()
    {
        // Arrange
        var categoryToUpdate = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(categoryToUpdate);

        categoryToUpdate.UpdateTo(CategoryTestHelper.Modified());

        // Act
        var isUpdated = await categoryRepository.UpdateAsync(categoryToUpdate);
        var retrievedCategory = await categoryRepository.GetByIdAsync(categoryToUpdate.Id);

        // Assert
        isUpdated.Should().BeTrue();
        retrievedCategory.Should().NotBeNull();
        retrievedCategory.Should().BeEquivalentTo(categoryToUpdate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentCategory = CategoryTestHelper.Default();

        // Act
        var isUpdated = await categoryRepository.UpdateAsync(nonExistentCategory);

        // Assert
        isUpdated.Should().BeFalse();
    }
}