using Catalog.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.v1.Abstractions;

public interface ICategoryController
{
    Task<IActionResult> GetAllCategories();
    Task<IActionResult> GetCategorySubcategories(Guid categoryId);
    Task<IActionResult> GetCategory(Guid categoryId);
    Task<IActionResult> AddCategory(CategoryWriteDto dto);
    Task<IActionResult> UpdateCategory(Guid categoryId, CategoryWriteDto dto);
    Task<IActionResult> DeleteCategory(Guid categoryId);
    Task<IActionResult> UpdateCategoryImage(Guid categoryId, CategoryImageUpdateDto dto);
    Task<IActionResult> DeleteCategoryImage(Guid categoryId);
}