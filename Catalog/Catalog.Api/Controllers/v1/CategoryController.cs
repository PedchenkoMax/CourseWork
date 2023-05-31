using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Api.ValidationAttributes;
using Catalog.Api.Validators;
using Catalog.Domain.Entities;
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
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;

    public CategoryController(ICategoryRepository categoryRepository, IBlobService blobService,
        IBlobServiceSettings blobServiceSettings)
    {
        this.categoryRepository = categoryRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
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
    /// <response code="409">Conflict occurred while adding the category to db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryWriteDto dto)
    {
        if (dto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(dto.ParentCategoryId.Value))
            return NotFound(nameof(dto.ParentCategoryId));

        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: dto.ParentCategoryId,
            name: dto.Name,
            description: dto.Description,
            imageFileName: "default.png",
            out var categoryEntity);

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
    /// <response code="409">Conflict occurred while updating the category in db.</response>
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

        var validationResult = categoryEntity.Update(
            parentCategoryId: dto.ParentCategoryId,
            name: dto.Name,
            description: dto.Description,
            imageFileName: categoryEntity.ImageFileName);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes an existing category.
    /// </summary>
    /// <param name="categoryId">The categoryId of the category to delete.</param>
    /// <response code="200">Category deleted successfully.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the category from db or deleting image from blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
            return NotFound(nameof(categoryId));

        if (categoryEntity.Name != "default.png") // TODO: replace with value from config
            await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, categoryEntity.ImageFileName);

        var isRemoved = await categoryRepository.RemoveByIdAsync(categoryId);

        return isRemoved ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates an image of a specific category.
    /// </summary>
    /// <param name="categoryId">ID of the category for which the image is to be updated.</param>
    /// <param name="dto">Object containing the file data of the image.</param>
    /// <response code="200">Category image added successfully.</response>
    /// <response code="400">Invalid image data, image data is null.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while uploading the image to blob storage or updating the category in db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{categoryId:guid}/image/")]
    public async Task<IActionResult> UpdateCategoryImage(Guid categoryId, [FromForm] CategoryImageUpdateDto dto)
    {
        var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!result.IsValid)
            return BadRequest(result);

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
            return NotFound(nameof(categoryId));

        // TODO: upload after TryCreate
        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);
        await blobService.UploadFileAsync(blobServiceSettings.CategoryImageBucketName, uniqueFileName, dto.ImageFile);
        await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, categoryEntity.ImageFileName);

        var validationResult = categoryEntity.Update(
            parentCategoryId: categoryEntity.ParentCategoryId,
            name: categoryEntity.Name,
            description: categoryEntity.Description,
            imageFileName: uniqueFileName);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes an image from a specific category.
    /// </summary>
    /// <param name="categoryId">ID of the category from which the image is to be deleted.</param>
    /// <response code="200">Category image deleted successfully.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the image from blob storage or updating the category in db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{categoryId:guid}/image/")]
    public async Task<IActionResult> DeleteCategoryImage(Guid categoryId)
    {
        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
            return NotFound(nameof(categoryId));

        // TODO: upload after TryCreate
        await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, categoryEntity.ImageFileName);

        categoryEntity.Update(
            parentCategoryId: categoryEntity.ParentCategoryId,
            name: categoryEntity.Name,
            description: categoryEntity.Description,
            imageFileName: "default.png");

        var isRemoved = await categoryRepository.UpdateAsync(categoryEntity);

        return isRemoved ? Ok() : Conflict();
    }
}