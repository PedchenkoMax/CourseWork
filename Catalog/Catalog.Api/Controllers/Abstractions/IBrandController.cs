using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.Abstractions;

public interface IBrandController
{
    Task<IActionResult> GetAllBrands();
    Task<IActionResult> GetBrandById(Guid id);
    Task<IActionResult> AddBrand(Brand brandDto);
    Task<IActionResult> UpdateBrand(Guid id, Brand brandDto);
    Task<IActionResult> DeleteBrand(Guid id);
}