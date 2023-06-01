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
/// Controller responsible for Brand entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/brands")]
public class BrandController : ApiControllerBase<BrandController>, IBrandController
{
    private readonly IBrandRepository brandRepository;
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;

    public BrandController(IBrandRepository brandRepository, IBlobService blobService,
        IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings)
    {
        this.brandRepository = brandRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
    }

    /// <summary>
    /// Gets all brands.
    /// </summary>
    /// <response code="200">Brands successfully retrieved, returns a list of all brands.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllBrands()
    {
        var brandEntities = await brandRepository.GetAllAsync();

        var brandDtos = brandEntities.Select(brandEntity => BrandMapper.MapToReadDto(brandEntity));

        return Ok(brandDtos);
    }

    /// <summary>
    /// Retrieves a specific brand by its ID.
    /// </summary>
    /// <param name="brandId">ID of the desired brand.</param>
    /// <response code="200">Brand found and returned successfully.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{brandId:guid}")]
    public async Task<IActionResult> GetBrand([FromRoute] [NonZeroGuid] Guid brandId)
    {
        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var brandDto = BrandMapper.MapToReadDto(brandEntity);

        return Ok(brandDto);
    }

    /// <summary>
    /// Adds a new brand.
    /// </summary>
    /// <param name="dto">Object containing the details of the new brand.</param>
    /// <response code="200">Brand created successfully.</response>
    /// <response code="400">Invalid brand data or brand data is null.</response>
    /// <response code="409">Conflict occurred while adding the brand to db.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddBrand([FromBody] BrandWriteDto dto)
    {
        var validationResult = BrandEntity.TryCreate(
            name: dto.Name,
            description: dto.Description,
            imageFileName: imageHandlingSettings.DefaultBrandImageName,
            entity: out var brandEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        using var transaction = brandRepository.BeginTransaction();

        var isAdded = await brandRepository.AddAsync(brandEntity);

        if (isAdded)
        {
            transaction.Commit();
            return Ok();
        }
        else
        {
            transaction.Rollback();
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
        using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var validationResult = brandEntity.Update(
            name: dto.Name,
            description: dto.Description,
            imageFileName: brandEntity.ImageFileName);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        if (isUpdated)
        {
            transaction.Commit();
            return Ok();
        }
        else
        {
            transaction.Rollback();
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
        using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
            return NotFound(nameof(brandId));

        if (brandEntity.Name != imageHandlingSettings.DefaultBrandImageName)
            await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);

        var isRemoved = await brandRepository.RemoveByIdAsync(brandId);

        if (isRemoved)
        {
            transaction.Commit();
            return Ok();
        }
        else
        {
            transaction.Rollback();
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
        var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!result.IsValid)
            return BadRequest(result);

        using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);

        var validationResult = brandEntity.Update(
            name: brandEntity.Name,
            description: brandEntity.Description,
            imageFileName: uniqueFileName);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        if (isUpdated)
        {
            await blobService.UploadFileAsync(blobServiceSettings.BrandImageBucketName, uniqueFileName, dto.ImageFile);
            await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);

            transaction.Commit();
            return Ok();
        }
        else
        {
            transaction.Rollback();
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
        using var transaction = brandRepository.BeginTransaction();

        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
            return NotFound(nameof(brandId));

        brandEntity.Update(
            name: brandEntity.Name,
            description: brandEntity.Description,
            imageFileName: imageHandlingSettings.DefaultBrandImageName);

        var isRemoved = await brandRepository.UpdateAsync(brandEntity);

        if (isRemoved)
        {
            await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);

            transaction.Commit();
            return Ok();
        }
        else
        {
            transaction.Rollback();
            return Conflict();
        }
    }
}