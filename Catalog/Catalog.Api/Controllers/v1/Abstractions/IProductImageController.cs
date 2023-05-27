using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductImageController
{
    Task<IActionResult> GetAllProductImagesByProductId(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImageWriteDto productImageDto);
    Task<IActionResult> UpdateProductImage(Guid id, ProductImageWriteDto productImageDto);
    Task<IActionResult> DeleteProductImage(Guid id);
}