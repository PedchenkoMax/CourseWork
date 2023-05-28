﻿using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface IProductImageController
{
    Task<IActionResult> GetProductImages(Guid productId);
    Task<IActionResult> AddProductImage(Guid productId, ProductImageCreateDto dto);
    Task<IActionResult> UpdateImageOrder(Guid productImageId, ProductImageUpdateOrderDto dto);
    Task<IActionResult> DeleteProductImage(Guid productImageId);
}