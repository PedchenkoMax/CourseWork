﻿using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductController
{
    Task<IActionResult> GetAllProducts();
    Task<IActionResult> GetProduct(Guid productId);
    Task<IActionResult> AddProduct(ProductWriteDto dto);
    Task<IActionResult> UpdateProduct(Guid productId, ProductWriteDto dto);
    Task<IActionResult> DeleteProduct(Guid productId);
    Task<IActionResult> GetProductImages(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImageCreateDto dto);
    Task<IActionResult> GetImage(Guid productId, Guid productImageId);
    Task<IActionResult> UpdateImage(Guid productId, Guid productImageId, ProductImageUpdateDto dto);
    Task<IActionResult> DeleteProductImage(Guid productId, Guid productImageId);
}