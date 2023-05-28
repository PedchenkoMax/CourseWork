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
/// Controller responsible for ProductImage entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/product-images")]
public class ProductImageController : ApiControllerBase<ProductImageController>, IProductImageController
{
    private const string BucketName = "your-bucket-name";
    private const string BlobAccess = "localhost:9000";
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;
    private readonly IBlobService blobService;

    public ProductImageController(IProductImageRepository productImageRepository, IProductRepository productRepository,
        IBlobService blobService)
    {
        this.productImageRepository = productImageRepository;
        this.productRepository = productRepository;
        this.blobService = blobService;
    }

    /// <summary>
    /// Gets all product images by product id.
    /// </summary>
    /// <param name="productId">The id of the product.</param>
    /// <response code="200">Returns the list of product images.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet("{productId:guid}/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllProductImagesByProductId([FromRoute] [NonZeroGuid] Guid productId)
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
    /// <param name="productId">The id of the product.</param>
    /// <param name="productImageDto">The product image to add.</param>
    /// <response code="200">Returns the URL to the added product image.</response>
    /// <response code="400">If the product image is null, invalid, or the maximum limit of images has been reached.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="409">If there was a conflict while adding the product image to db or adding it to the blob storage.</response>
    [HttpPost("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromForm] ProductImageCreateDto productImageDto)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var validationResult = await new ProductImageFileValidator().ValidateAsync(productImageDto.ImageFile);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        const int maxCount = 10;
        var imageCount = await productImageRepository.GetProductImageCount(productId);
        if (imageCount > maxCount)
            return BadRequest(nameof(imageCount), $"The maximum number of images {maxCount} for this product has been reached.");

        var uniqueFileName = await blobService.UploadFileAsync(BucketName, productImageDto.ImageFile);
        if (uniqueFileName == null)
            return Conflict();

        var imageUrl = $"{BlobAccess}/{BucketName}/{uniqueFileName}";

        // No need for validation check as it always valid at this point
        _ = ProductImageEntity.TryCreate(productId: productId, imageUrl: uniqueFileName, displayOrder: imageCount, out var entity);

        var isAdded = await productImageRepository.AddAsync(entity);

        return isAdded ? Ok(imageUrl) : Conflict();
    }

    /// <summary>
    /// Updates display order of the product image.
    /// </summary>
    /// <param name="id">The id of the product image to update.</param>
    /// <param name="productImageDto">The product image to update.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the product image is null or invalid.</response>
    /// <response code="404">If the product image is not found.</response>
    /// <response code="409">If there was a conflict while updating the product image.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProductImageOrder([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductImageUpdateOrderDto productImageDto)
    {
        if (productImageDto.DisplayOrder < 0)
            return BadRequest();

        var entity = await productImageRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound(nameof(id));

        if (entity.DisplayOrder == productImageDto.DisplayOrder)
            return Ok();

        var allImages = await productImageRepository.GetAllByProductIdAsync(entity.ProductId);
        allImages.Remove(entity);

        var orderedImages = allImages.OrderBy(x => x.DisplayOrder).ToList();
        orderedImages.Insert(Math.Min(productImageDto.DisplayOrder, orderedImages.Count), entity);
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
    /// <param name="id">The id of the product image to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the product image is not found.</response>
    /// <response code="409">If there was a conflict while deleting the product image from db or deleting it from the blob storage.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid id)
    {
        var productImageEntity = await productImageRepository.GetByIdAsync(id);
        if (productImageEntity == null)
            return NotFound(nameof(id));

        var isDeleted = await blobService.DeleteFileAsync(BucketName, productImageEntity.ImageUrl);
        if (!isDeleted)
            return Conflict();

        var isRemoved = await productImageRepository.RemoveByIdAsync(id);

        return isRemoved ? Ok() : Conflict();
    }
}