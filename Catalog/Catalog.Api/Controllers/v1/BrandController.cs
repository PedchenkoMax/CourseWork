using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Api.ValidationAttributes;
using Catalog.Api.Validators;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1;

/// <summary>
/// Controller responsible for Brand entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/brands")]
public class BrandController : ApiControllerBase<BrandController>, IBrandController
{
    private readonly ILogger<BrandController> logger;
    private readonly IBrandRepository brandRepository;
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;
    private readonly IBrandMapper brandMapper;

    public BrandController(ILogger<BrandController> logger, IBrandRepository brandRepository,
        IBlobService blobService, IBlobServiceSettings blobServiceSettings,
        IImageHandlingSettings imageHandlingSettings, IBrandMapper brandMapper)
    {
        this.logger = logger;
        this.brandRepository = brandRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
        this.brandMapper = brandMapper;
    }

    /// <summary>
    /// Gets all brands.
    /// </summary>
    /// <response code="200">Brands successfully retrieved, returns a list of all brands.</response>
    [ProducesResponseType(typeof(List<BrandReadDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllBrands()
    {
        logger.LogInformation("Getting all brands...");
        var brandEntities = await brandRepository.GetAllAsync();

        var brandDtos = brandEntities.Select(brandMapper.MapToDto).ToList();

        logger.LogInformation("Successfully retrieved {BrandCount} brands", brandDtos.Count);
        return Ok(brandDtos);
    }

    /// <summary>
    /// Retrieves a specific brand by its ID.
    /// </summary>
    /// <param name="brandId">ID of the desired brand.</param>
    /// <response code="200">Brand found and returned successfully.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    [ProducesResponseType(typeof(BrandReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{brandId:guid}")]
    public async Task<IActionResult> GetBrand([FromRoute] [NonZeroGuid] Guid brandId)
    {
        logger.LogInformation("Getting brand with ID {BrandId}...", brandId);

        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
        {
            logger.LogInformation("Brand with ID {BrandId} not found", brandId);
            return NotFound(nameof(brandId));
        }

        var brandDto = brandMapper.MapToDto(brandEntity);

        logger.LogInformation("Successfully retrieved brand with ID {BrandId}", brandId);

        return Ok(brandDto);
    }

    /// <summary>
    /// Adds a new brand.
    /// </summary>
    /// <param name="dto">Object containing the details of the new brand.</param>
    /// <response code="200">Brand created successfully, returns the ID of created brand.</response>
    /// <response code="400">Invalid brand data or brand data is null.</response>
    /// <response code="409">Conflict occurred while adding the brand to db.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddBrand([FromBody] BrandWriteDto dto)
    {
        logger.LogInformation("Adding new brand...");

        var (validationResult, brandEntity) = brandMapper.MapToEntity(dto);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid brand data");
            return BadRequest(validationResult);
        }

        // using var transaction = brandRepository.BeginTransaction();

        var isAdded = await brandRepository.AddAsync(brandEntity);

        if (isAdded)
        {
            logger.LogInformation("Brand added successfully");
            // transaction.Commit();
            return Ok(brandEntity.Id);
        }
        else
        {
            logger.LogError("Conflict occurred while adding the brand to db");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Updates an existing brand.
    /// </summary>
    /// <param name="brandId">ID of the brand to update.</param>
    /// <param name="dto">Object containing the updated details of the brand.</param>
    /// <response code="200">Brand updated successfully.</response>
    /// <response code="400">Invalid brand data or brand data is null.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the brand in db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{brandId:guid}")]
    public async Task<IActionResult> UpdateBrand([FromRoute] [NonZeroGuid] Guid brandId, [FromBody] BrandWriteDto dto)
    {
        logger.LogInformation("Updating brand with ID {BrandId}...", brandId);

        // using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
        {
            logger.LogInformation("Brand with ID {BrandId} not found", brandId);
            return NotFound(nameof(brandId));
        }

        var validationResult = brandEntity.Update(
            name: dto.Name,
            description: dto.Description,
            imageFileName: brandEntity.ImageFileName);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid brand data");
            return BadRequest(validationResult);
        }

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        if (isUpdated)
        {
            logger.LogInformation("Brand updated successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while updating the brand in db");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Deletes an existing brand.
    /// </summary>
    /// <param name="brandId">The brandId of the brand to delete.</param>
    /// <response code="200">Brand deleted successfully.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the brand from db or deleting image from blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{brandId:guid}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] [NonZeroGuid] Guid brandId)
    {
        logger.LogInformation("Deleting brand with ID {BrandId}...", brandId);
        // using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
        {
            logger.LogInformation("Brand with ID {BrandId} not found", brandId);
            return NotFound(nameof(brandId));
        }

        if (brandEntity.ImageFileName != null && brandEntity.ImageFileName != imageHandlingSettings.DefaultBrandImageName)
            await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);

        var isRemoved = await brandRepository.RemoveByIdAsync(brandId);

        if (isRemoved)
        {
            logger.LogInformation("Brand deleted successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the brand in db");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Updates an image of a specific brand.
    /// </summary>
    /// <param name="brandId">ID of the brand for which the image is to be updated.</param>
    /// <param name="dto">Object containing the file data of the image.</param>
    /// <response code="200">Brand image added successfully.</response>
    /// <response code="400">Invalid image data, image data is null.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while uploading the image to blob storage or updating the brand in db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{brandId:guid}/image/")]
    public async Task<IActionResult> UpdateBrandImage(Guid brandId, [FromForm] BrandImageUpdateDto dto)
    {
        logger.LogInformation("Updating image for brand with ID {BrandId}...", brandId);

        var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!result.IsValid)
        {
            logger.LogInformation("Invalid image file for brand with ID {BrandId}", brandId);
            return BadRequest(result);
        }

        // using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
        {
            logger.LogInformation("Brand with ID {BrandId} not found", brandId);
            return NotFound(nameof(brandId));
        }

        var fileNameToDelete = brandEntity.ImageFileName;
        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);

        var validationResult = brandEntity.Update(
            name: brandEntity.Name,
            description: brandEntity.Description,
            imageFileName: uniqueFileName);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid brand data");
            return BadRequest(validationResult);
        }

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        if (isUpdated)
        {
            if (fileNameToDelete != null)
                await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, fileNameToDelete);

            await blobService.UploadFileAsync(blobServiceSettings.BrandImageBucketName, uniqueFileName, dto.ImageFile);

            logger.LogInformation("Brand image updated successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while updating the brand image in db");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Deletes an image from a specific brand.
    /// </summary>
    /// <param name="brandId">ID of the brand from which the image is to be deleted.</param>
    /// <response code="200">Brand image deleted successfully.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the image from blob storage or updating the brand in db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{brandId:guid}/image/")]
    public async Task<IActionResult> DeleteBrandImage(Guid brandId)
    {
        logger.LogInformation("Deleting image for brand with ID {BrandId}...", brandId);

        // using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
        {
            logger.LogInformation("Brand with ID {BrandId} not found", brandId);
            return NotFound(nameof(brandId));
        }

        brandEntity.Update(
            name: brandEntity.Name,
            description: brandEntity.Description,
            imageFileName: imageHandlingSettings.DefaultBrandImageName);

        var isRemoved = await brandRepository.UpdateAsync(brandEntity);

        if (isRemoved)
        {
            await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);

            logger.LogInformation("Brand image deleted successfully");
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the brand image from db");
            // transaction.Rollback();
            return Conflict();
        }
    }
}