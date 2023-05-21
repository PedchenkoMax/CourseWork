using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Repositories.CategoryRepository;

public class GetChildrenByParentCategoryIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public GetChildrenByParentCategoryIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DbContext(fixture.ConnectionString);
        categoryRepository = new Infrastructure.Database.Repositories.CategoryRepository(context);
    }

    [Fact]
    public async Task GetChildrenByParentCategoryId_ShouldReturnChildrenCategories_WhenChildrenExist()
    {
        // Arrange
        var parentCategory = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(parentCategory);

        var childCategory1 = CategoryTestHelper.ChildOf(parentCategory);
        var childCategory2 = CategoryTestHelper.ChildOf(parentCategory);

        await categoryRepository.AddAsync(childCategory1);
        await categoryRepository.AddAsync(childCategory2);

        // Act
        var childrenCategories = await categoryRepository.GetChildrenByParentCategoryId(parentCategory.Id);

        // Assert
        childrenCategories.Should().NotBeNull();
        childrenCategories.Should().HaveCount(2);
        childrenCategories.Should().Contain(category => category.Id == childCategory1.Id);
        childrenCategories.Should().Contain(category => category.Id == childCategory2.Id);
    }

    [Fact]
    public async Task GetChildrenByParentCategoryId_ShouldReturnEmptyList_WhenNoChildrenExist()
    {
        // Arrange
        var testCategory = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(testCategory);

        // Act
        var childrenCategories = await categoryRepository.GetChildrenByParentCategoryId(testCategory.Id);

        // Assert
        childrenCategories.Should().BeEmpty();
    }
}