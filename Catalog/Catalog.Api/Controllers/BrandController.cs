using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.ValidationAttributes;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

/// <summary>
/// Controller responsible for Brand entity.
/// </summary>
[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase, IBrandController
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
    /// <response code="404">If the brands list is empty.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllBrands()
    {
        var brandEntities = await brandRepository.GetAllAsync();

        if (brandEntities.Count == 0)
            return NotFound();

        var brandDtos = brandEntities.Select(brandEntity => new BrandReadDto(
            Id: brandEntity.Id,
            Name: brandEntity.Name,
            Description: brandEntity.Description,
            ImageUrl: brandEntity.ImageUrl,
            DisplayOrder: brandEntity.DisplayOrder,
            Products: null));

        return Ok(brandDtos);
    }

    /// <summary>
    /// Gets a specific brand.
    /// </summary>
    /// <param name="id">The id of the brand to get.</param>
    /// <response code="200">Returns the requested brand.</response>
    /// <response code="404">If the brand is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBrandById([FromRoute] [NonZeroGuid] Guid id)
    {
        var brandEntity = await brandRepository.GetByIdAsync(id);

        if (brandEntity == null)
            return NotFound();

        var brandDto = new BrandReadDto(
            Id: brandEntity.Id,
            Name: brandEntity.Name,
            Description: brandEntity.Description,
            ImageUrl: brandEntity.ImageUrl,
            DisplayOrder: brandEntity.DisplayOrder,
            Products: null);

        return Ok(brandDto);
    }

    /// <summary>
    /// Creates a new brand.
    /// </summary>
    /// <param name="brandDto">The brand to create.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the brand is null or invalid.</response>
    /// <response code="409">If there was a conflict while adding the brand.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddBrand([FromBody] BrandWriteDto brandDto)
    {
        var validationResult = BrandEntity.TryCreate(
            name: brandDto.Name,
            description: brandDto.Description,
            imageUrl: brandDto.ImageUrl,
            displayOrder: brandDto.DisplayOrder,
            out var brandEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await brandRepository.AddAsync(brandEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates a specific brand.
    /// </summary>
    /// <param name="id">The id of the brand to update.</param>
    /// <param name="brandDto">The brand to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the brand is null or invalid.</response>
    /// <response code="404">If the brand is not found.</response>
    /// <response code="409">If there was a conflict while updating the brand.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBrand([FromRoute] [NonZeroGuid] Guid id, [FromBody] BrandWriteDto brandDto)
    {
        var brandEntity = await brandRepository.GetByIdAsync(id);

        if (brandEntity == null)
            return NotFound();

        var validationResult = brandEntity.Update(
            name: brandDto.Name,
            description: brandDto.Description,
            imageUrl: brandDto.ImageUrl,
            displayOrder: brandDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await brandRepository.UpdateAsync(brandEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes a specific brand.
    /// </summary>
    /// <param name="id">The id of the brand to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the brand is not found.</response>
    /// <response code="409">If there was a conflict while deleting the brand.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBrand([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await brandRepository.ExistsAsync(id))
            return NotFound();

        var isDeleted = await brandRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}