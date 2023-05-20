using Catalog.Api.Controllers.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.ValidationAttributes;
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
    private readonly IBrandRepository brandRepository;
    private readonly ICategoryRepository categoryRepository;

    public ProductController(IProductRepository productRepository, IProductImageRepository productImageRepository,
        IBrandRepository brandRepository, ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.productImageRepository = productImageRepository;
        this.brandRepository = brandRepository;
        this.categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await productRepository.GetAllAsync();

        if (products.Count == 0)
            return NotFound();

        var res = products.Select(product => new ProductReadDto(
            Id: product.Id,
            BrandId: product.BrandId,
            CategoryId: product.CategoryId,
            Name: product.Name,
            Description: product.Description,
            Price: product.Price,
            Discount: product.Discount,
            SKU: product.SKU,
            Stock: product.Stock,
            Availability: product.Availability,
            Brand: null,
            Category: null,
            Images: null));

        return Ok(res);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById([FromRoute] [NonZeroGuid] Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
            return NotFound();

        var res = new ProductReadDto(
            Id: product.Id,
            BrandId: product.BrandId,
            CategoryId: product.CategoryId,
            Name: product.Name,
            Description: product.Description,
            Price: product.Price,
            Discount: product.Discount,
            SKU: product.SKU,
            Stock: product.Stock,
            Availability: product.Availability,
            Brand: null,
            Category: null,
            Images: null);

        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductWriteDto productDto)
    {
        if (productDto.BrandId != null && !await brandRepository.ExistsAsync(productDto.BrandId.Value))
            return NotFound();

        if (productDto.CategoryId != null && !await categoryRepository.ExistsAsync(productDto.CategoryId.Value))
            return NotFound();

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
    public async Task<IActionResult> UpdateProduct([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductWriteDto productDto)
    {
        if (productDto.BrandId != null && !await brandRepository.ExistsAsync(productDto.BrandId.Value))
            return NotFound();

        if (productDto.CategoryId != null && !await categoryRepository.ExistsAsync(productDto.CategoryId.Value))
            return NotFound();

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
    public async Task<IActionResult> DeleteProduct([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await productRepository.ExistsAsync(id))
            return NotFound();

        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        var res = await productRepository.RemoveByIdAsync(id);

        return Ok(res);
    }

    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetAllProductImagesByProductId([FromRoute] [NonZeroGuid] Guid productId)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound();

        var productImages = await productImageRepository.GetAllByProductIdAsync(productId);

        if (productImages.Count == 0)
            return NotFound();

        var res = productImages.Select(productImage => new ProductImageReadDto(
            Id: productImage.Id,
            ProductId: productImage.ProductId,
            ImageUrl: productImage.ImageUrl,
            DisplayOrder: productImage.DisplayOrder,
            Product: null));

        return Ok(res);
    }

    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromBody] ProductImageWriteDto productImageDto)
    {
        if (!await productRepository.ExistsAsync(productId))
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
    public async Task<IActionResult> UpdateProductImage([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductImageWriteDto productImageDto)
    {
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
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await productImageRepository.ExistsAsync(id))
            return NotFound();

        var res = await productImageRepository.RemoveByIdAsync(id);

        return Ok(res);
    }
}