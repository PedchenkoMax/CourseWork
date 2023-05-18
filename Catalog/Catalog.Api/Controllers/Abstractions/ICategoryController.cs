using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.Abstractions;

public interface ICategoryController
{
    Task<IActionResult> GetAllCategories();
    Task<IActionResult> GetCategoryById(Guid id);
    Task<IActionResult> AddCategory(Category categoryDto);
    Task<IActionResult> UpdateCategory(Guid id, Category categoryDto);
    Task<IActionResult> DeleteCategory(Guid id);
}