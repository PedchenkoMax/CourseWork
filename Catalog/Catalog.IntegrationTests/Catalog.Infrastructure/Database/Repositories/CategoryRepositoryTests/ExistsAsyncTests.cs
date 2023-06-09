﻿using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.IntegrationTests.TestUtils;
using Catalog.IntegrationTests.TestUtils.SeedData;
using FluentAssertions;
using Xunit;

namespace Catalog.IntegrationTests.Catalog.Infrastructure.Database.Repositories.CategoryRepositoryTests;

public class ExistsAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly ICategoryRepository categoryRepository;

    public ExistsAsyncTests(DatabaseFixture fixture)
    {
        var context = new DapperDbContext(fixture.ConnectionString);
        categoryRepository = new CategoryRepository(new NullLogger<CategoryRepository>(), context);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenCategoryExists()
    {
        // Arrange
        var categoryToCheck = CategoryTestHelper.Default();
        await categoryRepository.AddAsync(categoryToCheck);

        // Act
        var isExist = await categoryRepository.ExistsAsync(categoryToCheck.Id);

        // Assert
        isExist.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentCategoryId = Guid.Empty;

        // Act
        var isExist = await categoryRepository.ExistsAsync(nonExistentCategoryId);

        // Assert
        isExist.Should().BeFalse();
    }
}