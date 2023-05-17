using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
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

        return Ok(brands);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBrandById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var brand = await brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        return Ok(brand);
    }

    [HttpPost]
    public async Task<IActionResult> AddBrand([FromBody] Brand brandDto)
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
    public async Task<IActionResult> UpdateBrand([FromRoute] Guid id, [FromBody] Brand brandDto)
    {
        if (id == Guid.Empty)
            return BadRequest();

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
    public async Task<IActionResult> DeleteBrand([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var brand = await brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound();

        var res = await brandRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}