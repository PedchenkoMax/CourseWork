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
        var brands = await brandRepository.GetAllAsync();

        var res = brands.Select(brand => new BrandReadDto(
            Id: brand.Id,
            Name: brand.Name,
            Description: brand.Description,
            ImageUrl: brand.ImageUrl,
            DisplayOrder: brand.DisplayOrder,
            Products: null));

        return Ok(res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBrandById([FromRoute] [NonZeroGuid] Guid id)
    {
        var brand = await brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        var res = new BrandReadDto(
            Id: brand.Id,
            Name: brand.Name,
            Description: brand.Description,
            ImageUrl: brand.ImageUrl,
            DisplayOrder: brand.DisplayOrder,
            Products: null);

        return Ok(res);
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

        var res = await brandRepository.AddAsync(brandEntity);

        return Ok(res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBrand([FromRoute] [NonZeroGuid] Guid id, [FromBody] BrandWriteDto brandDto)
    {
        var brand = await brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        var validationResult = brand.Update(
            name: brandDto.Name,
            description: brandDto.Description,
            imageUrl: brandDto.ImageUrl,
            displayOrder: brandDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await brandRepository.UpdateAsync(brand);

        return Ok(res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBrand([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await brandRepository.ExistsAsync(id))
            return NotFound();

        var res = await brandRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}