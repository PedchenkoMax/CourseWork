using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductImageController
{
    Task<IActionResult> GetAllProductImagesByProductId(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImageCreateDto productImageDto);
    Task<IActionResult> UpdateProductImageOrder(Guid id, ProductImageUpdateOrderDto productImageDto);
    Task<IActionResult> DeleteProductImage(Guid id);
}