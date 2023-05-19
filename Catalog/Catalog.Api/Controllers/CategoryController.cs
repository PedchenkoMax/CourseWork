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
        var categories = await categoryRepository.GetAllAsync();

        var res = categories.Select(category => new CategoryReadDto(
            Id: category.Id,
            ParentCategoryId: category.ParentCategoryId,
            Name: category.Name,
            Description: category.Description,
            ImageUrl: category.ImageUrl,
            DisplayOrder: category.DisplayOrder,
            ParentCategory: null,
            Products: null));

        return Ok(res);
    }

    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildrenByParentCategoryId([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound();

        var categories = await categoryRepository.GetChildrenByParentCategoryId(id);

        var res = categories.Select(category => new CategoryReadDto(
            Id: category.Id,
            ParentCategoryId: category.ParentCategoryId,
            Name: category.Name,
            Description: category.Description,
            ImageUrl: category.ImageUrl,
            DisplayOrder: category.DisplayOrder,
            ParentCategory: null,
            Products: null));

        return Ok(res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] [NonZeroGuid] Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        var res = new CategoryReadDto(
            Id: category.Id,
            ParentCategoryId: category.ParentCategoryId,
            Name: category.Name,
            Description: category.Description,
            ImageUrl: category.ImageUrl,
            DisplayOrder: category.DisplayOrder,
            ParentCategory: null,
            Products: null);

        return Ok(res);
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

        var res = await categoryRepository.AddAsync(categoryEntity);

        return Ok(res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] [NonZeroGuid] Guid id, [FromBody] CategoryWriteDto categoryDto)
    {
        if (categoryDto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(categoryDto.ParentCategoryId.Value))
            return NotFound();

        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        var validationResult = category.Update(
            parentCategoryId: categoryDto.ParentCategoryId,
            name: categoryDto.Name,
            description: categoryDto.Description,
            imageUrl: categoryDto.ImageUrl,
            displayOrder: categoryDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await categoryRepository.UpdateAsync(category);

        return Ok(res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound();

        var res = await categoryRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}