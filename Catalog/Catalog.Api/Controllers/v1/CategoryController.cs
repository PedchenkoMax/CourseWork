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
    /// <response code="200">Returns the list of categories.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categoryEntities = await categoryRepository.GetAllAsync();

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Gets all child categories by parent categoryId.
    /// </summary>
    /// <param name="categoryId">The categoryId of the parent category.</param>
    /// <response code="200">Returns the list of child categories.</response>
    /// <response code="404">If the parent category is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{categoryId:guid}/children")]
    public async Task<IActionResult> GetCategoryChildren([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        if (!await categoryRepository.ExistsAsync(categoryId))
            return NotFound(nameof(categoryId));

        var categoryEntities = await categoryRepository.GetChildrenByParentCategoryId(categoryId);

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Gets a specific category.
    /// </summary>
    /// <param name="categoryId">The categoryId of the category to get.</param>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category is not found.</response>
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
    /// Creates a new category.
    /// </summary>
    /// <param name="dto">The category to create.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the category is null or invalid.</response>
    /// <response code="404">If the parent category is not found.</response>
    /// <response code="409">If there was a conflict while adding the category.</response>
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
    /// Updates a specific category.
    /// </summary>
    /// <param name="categoryId">The categoryId of the category to update.</param>
    /// <param name="dto">The category to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the category is null or invalid.</response>
    /// <response code="404">If the category or parent category is not found.</response>
    /// <response code="409">If there was a conflict while updating the category.</response>
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
    /// Deletes a specific category.
    /// </summary>
    /// <param name="categoryId">The categoryId of the category to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the category is not found.</response>
    /// <response code="409">If there was a conflict while deleting the category.</response>
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