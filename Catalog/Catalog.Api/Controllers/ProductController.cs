using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase, IProductController
{
    private readonly IProductRepository productRepository;
    private readonly IProductImageRepository productImageRepository;

    public ProductController(IProductRepository productRepository, IProductImageRepository productImageRepository)
    {
        this.productRepository = productRepository;
        this.productImageRepository = productImageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await productRepository.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var product = await productRepository.GetByIdAsync(id);

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductWriteDto productDto)
    {
        var validationResult = ProductEntity.TryCreate(
            brandId: productDto.BrandId,
            categoryId: productDto.CategoryId,
            name: productDto.Name,
            description: productDto.Description,
            price: productDto.Price,
            discount: productDto.Discount,
            sku: productDto.SKU,
            stock: productDto.Stock,
            availability: productDto.Availability,
            out var product);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await productRepository.AddAsync(product);

        return Ok(res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductWriteDto productDto)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        var validationResult = product.Update(
            brandId: productDto.BrandId,
            categoryId: productDto.CategoryId,
            name: productDto.Name,
            description: productDto.Description,
            price: productDto.Price,
            discount: productDto.Discount,
            sku: productDto.SKU,
            stock: productDto.Stock,
            availability: productDto.Availability);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await productRepository.UpdateAsync(product);

        return Ok(res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        var res = await productRepository.RemoveByIdAsync(id);

        return Ok(res);
    }

    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetAllProductImagesByProductId([FromRoute] Guid productId)
    {
        var productImages = await productImageRepository.GetAllByProductIdAsync(productId);

        return Ok(productImages);
    }

    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddProductImage([FromRoute] Guid productId, [FromBody] ProductImageWriteDto productImageDto)
    {
        if (productId == Guid.Empty)
            return BadRequest();

        var product = await productRepository.GetByIdAsync(productId);

        if (product == null)
            return NotFound();

        var validationResult = ProductImageEntity.TryCreate(
            productId: productId,
            imageUrl: productImageDto.ImageUrl,
            displayOrder: productImageDto.DisplayOrder,
            out var productImage);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await productImageRepository.AddAsync(productImage);

        return Ok(res);
    }

    [HttpPut("images/{id:guid}")]
    public async Task<IActionResult> UpdateProductImage([FromRoute] Guid id, [FromBody] ProductImageWriteDto productImageDto)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var productImage = await productImageRepository.GetByIdAsync(id);

        if (productImage == null)
            return NotFound();

        var validationResult = productImage.Update(displayOrder: productImageDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var res = await productImageRepository.UpdateAsync(productImage);

        return Ok(res);
    }

    [HttpDelete("images/{id:guid}")]
    public async Task<IActionResult> DeleteProductImage([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var productImage = await productImageRepository.GetByIdAsync(id);

        if (productImage == null)
            return NotFound();

        var res = await productImageRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}