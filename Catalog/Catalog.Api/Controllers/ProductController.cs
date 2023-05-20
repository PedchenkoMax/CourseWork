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
        var productEntities = await productRepository.GetAllAsync();

        if (productEntities.Count == 0)
            return NotFound();

        var productDtos = productEntities.Select(productEntity => new ProductReadDto(
            Id: productEntity.Id,
            BrandId: productEntity.BrandId,
            CategoryId: productEntity.CategoryId,
            Name: productEntity.Name,
            Description: productEntity.Description,
            Price: productEntity.Price,
            Discount: productEntity.Discount,
            SKU: productEntity.SKU,
            Stock: productEntity.Stock,
            Availability: productEntity.Availability,
            Brand: null,
            Category: null,
            Images: null));

        return Ok(productDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById([FromRoute] [NonZeroGuid] Guid id)
    {
        var productEntity = await productRepository.GetByIdAsync(id);

        if (productEntity == null)
            return NotFound();

        var productDto = new ProductReadDto(
            Id: productEntity.Id,
            BrandId: productEntity.BrandId,
            CategoryId: productEntity.CategoryId,
            Name: productEntity.Name,
            Description: productEntity.Description,
            Price: productEntity.Price,
            Discount: productEntity.Discount,
            SKU: productEntity.SKU,
            Stock: productEntity.Stock,
            Availability: productEntity.Availability,
            Brand: null,
            Category: null,
            Images: null);

        return Ok(productDto);
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
            out var productEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await productRepository.AddAsync(productEntity);

        return isAdded ? Ok() : Conflict();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductWriteDto productDto)
    {
        if (productDto.BrandId != null && !await brandRepository.ExistsAsync(productDto.BrandId.Value))
            return NotFound();

        if (productDto.CategoryId != null && !await categoryRepository.ExistsAsync(productDto.CategoryId.Value))
            return NotFound();

        var productEntity = await productRepository.GetByIdAsync(id);

        if (productEntity == null)
            return NotFound();

        var validationResult = productEntity.Update(
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

        var isUpdated = await productRepository.UpdateAsync(productEntity);

        return isUpdated ? Ok() : Conflict();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await productRepository.ExistsAsync(id))
            return NotFound();

        var productEntity = await productRepository.GetByIdAsync(id);

        if (productEntity == null)
            return NotFound();

        var isDeleted = await productRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }

    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetAllProductImagesByProductId([FromRoute] [NonZeroGuid] Guid productId)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound();

        var productImageEntities = await productImageRepository.GetAllByProductIdAsync(productId);

        if (productImageEntities.Count == 0)
            return NotFound();

        var productImageDtos = productImageEntities.Select(productImageEntity => new ProductImageReadDto(
            Id: productImageEntity.Id,
            ProductId: productImageEntity.ProductId,
            ImageUrl: productImageEntity.ImageUrl,
            DisplayOrder: productImageEntity.DisplayOrder,
            Product: null));

        return Ok(productImageDtos);
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
            out var productImageEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await productImageRepository.AddAsync(productImageEntity);

        return isAdded ? Ok() : Conflict();
    }

    [HttpPut("images/{id:guid}")]
    public async Task<IActionResult> UpdateProductImage([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductImageWriteDto productImageDto)
    {
        var productImageEntity = await productImageRepository.GetByIdAsync(id);

        if (productImageEntity == null)
            return NotFound();

        var validationResult = productImageEntity.Update(displayOrder: productImageDto.DisplayOrder);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await productImageRepository.UpdateAsync(productImageEntity);

        return isUpdated ? Ok() : Conflict();
    }

    [HttpDelete("images/{id:guid}")]
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await productImageRepository.ExistsAsync(id))
            return NotFound();

        var isDeleted = await productImageRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}