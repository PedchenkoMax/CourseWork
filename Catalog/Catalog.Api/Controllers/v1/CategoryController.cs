using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Api.ValidationAttributes;
using Catalog.Api.Validators;
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
    private readonly ILogger<CategoryController> logger;
    private readonly ICategoryRepository categoryRepository;
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;
    private readonly ICategoryMapper categoryMapper;

    public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository,
        IBlobService blobService, IBlobServiceSettings blobServiceSettings,
        IImageHandlingSettings imageHandlingSettings, ICategoryMapper categoryMapper)
    {
        this.logger = logger;
        this.categoryRepository = categoryRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
        this.categoryMapper = categoryMapper;
    }

    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <response code="200">Products successfully retrieved, returns a list of all categories.</response>
    [ProducesResponseType(typeof(List<CategoryReadDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        logger.LogInformation("Getting all categories...");
        var categoryEntities = await categoryRepository.GetAllAsync();

        var categoryDtos = categoryEntities.Select(categoryMapper.MapToDto).ToList();

        logger.LogInformation("Successfully retrieved {CategoryCount} categories", categoryDtos.Count);
        return Ok(categoryDtos);
    }

    /// <summary>
    /// Retrieves all subcategories by parent category ID.
    /// </summary>
    /// <param name="categoryId">ID of the parent category.</param>
    /// <response code="200">Subcategories found and returned successfully.</response>
    /// <response code="404">Parent category with the given ID does not exist.</response>
    [ProducesResponseType(typeof(List<CategoryReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{categoryId:guid}/subcategories")]
    public async Task<IActionResult> GetCategorySubcategories([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        logger.LogInformation("Getting subcategories for category with ID {CategoryId}...", categoryId);

        if (!await categoryRepository.ExistsAsync(categoryId))
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        var categoryEntities = await categoryRepository.GetSubcategoriesByParentCategoryIdAsync(categoryId);

        var categoryDtos = categoryEntities.Select(categoryMapper.MapToDto).ToList();

        logger.LogInformation("Successfully retrieved {SubcategoryCount} subcategories for category with ID {CategoryId}", categoryDtos.Count, categoryId);
        return Ok(categoryDtos);
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="categoryId">ID of the desired category.</param>
    /// <response code="200">Category found and returned successfully.</response>
    /// <response code="404">Category with the given ID does not exist.</response>
    [ProducesResponseType(typeof(CategoryReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategory([FromRoute] [NonZeroGuid] Guid categoryId)
    {
        logger.LogInformation("Getting category with ID {CategoryId}...", categoryId);

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);

        if (categoryEntity == null)
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        var categoryDto = categoryMapper.MapToDto(categoryEntity);

        logger.LogInformation("Successfully retrieved category with ID {CategoryId}", categoryId);
        return Ok(categoryDto);
    }

    /// <summary>
    /// Adds a new category.
    /// </summary>
    /// <param name="dto">Object containing the details of the new category.</param>
    /// <response code="200">Category created successfully, returns the ID of created category.</response>
    /// <response code="400">Invalid category data or category data is null.</response>
    /// <response code="404">Parent category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while adding the category to db.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryWriteDto dto)
    {
        logger.LogInformation("Adding new category...");

        // using var transaction = categoryRepository.BeginTransaction();

        if (dto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(dto.ParentCategoryId.Value))
        {
            logger.LogInformation("Parent category with ID {ParentCategoryId} not found", dto.ParentCategoryId.Value);
            return NotFound(nameof(dto.ParentCategoryId));
        }

        var (validationResult, categoryEntity) = categoryMapper.MapToEntity(dto);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid category data");
            return BadRequest(validationResult);
        }

        var isAdded = await categoryRepository.AddAsync(categoryEntity);

        if (isAdded)
        {
            logger.LogInformation("Category added successfully");
            // transaction.Commit();
            return Ok(categoryEntity.Id);
        }
        else
        {
            logger.LogError("Conflict occurred while adding the category to db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Updating category with ID {CategoryId}...", categoryId);

        // using var transaction = categoryRepository.BeginTransaction();

        if (dto.ParentCategoryId != null && !await categoryRepository.ExistsAsync(dto.ParentCategoryId.Value))
        {
            logger.LogInformation("Parent category with ID {ParentCategoryId} not found", dto.ParentCategoryId.Value);
            return NotFound(nameof(dto.ParentCategoryId));
        }

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);

        if (categoryEntity == null)
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        var validationResult = categoryEntity.Update(
            parentCategoryId: dto.ParentCategoryId,
            name: dto.Name,
            description: dto.Description,
            imageFileName: categoryEntity.ImageFileName);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid category data");
            return BadRequest(validationResult);
        }

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        if (isUpdated)
        {
            logger.LogInformation("Category updated successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while updating the category in db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Deleting category with ID {CategoryId}...", categoryId);

        // using var transaction = categoryRepository.BeginTransaction();

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        if (categoryEntity.ImageFileName != null && categoryEntity.ImageFileName != imageHandlingSettings.DefaultCategoryImageName)
            await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, categoryEntity.ImageFileName);

        var isRemoved = await categoryRepository.RemoveByIdAsync(categoryId);

        if (isRemoved)
        {
            logger.LogInformation("Category deleted successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the category in db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Updating image for category with ID {CategoryId}...", categoryId);

        // using var transaction = categoryRepository.BeginTransaction();

        var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!result.IsValid)
        {
            logger.LogInformation("Invalid image file for category with ID {CategoryId}", categoryId);
            return BadRequest(result);
        }

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        var fileNameToDelete = categoryEntity.ImageFileName;
        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);

        var validationResult = categoryEntity.Update(
            parentCategoryId: categoryEntity.ParentCategoryId,
            name: categoryEntity.Name,
            description: categoryEntity.Description,
            imageFileName: uniqueFileName);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid category data");
            return BadRequest(validationResult);
        }

        var isUpdated = await categoryRepository.UpdateAsync(categoryEntity);

        if (isUpdated)
        {
            await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, fileNameToDelete);
            await blobService.UploadFileAsync(blobServiceSettings.CategoryImageBucketName, uniqueFileName, dto.ImageFile);

            logger.LogInformation("Category image updated successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while updating the category image in db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Deleting image for category with ID {CategoryId}...", categoryId);

        // using var transaction = categoryRepository.BeginTransaction();

        var categoryEntity = await categoryRepository.GetByIdAsync(categoryId);
        if (categoryEntity == null)
        {
            logger.LogInformation("Category with ID {CategoryId} not found", categoryId);
            return NotFound(nameof(categoryId));
        }

        categoryEntity.Update(
            parentCategoryId: categoryEntity.ParentCategoryId,
            name: categoryEntity.Name,
            description: categoryEntity.Description,
            imageFileName: imageHandlingSettings.DefaultCategoryImageName);

        var isRemoved = await categoryRepository.UpdateAsync(categoryEntity);

        if (isRemoved)
        {
            await blobService.DeleteFileAsync(blobServiceSettings.CategoryImageBucketName, categoryEntity.ImageFileName);

            logger.LogInformation("Category image deleted successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the category image from db");
            // transaction.Rollback();
            return Conflict();
        }
    }
}