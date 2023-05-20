using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.ValidationAttributes;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase, IBrandController
{
    private readonly IBrandRepository brandRepository;

    public BrandController(IBrandRepository brandRepository)
    {
        this.brandRepository = brandRepository;
    }

    [HttpGet]
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

    [HttpGet("{id:guid}")]
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

    [HttpPost]
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

    [HttpPut("{id:guid}")]
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await brandRepository.ExistsAsync(id))
            return NotFound();

        var isDeleted = await brandRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}