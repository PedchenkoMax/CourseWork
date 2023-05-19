using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.Abstractions;

public interface IBrandController
{
    Task<IActionResult> GetAllBrands();
    Task<IActionResult> GetBrandById(Guid id);
    Task<IActionResult> AddBrand(BrandWriteDto brandDto);
    Task<IActionResult> UpdateBrand(Guid id, BrandWriteDto brandDto);
    Task<IActionResult> DeleteBrand(Guid id);
}