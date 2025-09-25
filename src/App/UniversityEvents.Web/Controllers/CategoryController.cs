using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

[Authorize]
[Route("Category")]
public class CategoryController(ICategoryRepository categoryRepository, IAppLogger<CategoryController> logger) : Controller
{

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        try
        {
            var filter = new Filter
            {
                Search = search,
                IsDelete = false,
                Page = page,
                PageSize = pageSize
            };
            logger.LogInfo($"Fetching categories. Search: {search}, Page: {page}, PageSize: {pageSize}");
            var pagination = await categoryRepository.GetCategoriesAsync(filter, HttpContext.RequestAborted);
            logger.LogInfo($"Fetched {pagination.Items.Count()} categories");
            return View(pagination);
        }
        catch (Exception ex)
        {
            logger.LogError("Error while fetching categories", ex);
            return StatusCode(500, "An error occurred while fetching categories");
        }
    }

    [HttpGet]
    [Route("category/createoredit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(long Id=0)
    {
        try
        {
            CategoryVm categoryVm = new CategoryVm();
            if (Id > 0)
            {
                logger.LogInfo($"Opening CreateOrEdit view for editing Category with Id {Id}");
                try
                {
                    categoryVm = await categoryRepository.GetCategoryByIdAsync(Id, CancellationToken.None);
                    logger.LogInfo($"Editing Category with Id {Id}");
                    return View(categoryVm);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error while fetching category with Id {Id}", ex);
                    return StatusCode(500, "An error occurred while fetching the category");
                }
            }
            else
            {
                logger.LogInfo("Opening CreateOrEdit view for new Category");
                return View(categoryVm);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error while fetching categories", ex);
            throw;
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("category/createoredit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(CategoryVm categoryVm)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid category model submitted");
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(categoryVm);
        }

        try
        {
            logger.LogInfo(categoryVm.Id > 0
                ? $"Attempting to update Category with Id {categoryVm.Id}"
                : "Attempting to create a new Category");

            var result = await categoryRepository.CreateOrUpdateCategoryAsync(categoryVm, HttpContext.RequestAborted);

            if (result == null)
            {
                logger.LogWarning($"Category with Id {categoryVm.Id} not found for update");
                TempData["AlertMessage"] = $"Category with Id {categoryVm.Id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            logger.LogInfo(categoryVm.Id > 0
                ? $"Category updated successfully. Id={result.Id}"
                : $"Category created successfully. Id={result.Id}");

            TempData["AlertMessage"] = categoryVm.Id > 0
                ? "Category updated successfully!"
                : "Category created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while creating or updating the category", ex);
            TempData["AlertMessage"] = "An error occurred while saving the category.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
    [HttpPost]
    [Route("category/delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var deleted = await categoryRepository.DeleteCategoryAsync(id, HttpContext.RequestAborted);
            if (!deleted)
            {
                TempData["AlertMessage"] = $"Category with Id {id} not found.";
                TempData["AlertType"] = "Error";
                logger.LogWarning($"Delete failed: Category Id={id} not found");
                return NotFound();
            }

            TempData["AlertMessage"] = "Category deleted successfully!";
            TempData["AlertType"] = "Success";
            logger.LogInfo($"Deleted Category Id={id}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while deleting category Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the category.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
