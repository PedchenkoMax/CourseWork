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
    /// <response code="200">Returns the list of brands.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllBrands()
    {
        var brandEntities = await brandRepository.GetAllAsync();

        var brandDtos = brandEntities.Select(brandEntity => BrandMapper.MapToReadDto(brandEntity));

        return Ok(brandDtos);
    }

    /// <summary>
    /// Gets a specific brand.
    /// </summary>
    /// <param name="brandId">The brandId of the brand to get.</param>
    /// <response code="200">Returns the requested brand.</response>
    /// <response code="404">If the brand is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{brandId:guid}")]
    public async Task<IActionResult> GetBrandById([FromRoute] [NonZeroGuid] Guid brandId)
    {
        var brandEntity = await brandRepository.GetByIdAsync(brandId);

        if (brandEntity == null)
            return NotFound(nameof(brandId));

        var brandDto = BrandMapper.MapToReadDto(brandEntity);

        return Ok(brandDto);
    }

    /// <summary>
    /// Creates a new brand.
    /// </summary>
    /// <param name="dto">The brand to create.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the brand is null or invalid.</response>
    /// <response code="409">If there was a conflict while adding the brand.</response>
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
    /// Updates a specific brand.
    /// </summary>
    /// <param name="brandId">The brandId of the brand to update.</param>
    /// <param name="dto">The brand to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the brand is null or invalid.</response>
    /// <response code="404">If the brand is not found.</response>
    /// <response code="409">If there was a conflict while updating the brand.</response>
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
    /// Deletes a specific brand.
    /// </summary>
    /// <param name="brandId">The brandId of the brand to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the brand is not found.</response>
    /// <response code="409">If there was a conflict while deleting the brand.</response>
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