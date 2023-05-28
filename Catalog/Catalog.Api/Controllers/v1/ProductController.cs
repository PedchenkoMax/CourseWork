using Catalog.Api.Controllers.v1.Abstractions;
using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Api.ValidationAttributes;
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
    private readonly IProductRepository productRepository;
    private readonly IBrandRepository brandRepository;
    private readonly ICategoryRepository categoryRepository;

    public ProductController(IProductRepository productRepository, IBrandRepository brandRepository,
        ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.brandRepository = brandRepository;
        this.categoryRepository = categoryRepository;
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
}