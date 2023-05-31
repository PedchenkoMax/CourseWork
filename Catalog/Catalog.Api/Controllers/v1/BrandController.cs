using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
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

    public BrandController(IBrandRepository brandRepository, IBlobService blobService,
        IBlobServiceSettings blobServiceSettings)
    {
        this.brandRepository = brandRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
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
    /// <response code="409">Conflict occurred while adding the brand to db or adding image to blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddBrand([FromBody] BrandWriteDto dto)
    {
        string? fileName = null;
        if (dto.ImageFile != null)
        {
            var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
            if (!result.IsValid)
                return BadRequest(result);

            // TODO: upload after TryCreate
            fileName = await blobService.UploadFileAsync(blobServiceSettings.BrandImageBucketName, dto.ImageFile);

            if (fileName == null)
                return Conflict();
        }

        var validationResult = BrandEntity.TryCreate(
            name: dto.Name,
            description: dto.Description,
            imageFileName: fileName ?? "default.png", // TODO: replace with value from config
            displayOrder: 0,
            entity: out var brandEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await brandRepository.AddAsync(brandEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates an existing brand.
    /// </summary>
    /// <param name="brandId">ID of the brand to update.</param>
    /// <param name="dto">Object containing the updated details of the brand.</param>
    /// <response code="200">Brand updated successfully.</response>
    /// <response code="400">Invalid brand data or brand data is null.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the brand in db or updating image in blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{brandId:guid}")]
    public async Task<IActionResult> UpdateBrand([FromRoute] [NonZeroGuid] Guid brandId, [FromBody] BrandWriteDto dto)
    {
        string? fileName = null;
        if (dto.ImageFile != null)
        {
            var result = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
            if (!result.IsValid)
                return BadRequest(result);

            // TODO: upload after Update
            fileName = await blobService.UploadFileAsync(blobServiceSettings.BrandImageBucketName, dto.ImageFile);

            if (fileName == null)
                return Conflict();
        }

        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var validationResult = brandEntity.Update(
            name: dto.Name,
            description: dto.Description,
            imageFileName: fileName ?? brandEntity.ImageFileName,
            displayOrder: dto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        return isUpdated ? Ok() : Conflict();
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
        var brandEntity = await brandRepository.GetByIdAsync(brandId);
        if (brandEntity == null)
            return NotFound(nameof(brandId));

        if (brandEntity.Name != "default.png") // TODO: replace with value from config
        {
            var isDeleted = await blobService.DeleteFileAsync(blobServiceSettings.BrandImageBucketName, brandEntity.ImageFileName);
            if (!isDeleted)
                return Conflict();
        }

        var isRemoved = await brandRepository.RemoveByIdAsync(brandId);

        return isRemoved ? Ok() : Conflict();
    }
}