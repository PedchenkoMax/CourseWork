using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Api.Mappers;

public class BrandMapperTests
{
    // [Fact]
    // public void MapToReadDto_ValidEntity_ReturnsValidDto()
    // {
    //     // Arrange
    //     BrandEntity.TryCreate
    //     (
    //         name: "Test Brand",
    //         description: "Test Description",
    //         imageFileName: "",
    //         out var entity
    //     );
    //
    //     // Act
    //     var dto = BrandMapper.MapToReadDto(entity);
    //
    //     // Assert
    //     dto.Should().NotBeNull();
    //     dto.Id.Should().Be(entity.Id);
    //     dto.Name.Should().Be(entity.Name);
    //     dto.Description.Should().Be(entity.Description);
    //     dto.ImageUrl.Should().Be(entity.ImageFileName);
    // }
    //
    // [Fact]
    // public void TryCreateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     var dto = new BrandWriteDto
    //     (
    //         Name: "Test Brand",
    //         Description: "Test Description",
    //         ImageUrl: ""
    //     );
    //
    //     // Act
    //     var result = BrandMapper.TryCreateEntity(dto, out var entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.Should().NotBeNull();
    //     entity.Name.Should().Be(dto.Name);
    //     entity.Description.Should().Be(dto.Description);
    //     entity.ImageFileName.Should().Be(dto.ImageUrl);
    // }
    //
    // [Fact]
    // public void TryUpdateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     BrandEntity.TryCreate("Test Brand", "Test Description", "", out var entity);
    //
    //     var dto = new BrandWriteDto
    //     (
    //         Name: "Updated Test Brand",
    //         Description: "Updated Test Description",
    //         ImageUrl: "",
    //     );
    //
    //     // Act
    //     var result = BrandMapper.TryUpdateEntity(dto, entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.Name.Should().Be(dto.Name);
    //     entity.Description.Should().Be(dto.Description);
    //     entity.ImageFileName.Should().Be(dto.ImageUrl);
    // }
}