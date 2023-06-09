﻿using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.CategoryRepositoryTests;

public class GetSubcategoriesByParentCategoryIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public GetSubcategoriesByParentCategoryIdAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        categoryRepository = new CategoryRepository(new NullLogger<CategoryRepository>(), context);
    }

    [Fact]
    public async Task GetSubcategoriesByParentCategoryIdAsync_ShouldReturnChildrenCategories_WhenChildrenExist()
    {
        // Arrange
        var parentCategory = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(parentCategory);

        var childCategory1 = CategoryTestHelper.ChildOf(parentCategory);
        var childCategory2 = CategoryTestHelper.ChildOf(parentCategory);

        await categoryRepository.AddAsync(childCategory1);
        await categoryRepository.AddAsync(childCategory2);

        // Act
        var childrenCategories = await categoryRepository.GetSubcategoriesByParentCategoryIdAsync(parentCategory.Id);

        // Assert
        childrenCategories.Should().NotBeNull();
        childrenCategories.Should().HaveCount(2);
        childrenCategories.Should().Contain(category => category.Id == childCategory1.Id);
        childrenCategories.Should().Contain(category => category.Id == childCategory2.Id);
    }

    [Fact]
    public async Task GetSubcategoriesByParentCategoryIdAsync_ShouldReturnEmptyList_WhenNoChildrenExist()
    {
        // Arrange
        var testCategory = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(testCategory);

        // Act
        var childrenCategories = await categoryRepository.GetSubcategoriesByParentCategoryIdAsync(testCategory.Id);

        // Assert
        childrenCategories.Should().BeEmpty();
    }
}