using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.Mappers.Abstractions;
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
    private readonly ILogger<ProductController> logger;
    private readonly IProductRepository productRepository;
    private readonly IProductImageRepository productImageRepository;
    private readonly IBrandRepository brandRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IBlobService blobService;
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;
    private readonly IProductMapper productMapper;
    private readonly IProductImageMapper productImageMapper;

    public ProductController(ILogger<ProductController> logger, IProductRepository productRepository,
        IProductImageRepository productImageRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository,
        IBlobService blobService, IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings,
        IProductMapper productMapper, IProductImageMapper productImageMapper)
    {
        this.logger = logger;
        this.productRepository = productRepository;
        this.productImageRepository = productImageRepository;
        this.brandRepository = brandRepository;
        this.categoryRepository = categoryRepository;
        this.blobService = blobService;
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
        this.productMapper = productMapper;
        this.productImageMapper = productImageMapper;
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <response code="200">Products successfully retrieved, returns a list of all products.</response>
    [ProducesResponseType(typeof(List<ProductReadDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        logger.LogInformation("Retrieving all products...");
        var productEntities = await productRepository.GetAllAsync();

        var productDtos = productEntities.Select(productMapper.MapToDto).ToList();

        logger.LogInformation("All products retrieved successfully");
        return Ok(productDtos);
    }

    /// <summary>
    /// Retrieves a specific product by its ID.
    /// </summary>
    /// <param name="productId">ID of the desired product.</param>
    /// <response code="200">Product found and returned successfully.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetProduct([FromRoute] [NonZeroGuid] Guid productId)
    {
        logger.LogInformation("Retrieving product with ID {ProductId}...", productId);

        var productEntity = await productRepository.GetByIdAsync(productId);

        if (productEntity == null)
        {
            logger.LogInformation("Product with ID {ProductId} not found", productId);
            return NotFound(nameof(productId));
        }

        var productDto = productMapper.MapToDto(productEntity);

        logger.LogInformation("Product with ID {ProductId} retrieved successfully", productId);
        return Ok(productDto);
    }

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="dto">Object containing the details of the new product.</param>
    /// <response code="200">Product created successfully, returns the ID of created product.</response>
    /// <response code="400">Invalid product data or product data is null.</response>
    /// <response code="404">Brand, or category with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while updating the product.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductWriteDto dto)
    {
        logger.LogInformation("Adding a new product...");

        // using var transaction = productRepository.BeginTransaction();

        if (dto.BrandId != null && !await brandRepository.ExistsAsync(dto.BrandId.Value))
        {
            logger.LogInformation("Brand with ID {BrandId} not found", dto.BrandId.Value);
            return NotFound(nameof(dto.BrandId));
        }

        if (dto.CategoryId != null && !await categoryRepository.ExistsAsync(dto.CategoryId.Value))
        {
            logger.LogInformation("Category with ID {CategoryId} not found", dto.CategoryId.Value);
            return NotFound(nameof(dto.CategoryId));
        }

        var (validationResult, productEntity) = productMapper.MapToEntity(dto);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid product data");
            return BadRequest(validationResult);
        }

        var isAdded = await productRepository.AddAsync(productEntity);

        if (isAdded)
        {
            logger.LogInformation("Product added successfully with ID {ProductId}", productEntity.Id);
            // transaction.Commit();
            return Ok(productEntity.Id);
        }
        else
        {
            logger.LogError("Conflict occurred while adding the product in db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Updating product with ID {ProductId}...", productId);

        // using var transaction = productRepository.BeginTransaction();

        if (dto.BrandId != null && !await brandRepository.ExistsAsync(dto.BrandId.Value))
        {
            logger.LogInformation("Brand with ID {BrandId} not found", dto.BrandId.Value);
            return NotFound(nameof(dto.BrandId));
        }

        if (dto.CategoryId != null && !await categoryRepository.ExistsAsync(dto.CategoryId.Value))
        {
            logger.LogInformation("Category with ID {CategoryId} not found", dto.CategoryId.Value);
            return NotFound(nameof(dto.CategoryId));
        }

        var productEntity = await productRepository.GetByIdAsync(productId);

        if (productEntity == null)
        {
            logger.LogInformation("Product with ID {ProductId} not found", productId);
            return NotFound(nameof(productId));
        }

        var validationResult = productEntity.Update(dto.BrandId, dto.CategoryId, dto.Name, dto.Description,
            dto.Price, dto.Discount, dto.SKU, dto.Stock, dto.Availability);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid product data");
            return BadRequest(validationResult);
        }

        var isUpdated = await productRepository.UpdateAsync(productEntity);

        if (isUpdated)
        {
            logger.LogInformation("Product with ID {ProductId} updated successfully", productId);
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while updating the product in db");
            // transaction.Rollback();
            return Conflict();
        }
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
        logger.LogInformation("Deleting product with ID {ProductId}...", productId);

        // using var transaction = productRepository.BeginTransaction();

        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        // var productEntity = await productRepository.GetByIdAsync(productId);
        // // TODO: add
        // if (productEntity == null)
        //     return NotFound(nameof(productId));

        var isDeleted = await productRepository.RemoveByIdAsync(productId);

        if (isDeleted)
        {
            logger.LogInformation("Product with ID {ProductId} deleted successfully", productId);
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the product from db");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Retrieves all images for a specified product.
    /// </summary>
    /// <param name="productId">ID of the product.</param>
    /// <response code="200">Images successfully retrieved, returns a list of all product images.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    [ProducesResponseType(typeof(List<ProductImageReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}/images")]
    public async Task<IActionResult> GetProductImages([FromRoute] [NonZeroGuid] Guid productId)
    {
        logger.LogInformation("Retrieving all images for product with ID {ProductId}...", productId);

        if (!await productRepository.ExistsAsync(productId))
        {
            logger.LogInformation("Product with ID {ProductId} not found", productId);
            return NotFound(nameof(productId));
        }

        var productImageEntities = await productImageRepository.GetAllByProductIdAsync(productId);

        var productImageDtos = productImageEntities.Select(productImageMapper.MapToDto).ToList();

        logger.LogInformation("All images for product with ID {ProductId} retrieved successfully", productId);
        return Ok(productImageDtos);
    }

    /// <summary>
    /// Adds a new image to the specified product.
    /// </summary>
    /// <param name="productId">ID of the product.</param>
    /// <param name="dto">Object containing the new image details.</param>
    /// <response code="200">Image added successfully, returns the ID of created Image.</response>
    /// <response code="400">Invalid image data, image data is null, or maximum limit of images has been reached.</response>
    /// <response code="404">Product with the given ID does not exist.</response>
    /// <response code="409">Conflict occurred while adding the product image to the database or adding it to the blob storage.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromForm] ProductImageCreateDto dto)
    {
        logger.LogInformation("Adding a new image to product with ID {ProductId}...", productId);

        var validationResult = await new ProductImageFileValidator().ValidateAsync(dto.ImageFile);
        if (!validationResult.IsValid)
        {
            logger.LogInformation("Invalid image data");
            return BadRequest(validationResult);
        }

        // using var transaction = productImageRepository.BeginTransaction();

        if (!await productRepository.ExistsAsync(productId))
        {
            logger.LogInformation("Product with ID {ProductId} not found", productId);
            return NotFound(nameof(productId));
        }

        var imageCount = await productImageRepository.GetProductImageCount(productId);
        if (imageCount > imageHandlingSettings.MaxProductImages)
        {
            logger.LogInformation("The maximum number of images {MaxProductImages} for product with ID {ProductId} has been reached",
                imageHandlingSettings.MaxProductImages, productId);

            return BadRequest(nameof(imageCount), $"The maximum number of images {imageHandlingSettings.MaxProductImages} for this product has been reached.");
        }

        var uniqueFileName = BlobService.GenerateUniqueFileName(dto.ImageFile);

        var imageUrl = $"{blobServiceSettings.Endpoint}/{blobServiceSettings.ProductImageBucketName}/{uniqueFileName}";

        var (_, entity) = productImageMapper.MapToEntity(productId, uniqueFileName, imageCount);

        var isAdded = await productImageRepository.AddAsync(entity);

        if (isAdded)
        {
            await blobService.UploadFileAsync(blobServiceSettings.ProductImageBucketName, uniqueFileName, dto.ImageFile);

            logger.LogInformation("Image added successfully to product with ID {ProductId}", productId);
            // transaction.Commit();
            return Ok(entity.Id); // TODO: should i return link or id? or both?
        }
        else
        {
            logger.LogError("Conflict occurred while adding the product image to the database or adding it to the blob storage");
            // transaction.Rollback();
            return Conflict();
        }
    }

    /// <summary>
    /// Retrieves a specific image by its ID.
    /// </summary>
    /// <param name="productId">ID of the product to which the image belongs.</param>
    /// <param name="productImageId">ID of the desired product image.</param>
    /// <response code="200">Image found and returned successfully.</response>
    /// <response code="400">Provided product ID does not match with the product ID associated with the image.</response>
    /// <response code="404">Product image with the given ID does not exist.</response>
    [ProducesResponseType(typeof(ProductImageReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{productId:guid}/images/{productImageId:guid}")]
    public async Task<IActionResult> GetImage([FromRoute] [NonZeroGuid] Guid productId, [FromRoute] [NonZeroGuid] Guid productImageId)
    {
        logger.LogInformation("Retrieving image with ID {ProductImageId} for product with ID {ProductId}...", productImageId, productId);

        var productImageEntity = await productImageRepository.GetByIdAsync(productImageId);
        if (productImageEntity == null)
        {
            logger.LogInformation("Image with ID {ProductImageId} not found", productImageId);
            return NotFound(nameof(productImageId));
        }

        if (productImageEntity.ProductId != productId)
        {
            logger.LogInformation("The product ID provided does not match the product ID associated with the image");
            return BadRequest("The product ID provided does not match the product ID associated with the image.");
        }

        var productImageDto = productImageMapper.MapToDto(productImageEntity);

        logger.LogInformation("Image with ID {ProductImageId} for product with ID {ProductId} retrieved successfully", productImageId, productId);
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
        logger.LogInformation("Updating image with ID {ProductImageId} for product with ID {ProductId}...", productImageId, productId);

        if (dto.DisplayOrder < 0)
        {
            logger.LogInformation("Invalid display order provided.");
            return BadRequest();
        }

        // using var transaction = productImageRepository.BeginTransaction();

        var entity = await productImageRepository.GetByIdAsync(productImageId);
        if (entity == null)
        {
            logger.LogInformation("Image with ID {ProductImageId} not found", productImageId);
            return NotFound(nameof(productImageId));
        }

        if (entity.ProductId != productId)
        {
            logger.LogInformation("The product ID provided does not match the product ID associated with the image");
            return BadRequest("The product ID provided does not match the product ID associated with the image.");
        }

        if (entity.DisplayOrder == dto.DisplayOrder)
        {
            logger.LogInformation("The provided display order matches the current one, no updates required");
            return Ok();
        }

        var allImages = await productImageRepository.GetAllByProductIdAsync(entity.ProductId);
        allImages.RemoveAll(x => x.Id == entity.Id);

        var orderedImages = allImages.OrderBy(x => x.DisplayOrder).ToList();
        orderedImages.Insert(Math.Min(dto.DisplayOrder, orderedImages.Count) - 1, entity);
        for (var i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].Update(i);
        }

        var isUpdated = await productImageRepository.BatchUpdateAsync(orderedImages);

        if (isUpdated)
        {
            // transaction.Commit();
            logger.LogInformation("Image with ID {ProductImageId} for product with ID {ProductId} updated successfully", productImageId, productId);
            return Ok();
        }
        else
        {
            // transaction.Rollback();
            logger.LogError("Conflict occurred while updating the image data");
            return Conflict();
        }
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
        logger.LogInformation("Deleting image with ID {ProductImageId} from product with ID {ProductId}...", productImageId, productId);

        // using var transaction = productImageRepository.BeginTransaction();

        var productImageEntity = await productImageRepository.GetByIdAsync(productImageId);
        if (productImageEntity == null)
        {
            logger.LogInformation("Image with ID {ProductImageId} not found", productImageId);
            return NotFound(nameof(productImageId));
        }

        if (productImageEntity.ProductId != productId)
        {
            logger.LogInformation("The product ID provided does not match the product ID associated with the image");
            return BadRequest("The product ID provided does not match the product ID associated with the image.");
        }

        var allImages = await productImageRepository.GetAllByProductIdAsync(productImageEntity.ProductId);
        allImages.RemoveAll(x => x.Id == productImageEntity.Id);

        var orderedImages = allImages.OrderBy(x => x.DisplayOrder).ToList();
        for (var i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].Update(i);
        }

        var isRemoved = await productImageRepository.RemoveByIdAsync(productImageId);
        var isUpdated = await productImageRepository.BatchUpdateAsync(orderedImages);

        if (isRemoved && isUpdated)
        {
            await blobService.DeleteFileAsync(blobServiceSettings.ProductImageBucketName, productImageEntity.ImageFileName);

            logger.LogInformation("Image with ID {ProductImageId} from product with ID {ProductId} deleted successfully", productImageId, productId);
            // transaction.Commit();
            return Ok();
        }
        else
        {
            logger.LogError("Conflict occurred while deleting the product image from db or deleting it from the blob storage");
            // transaction.Rollback();
            return Conflict();
        }
    }
}