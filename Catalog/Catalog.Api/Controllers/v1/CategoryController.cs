using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.ValidationAttributes;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1;

/// <summary>
/// Controller responsible for Category entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoryController : ApiControllerBase<CategoryController>, ICategoryController
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <response code="200">Products successfully retrieved, returns a list of all categories.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categoryEntities = await categoryRepository.GetAllAsync();

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Retrieves all subcategories by parent category ID.
    /// </summary>
    /// <param name="categoryId">ID of the parent category.</param>
    /// <response code="200">Subcategories found and returned successfully.</response>
    /// <response code="404">Parent category with the given ID does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{categoryId:guid}/subcategories")]
    public async Task<IActionResult> GetCategorySubcategories([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        if (!await categoryRepository.ExistsAsync(categoryId))
            return NotFound(nameof(categoryId));

        var categoryEntities = await categoryRepository.GetSubcategoriesByParentCategoryIdAsync(categoryId);

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="categoryId">ID of the desired category.</param>
    /// <response code="200">Category found and returned successfully.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategory([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);

        if (categoryEntity == null)
            return NotFound(nameof(categoryId));

        var categoryDto = CategoryMapper.MapToReadDto(categoryEntity);

        return Ok(categoryDto);
    }

    /// <summary>
    /// Adds a new category.
    /// </summary>
    /// <param name="dto">Object containing the details of the new category.</param>
    /// <response code="200">Category created successfully.</response>
    /// <response code="400">Invalid category data or category data is null.</response>
    /// <response code="404">Parent category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while adding the category.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryWriteDto dto)
    {
        if (dto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(dto.ParentCategoryId.Value))
            return NotFound(nameof(dto.ParentCategoryId));

        var validationResult = CategoryMapper.TryCreateEntity(dto, out var categoryEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await categoryRepository.AddAsync(categoryEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="categoryId">ID of the category to update.</param>
    /// <param name="dto">Object containing the updated details of the category.</param>
    /// <response code="200">Category updated successfully.</response>
    /// <response code="400">Invalid category data or category data is null.</response>
    /// <response code="404">Category or parent category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the category.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] [NonZeroGuid] Guid categoryId, [FromBody] CategoryWriteDto dto)
    {
        if (dto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(dto.ParentCategoryId.Value))
            return NotFound(nameof(dto.ParentCategoryId));

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);

        if (categoryEntity == null)
            return NotFound(nameof(categoryId));

        var validationResult = CategoryMapper.TryUpdateEntity(dto, categoryEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes an existing category.
    /// </summary>
    /// <param name="categoryId">The categoryId of the category to delete.</param>
    /// <response code="200">Category deleted successfully, returns a confirmation of action.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the category.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        if (!await categoryRepository.ExistsAsync(categoryId))
            return NotFound(nameof(categoryId));

        var isDeleted = await categoryRepository.RemoveByIdAsync(categoryId);

        return isDeleted ? Ok() : Conflict();
    }
}