using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.Services;
using Catalog.Api.Services.Abstractions;
using Catalog.Api.ValidationAttributes;
using Catalog.Api.Validators;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1;

/// <summary>
/// Controller responsible for Product entity and Product Image entity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ApiControllerBase<ProductController>, IProductController
{
    private readonly IProductRepository productRepository;
    private readonly IProductImageRepository productImageRepository;
    private readonly IBrandRepository brandRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;

    public ProductController(IProductRepository productRepository, IProductImageRepository productImageRepository,
        IBrandRepository brandRepository, ICategoryRepository categoryRepository, IBlobService blobService,
        IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings)
    {
        this.productRepository = productRepository;
        this.productImageRepository = productImageRepository;
        this.brandRepository = brandRepository;
        this.categoryRepository = categoryRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <response code="200">Products successfully retrieved, returns a list of all products.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var productEntities = await productRepository.GetAllAsync();

        var productDtos = productEntities.Select(productEntity => ProductMapper.MapToReadDto(productEntity));

        return Ok(productDtos);
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="productId">ID of the desired product.</param>
    /// <response code="200">Product found and returned successfully.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
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
    /// Adds a new product.
    /// </summary>
    /// <param name="dto">Object containing the details of the new product.</param>
    /// <response code="200">Product created successfully.</response>
    /// <response code="400">Invalid product data or product data is null.</response>
    /// <response code="404">Brand, or category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the product.</response>
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
    /// Updates an existing product.
    /// </summary>
    /// <param name="productId">ID of the product to update.</param>
    /// <param name="dto">Object containing the updated details of the product.</param>
    /// <response code="200">Product updated successfully.</response>
    /// <response code="400">Invalid product data or product data is null.</response>
    /// <response code="404">Product, brand, or category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the product.</response>
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
    /// Deletes an existing product.
    /// </summary>
    /// <param name="productId">ID of the product to delete.</param>
    /// <response code="200">Product deleted successfully.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while deleting the product.</response>
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
    /// Retrieves all images for a specified product.
    /// </summary>
    /// <param name="productId">ID of the product.</param>
    /// <response code="200">Images successfully retrieved, returns a list of all product images.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetProductImages([FromRoute] [NonZeroGuid] Guid productId)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var productImageEntities = await productImageRepository.GetAllByProductIdAsync(productId);

        var productImageDtos = productImageEntities.Select(productImageEntity => ProductImageMapper.MapToReadDto(productImageEntity));

        return Ok(productImageDtos);
    }

    /// <summary>
    /// Adds a new image to the specified product.
    /// </summary>
    /// <param name="productId">ID of the product.</param>
    /// <param name="dto">Object containing the new image details.</param>
    /// <response code="200">Image added successfully, returns the URL of the added image.</response>
    /// <response code="400">Invalid image data, image data is null, or maximum limit of images has been reached.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while adding the product image to the database or adding it to the blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromForm] ProductImageCreateDto dto)
    {
        var validationResult = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var imageCount = await productImageRepository.GetProductImageCount(productId);
        if (imageCount > imageHandlingSettings.MaxProductImages)
            return BadRequest(nameof(imageCount), $"The maximum number of images {imageHandlingSettings.MaxProductImages} for this product has been reached.");

        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);
        await blobService.UploadFileAsync(blobServiceSettings.ProductImageBucketName, uniqueFileName, dto.ImageFile);

        var imageUrl = $"{blobServiceSettings.Endpoint}/{blobServiceSettings.ProductImageBucketName}/{uniqueFileName}";

        // No need for validation check as it always valid at this point
        _ = ProductImageEntity.TryCreate(productId: productId, imageFileName: uniqueFileName, displayOrder: imageCount, out var entity);

        var isAdded = await productImageRepository.AddAsync(entity);

        return isAdded ? Ok(imageUrl) : Conflict();
    }

    /// <summary>
    /// Retrieves a specific image by its ID.
    /// </summary>
    /// <param name="productId">ID of the product to which the image belongs.</param>
    /// <param name="productImageId">ID of the desired product image.</param>
    /// <response code="200">Image found and returned successfully.</response>
    /// <response code="400">Provided product ID does not match with the product ID associated with the image.</response>
    /// <response code="404">Product image with the given ID does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}/images/{productImageId:guid}")]
    public async Task<IActionResult> GetImage([FromRoute] [NonZeroGuid] Guid productId, [FromRoute] [NonZeroGuid] Guid productImageId)
    {
        var productImageEntity = await productImageRepository.GetByIdAsync(productImageId);
        if (productImageEntity == null)
            return NotFound(nameof(productImageId));

        if (productImageEntity.ProductId != productId)
            return BadRequest("The product ID provided does not match the product ID associated with the image.");

        var productImageDto = ProductImageMapper.MapToReadDto(productImageEntity);

        return Ok(productImageDto);
    }

    /// <summary>
    /// Updates an existing product image.
    /// </summary>
    /// <param name="productId">ID of the product to which the image belongs.</param>
    /// <param name="productImageId">ID of the product image to update.</param>
    /// <param name="dto">Object containing the updated details of the product image.</param>
    /// <response code="200">Product image updated successfully.</response>
    /// <response code="400">Invalid product image data, product image data is null
    /// or provided product ID does not match with the product ID associated with the image.</response>
    /// <response code="404">Product image with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the product image.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{productId:guid}/images/{productImageId:guid}")]
    public async Task<IActionResult> UpdateImage([FromRoute] [NonZeroGuid] Guid productId, [FromRoute] [NonZeroGuid] Guid productImageId,
        [FromBody] ProductImageUpdateDto dto)
    {
        if (dto.DisplayOrder < 0)
            return BadRequest();

        var entity = await productImageRepository.GetByIdAsync(productImageId);
        if (entity == null)
            return NotFound(nameof(productImageId));

        if (entity.ProductId != productId)
            return BadRequest("The product ID provided does not match the product ID associated with the image.");

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
    /// Deletes an existing product image.
    /// </summary>
    /// <param name="productId">ID of the product to which the image belongs.</param>
    /// <param name="productImageId">ID of the product image to delete.</param>
    /// <response code="200">Product image deleted successfully.</response>
    /// <response code="400">Provided product ID does not match with the product ID associated with the image.</response>
    /// <response code="404">Product image with the given ID does not exist</response>
    /// <response code="409">Conflict occurred while deleting the product image from db or deleting it from the blob storage.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{productId:guid}/images/{productImageId:guid}")]
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromRoute] [NonZeroGuid] Guid productImageId)
    {
        var productImageEntity = await productImageRepository.GetByIdAsync(productImageId);
        if (productImageEntity == null)
            return NotFound(nameof(productImageId));

        if (productImageEntity.ProductId != productId)
            return BadRequest("The product ID provided does not match the product ID associated with the image.");

        await blobService.DeleteFileAsync(blobServiceSettings.ProductImageBucketName, productImageEntity.ImageFileName);

        var isRemoved = await productImageRepository.RemoveByIdAsync(productImageId);

        return isRemoved ? Ok() : Conflict();
    }
}