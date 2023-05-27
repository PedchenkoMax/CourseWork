using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.ValidationAttributes;
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
    private readonly IProductImageRepository productImageRepository;
    private readonly IProductRepository productRepository;
    
    public ProductImageController(IProductImageRepository productImageRepository, IProductRepository productRepository)
    {
        this.productImageRepository = productImageRepository;
        this.productRepository = productRepository;
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
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="400">If the product image is null or invalid.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="409">If there was a conflict while adding the product image.</response>
    [HttpPost("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddProductImage([FromRoute] [NonZeroGuid] Guid productId, [FromBody] ProductImageWriteDto productImageDto)
    {
        if (!await productRepository.ExistsAsync(productId))
            return NotFound(nameof(productId));

        var validationResult = ProductImageMapper.TryCreateEntity(productImageDto, out var productImageEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isAdded = await productImageRepository.AddAsync(productImageEntity);

        return isAdded ? Ok() : Conflict();
    }

    /// <summary>
    /// Updates a specific product image.
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
    public async Task<IActionResult> UpdateProductImage([FromRoute] [NonZeroGuid] Guid id, [FromBody] ProductImageWriteDto productImageDto)
    {
        // TODO: only 'DisplayOrder' could be updated, how to resolve when user tries to change 'ProductId'?
        var productImageEntity = await productImageRepository.GetByIdAsync(id);

        if (productImageEntity == null)
            return NotFound(nameof(id));

        var validationResult = ProductImageMapper.TryUpdateEntity(productImageDto, productImageEntity);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var isUpdated = await productImageRepository.UpdateAsync(productImageEntity);

        return isUpdated ? Ok() : Conflict();
    }

    /// <summary>
    /// Deletes a specific product image.
    /// </summary>
    /// <param name="id">The id of the product image to delete.</param>
    /// <response code="200">Returns a confirmation of action.</response>
    /// <response code="404">If the product image is not found.</response>
    /// <response code="409">If there was a conflict while deleting the product image.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteProductImage([FromRoute] [NonZeroGuid] Guid id)
    {
        if (!await productImageRepository.ExistsAsync(id))
            return NotFound(nameof(id));

        var isDeleted = await productImageRepository.RemoveByIdAsync(id);

        return isDeleted ? Ok() : Conflict();
    }
}