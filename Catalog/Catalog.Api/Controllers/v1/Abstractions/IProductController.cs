using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductController
{
    Task<IActionResult> GetAllProducts();
    Task<IActionResult> GetProductById(Guid id);
    Task<IActionResult> AddProduct(ProductWriteDto productDto);
    Task<IActionResult> UpdateProduct(Guid id, ProductWriteDto productDto);
    Task<IActionResult> DeleteProduct(Guid id);
    Task<IActionResult> GetAllProductImagesByProductId(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImageWriteDto productImageDto);
    Task<IActionResult> UpdateProductImage(Guid id, ProductImageWriteDto productImageDto);
    Task<IActionResult> DeleteProductImage(Guid id);
}