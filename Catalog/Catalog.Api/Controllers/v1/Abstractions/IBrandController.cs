using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IBrandController
{
    Task<IActionResult> GetAllBrands();
    Task<IActionResult> GetBrand(Guid brandId);
    Task<IActionResult> AddBrand(BrandWriteDto dto);
    Task<IActionResult> UpdateBrand(Guid brandId, BrandWriteDto dto);
    Task<IActionResult> DeleteBrand(Guid brandId);
}