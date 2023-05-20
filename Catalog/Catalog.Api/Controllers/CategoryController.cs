using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.ValidationAttributes;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase, ICategoryController
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categoryEntities = await categoryRepository.GetAllAsync();

        if (categoryEntities.Count == 0)
            return NotFound();

        var categoryDtos = categoryEntities.Select(categoryEntity => new CategoryReadDto(
            Id: categoryEntity.Id,
            ParentCategoryId: categoryEntity.ParentCategoryId,
            Name: categoryEntity.Name,
            Description: categoryEntity.Description,
            ImageUrl: categoryEntity.ImageUrl,
            DisplayOrder: categoryEntity.DisplayOrder,
            ParentCategory: null,
            Products: null));

        return Ok(categoryDtos);
    }

    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildrenByParentCategoryId([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound();

        var categoryEntities = await categoryRepository.GetChildrenByParentCategoryId(id);

        if (categoryEntities.Count == 0)
            return NotFound();

        var categoryDtos = categoryEntities.Select(categoryEntity => new CategoryReadDto(
            Id: categoryEntity.Id,
            ParentCategoryId: categoryEntity.ParentCategoryId,
            Name: categoryEntity.Name,
            Description: categoryEntity.Description,
            ImageUrl: categoryEntity.ImageUrl,
            DisplayOrder: categoryEntity.DisplayOrder,
            ParentCategory: null,
            Products: null));

        return Ok(categoryDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] [NonZeroGuid] Guid id)
    {
        var categoryEntity = await categoryRepository.GetByIdAsync(id);

        if (categoryEntity == null)
            return NotFound();

        var categoryDto = new CategoryReadDto(
            Id: categoryEntity.Id,
            ParentCategoryId: categoryEntity.ParentCategoryId,
            Name: categoryEntity.Name,
            Description: categoryEntity.Description,
            ImageUrl: categoryEntity.ImageUrl,
            DisplayOrder: categoryEntity.DisplayOrder,
            ParentCategory: null,
            Products: null);

        return Ok(categoryDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryWriteDto categoryDto)
    {
        if (categoryDto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(categoryDto.ParentCategoryId.Value))
            return NotFound();

        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: categoryDto.ParentCategoryId,
            name: categoryDto.Name,
            description: categoryDto.Description,
            imageUrl: categoryDto.ImageUrl,
            displayOrder: categoryDto.DisplayOrder,
            out var categoryEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await categoryRepository.AddAsync(categoryEntity);

        return isAdded ? Ok() : Conflict();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] [NonZeroGuid] Guid id, [FromBody] CategoryWriteDto categoryDto)
    {
        if (categoryDto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(categoryDto.ParentCategoryId.Value))
            return NotFound();

        var categoryEntity = await categoryRepository.GetByIdAsync(id);

        if (categoryEntity == null)
            return NotFound();

        var validationResult = categoryEntity.Update(
            parentCategoryId: categoryDto.ParentCategoryId,
            name: categoryDto.Name,
            description: categoryDto.Description,
            imageUrl: categoryDto.ImageUrl,
            displayOrder: categoryDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        return isUpdated ? Ok() : Conflict();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound();

        var isDeleted = await categoryRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}