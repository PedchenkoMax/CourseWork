using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.Abstractions;

public interface IProductController
{
    Task<IActionResult> GetAllProducts();
    Task<IActionResult> GetProductById(Guid id);
    Task<IActionResult> AddProduct(Product productDto);
    Task<IActionResult> UpdateProduct(Guid id, Product productDto);
    Task<IActionResult> DeleteProduct(Guid id);
    Task<IActionResult> GetAllProductImagesByProductId(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImage productImageDto);
    Task<IActionResult> UpdateProductImage(Guid id, ProductImage productImageDto);
    Task<IActionResult> DeleteProductImage(Guid id);
}