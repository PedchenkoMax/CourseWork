using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Api.Mappers;

public class CategoryMapperTests
{
    // [Fact]
    // public void MapToReadDto_ValidEntity_ReturnsValidDto()
    // {
    //     // Arrange
    //     CategoryEntity.TryCreate
    //     (
    //         parentCategoryId: null,
    //         name: "Test Category",
    //         description: "Test Description",
    //         imageFileName: "",
    //         out var entity
    //     );
    //
    //     // Act
    //     var dto = CategoryMapper.MapToReadDto(entity);
    //
    //     // Assert
    //     dto.Should().NotBeNull();
    //     dto.Id.Should().Be(entity.Id);
    //     dto.ParentCategoryId.Should().Be(entity.ParentCategoryId);
    //     dto.Name.Should().Be(entity.Name);
    //     dto.Description.Should().Be(entity.Description);
    //     dto.ImageUrl.Should().Be(entity.ImageFileName);
    // }
    //
    // [Fact]
    // public void TryCreateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     var dto = new CategoryWriteDto
    //     (
    //         ParentCategoryId: null,
    //         Name: "Test Category",
    //         Description: "Test Description",
    //         ImageUrl: ""
    //     );
    //
    //     // Act
    //     var result = CategoryMapper.TryCreateEntity(dto, out var entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.Should().NotBeNull();
    //     entity.ParentCategoryId.Should().Be(dto.ParentCategoryId);
    //     entity.Name.Should().Be(dto.Name);
    //     entity.Description.Should().Be(dto.Description);
    //     entity.ImageFileName.Should().Be(dto.ImageUrl);
    // }
    //
    // [Fact]
    // public void TryUpdateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     CategoryEntity.TryCreate(null, "Test Category", "Test Description", "", out var entity);
    //
    //     var dto = new CategoryWriteDto
    //     (
    //         ParentCategoryId: null,
    //         Name: "Updated Test Category",
    //         Description: "Updated Test Description",
    //         ImageUrl: ""
    //     );
    //
    //     // Act
    //     var result = CategoryMapper.TryUpdateEntity(dto, entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.ParentCategoryId.Should().Be(dto.ParentCategoryId);
    //     entity.Name.Should().Be(dto.Name);
    //     entity.Description.Should().Be(dto.Description);
    //     entity.ImageFileName.Should().Be(dto.ImageUrl);
    // }
}