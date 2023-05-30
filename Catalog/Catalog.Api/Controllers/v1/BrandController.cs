using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.ValidationAttributes;
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

    public BrandController(IBrandRepository brandRepository)
    {
        this.brandRepository = brandRepository;
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
    /// <response code="409">Conflict occurred while adding the brand.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddBrand([FromBody] BrandWriteDto dto)
    {
        var validationResult = BrandMapper.TryCreateEntity(dto, out var brandEntity);

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
    /// <response code="409">Conflict occurred while updating the brand.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{brandId:guid}")]
    public async Task<IActionResult> UpdateBrand([FromRoute] [NonZeroGuid] Guid brandId, [FromBody] BrandWriteDto dto)
    {
        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var validationResult = BrandMapper.TryUpdateEntity(dto, brandEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes an existing brand.
    /// </summary>
    /// <param name="brandId">The brandId of the brand to delete.</param>
    /// <response code="200">Brand deleted successfully, returns a confirmation of action.</response>
    /// <response code="404">Brand with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the brand.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{brandId:guid}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] [NonZeroGuid] Guid brandId)
    {
        if (!await brandRepository.ExistsAsync(brandId))
            return NotFound(nameof(brandId));

        var isDeleted = await brandRepository.RemoveByIdAsync(brandId);

        return isDeleted ? Ok() : Conflict();
    }
}