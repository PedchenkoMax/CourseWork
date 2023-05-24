using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface ICategoryController
{
    Task<IActionResult> GetAllCategories();
    Task<IActionResult> GetChildrenByParentCategoryId(Guid id);
    Task<IActionResult> GetCategoryById(Guid id);
    Task<IActionResult> AddCategory(CategoryWriteDto categoryDto);
    Task<IActionResult> UpdateCategory(Guid id, CategoryWriteDto categoryDto);
    Task<IActionResult> DeleteCategory(Guid id);
}