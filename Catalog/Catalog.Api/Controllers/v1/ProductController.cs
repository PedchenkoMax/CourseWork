using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.Services.Abstractions;
using Catalog.Api.ValidationAttributes;
using Catalog.Api.Validators;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1;

/// <summary>
/// Controller responsible for Product entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ApiControllerBase<ProductController>, IProductController
{
    private const string BlobAccess = "VALUE";
    private const string BucketName = "VALUE";

    private readonly IProductRepository productRepository;
    private readonly IProductImageRepository productImageRepository;
    private readonly IBrandRepository brandRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IBlobService blobService;

    public ProductController(IProductRepository productRepository, IProductImageRepository productImageRepository,
        IBrandRepository brandRepository, ICategoryRepository categoryRepository, IBlobService blobService)
    {
        this.productRepository = productRepository;
        this.productImageRepository = productImageRepository;
        this.brandRepository = brandRepository;
        this.categoryRepository = categoryRepository;
        this.blobService = blobService;
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <response code="200">Returns the list of products.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var productEntities = await productRepository.GetAllAsync();

        var productDtos = productEntities.Select(productEntity => ProductMapper.MapToReadDto(productEntity));

        return Ok(productDtos);
    }

    /// <summary>
    /// Gets a specific product.
    /// </summary>
    /// <param name="productId">The productId of the product to get.</param>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetProduct([FromRoute] [NonZeroGuid] Guid productId)
    {
        var productEntity = await productRepository.GetByIdAsync(productId);

        if (productEntity == null)
            return NotFound(nameof(productId));

        var productDto = ProductMapper.MapToReadDto(productEntity);

        return Ok(productDto);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="dto">The product to create.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the product is null or invalid.</response>
    /// <response code="404">If the brand or category is not found.</response>
    /// <response code="409">If there was a conflict while adding the product.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductWriteDto dto)
    {
        if (dto.BrandId != null && !await brandRepository.ExistsAsync(dto.BrandId.Value))
            return NotFound(nameof(dto.BrandId));

        if (dto.CategoryId != null && !await categoryRepository.ExistsAsync(dto.CategoryId.Value))
            return NotFound(nameof(dto.CategoryId));

        var validationResult = ProductMapper.TryCreateEntity(dto, out var productEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await productRepository.AddAsync(productEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates a specific product.
    /// </summary>
    /// <param name="productId">The productId of the product to update.</param>
    /// <param name="dto">The product to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the product is null or invalid.</response>
    /// <response code="404">If the product, brand, or category is not found.</response>
    /// <response code="409">If there was a conflict while updating the product.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] [NonZeroGuid] Guid productId, [FromBody] ProductWriteDto dto)
    {
        if (dto.BrandId != null && !await brandRepository.ExistsAsync(dto.BrandId.Value))
            return NotFound(nameof(dto.BrandId));

        if (dto.CategoryId != null && !await categoryRepository.ExistsAsync(dto.CategoryId.Value))
            return NotFound(nameof(dto.CategoryId));

        var productEntity = await productRepository.GetByIdAsync(productId);

        if (productEntity == null)
            return NotFound(nameof(productId));

        var validationResult = ProductMapper.TryUpdateEntity(dto, productEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await productRepository.UpdateAsync(productEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes a specific product.
    /// </summary>
    /// <param name="productId">The productId of the product to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="409">If there was a conflict while deleting the product.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] [NonZeroGuid] Guid productId)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var productEntity = await productRepository.GetByIdAsync(productId);

        if (productEntity == null)
            return NotFound(nameof(productId));

        var isDeleted = await productRepository.RemoveByIdAsync(productId);

        return isDeleted ? Ok() : Conflict();
    }

    /// <summary>
    /// Gets all product images by product productId.
    /// </summary>
    /// <param name="productId">The productId of the product.</param>
    /// <response code="200">Returns the list of product images.</response>
    /// <response code="404">If the product is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}/all")]
    public async Task<IActionResult> GetProductImages([FromRoute] [NonZeroGuid] Guid productId)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var productImageEntities = await productImageRepository.GetAllByProductIdAsync(productId);

        var productImageDtos = productImageEntities.Select(productImageEntity => ProductImageMapper.MapToReadDto(productImageEntity));

        return Ok(productImageDtos);
    }

    /// <summary>
    /// Adds a new product image.
    /// </summary>
    /// <param name="productId">The productId of the product.</param>
    /// <param name="dto">The product image to add.</param>
    /// <response code="200">Returns the URL to the added product image.</response>
    /// <response code="400">If the product image is null, invalid, or the maximum limit of images has been reached.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="409">If there was a conflict while adding the product image to db or adding it to the blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromForm] ProductImageCreateDto dto)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var validationResult = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        const int maxCount = 10;
        var imageCount = await productImageRepository.GetProductImageCount(productId);
        if (imageCount > maxCount)
            return BadRequest(nameof(imageCount), $"The maximum number of images {maxCount} for this product has been reached.");

        var uniqueFileName = await blobService.UploadFileAsync(BucketName, dto.ImageFile);
        if (uniqueFileName == null)
            return Conflict();

        var imageUrl = $"{BlobAccess}/{BucketName}/{uniqueFileName}";

        // No need for validation check as it always valid at this point
        _ = ProductImageEntity.TryCreate(productId: productId, imageFileName: uniqueFileName, displayOrder: imageCount, out var entity);

        var isAdded = await productImageRepository.AddAsync(entity);

        return isAdded ? Ok(imageUrl) : Conflict();
    }

    /// <summary>
    /// Updates display order of the product image.
    /// </summary>
    /// <param name="productImageId">The productImageId of the product image to update.</param>
    /// <param name="dto">The product image to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the product image is null or invalid.</response>
    /// <response code="404">If the product image is not found.</response>
    /// <response code="409">If there was a conflict while updating the product image.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{productImageId:guid}")]
    public async Task<IActionResult> UpdateImageOrder([FromRoute] [NonZeroGuid] Guid productImageId, [FromBody] ProductImageUpdateOrderDto dto)
    {
        if (dto.DisplayOrder < 0)
            return BadRequest();

        var entity = await productImageRepository.GetByIdAsync(productImageId);
        if (entity == null)
            return NotFound(nameof(productImageId));

        if (entity.DisplayOrder == dto.DisplayOrder)
            return Ok();

        var allImages = await productImageRepository.GetAllByProductIdAsync(entity.ProductId);
        allImages.Remove(entity);

        var orderedImages = allImages.OrderBy(x => x.DisplayOrder).ToList();
        orderedImages.Insert(Math.Min(dto.DisplayOrder, orderedImages.Count), entity);
        for (var i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].Update(i);
        }

        var isUpdated = await productImageRepository.BatchUpdateAsync(orderedImages);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes a specific product image.
    /// </summary>
    /// <param name="productImageId">The productImageId of the product image to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the product image is not found.</response>
    /// <response code="409">If there was a conflict while deleting the product image from db or deleting it from the blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{productImageId:guid}")]
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid productImageId)
    {
        var productImageEntity = await productImageRepository.GetByIdAsync(productImageId);
        if (productImageEntity == null)
            return NotFound(nameof(productImageId));

        var isDeleted = await blobService.DeleteFileAsync(BucketName, productImageEntity.ImageFileName);
        if (!isDeleted)
            return Conflict();

        var isRemoved = await productImageRepository.RemoveByIdAsync(productImageId);

        return isRemoved ? Ok() : Conflict();
    }
}