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
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories()
    {
        var categoryEntities = await categoryRepository.GetAllAsync();

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Gets all child categories for a parent category by parent id.
    /// </summary>
    /// <param name="id">The id of the parent category.</param>
    /// <response code="200">Returns the list of child categories.</response>
    /// <response code="404">If the parent category is not found.</response>
    [HttpGet("{id:guid}/children")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChildrenByParentCategoryId([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound(nameof(id));

        var categoryEntities = await categoryRepository.GetChildrenByParentCategoryId(id);

        var categoryDtos = categoryEntities.Select(entity => CategoryMapper.MapToReadDto(entity));

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Gets a specific category.
    /// </summary>
    /// <param name="id">The id of the category to get.</param>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById([FromRoute] [NonZeroGuid] Guid id)
    {
        var categoryEntity = await categoryRepository.GetByIdAsync(id);

        if (categoryEntity == null)
            return NotFound(nameof(id));

        var categoryDto = CategoryMapper.MapToReadDto(categoryEntity);

        return Ok(categoryDto);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="categoryDto">The category to create.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the category is null or invalid.</response>
    /// <response code="404">If the parent category is not found.</response>
    /// <response code="409">If there was a conflict while adding the category.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCategory([FromBody] CategoryWriteDto categoryDto)
    {
        if (categoryDto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(categoryDto.ParentCategoryId.Value))
            return NotFound(nameof(categoryDto.ParentCategoryId));

        var validationResult = CategoryMapper.TryCreateEntity(categoryDto, out var categoryEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await categoryRepository.AddAsync(categoryEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates a specific category.
    /// </summary>
    /// <param name="id">The id of the category to update.</param>
    /// <param name="categoryDto">The category to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the category is null or invalid.</response>
    /// <response code="404">If the category or parent category is not found.</response>
    /// <response code="409">If there was a conflict while updating the category.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory([FromRoute] [NonZeroGuid] Guid id, [FromBody] CategoryWriteDto categoryDto)
    {
        if (categoryDto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(categoryDto.ParentCategoryId.Value))
            return NotFound(nameof(categoryDto.ParentCategoryId));

        var categoryEntity = await categoryRepository.GetByIdAsync(id);

        if (categoryEntity == null)
            return NotFound(nameof(id));

        var validationResult = CategoryMapper.TryUpdateEntity(categoryDto, categoryEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes a specific category.
    /// </summary>
    /// <param name="id">The id of the category to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the category is not found.</response>
    /// <response code="409">If there was a conflict while deleting the category.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await categoryRepository.ExistsAsync(id))
            return NotFound(nameof(id));

        var isDeleted = await categoryRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}