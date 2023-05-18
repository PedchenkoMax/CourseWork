using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
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

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] Category categoryDto)
    {
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
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] Category categoryDto)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        var validationResult = category.Update(
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
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        var res = await categoryRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}