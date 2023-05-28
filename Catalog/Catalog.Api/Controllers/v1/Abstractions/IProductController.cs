using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductController
{
    Task<IActionResult> GetAllProducts();
    Task<IActionResult> GetProduct(Guid productId);
    Task<IActionResult> AddProduct(ProductWriteDto dto);
    Task<IActionResult> UpdateProduct(Guid productId, ProductWriteDto dto);
    Task<IActionResult> DeleteProduct(Guid productId);
}