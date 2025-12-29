using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

[Authorize]
[Route("Category")]
[Authorize(Roles = AppRoles.Administrator + "," + AppRoles.Manager)]
public class CategoryController(ICategoryRepository categoryRepository, IAppLogger<CategoryController> logger) : Controller
{
    // GET: Category
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
            #if DEBUG
            logger.LogInfo($"Start Watch");
            var stopwatch = Stopwatch.StartNew();
            #endif

            logger.LogInfo($"Fetching categories. Search={search}, Page={page}, PageSize={pageSize}");

            // Always fetch fresh data (real-time)
            var pagination = await categoryRepository.GetCategoriesAsync(filter, HttpContext.RequestAborted);

            #if DEBUG
            logger.LogInfo($"GetUserData took {stopwatch.ElapsedMilliseconds}ms");
            #endif

            logger.LogInfo($"Fetched {pagination.Items.Count()} categories");
            return View(pagination);
        }
        catch (Exception ex)
        {
            logger.LogError("Error while fetching categories", ex);
            return StatusCode(500, "An error occurred while fetching categories.");
        }
    }

    [HttpGet]
    [Route("category/createoredit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(long Id = 0)
    {
        try
        {
            if (Id > 0)
            {
                logger.LogInfo($"Editing Category Id={Id}");
                var categoryVm = await categoryRepository.GetCategoryByIdAsync(Id, CancellationToken.None);

                if (categoryVm == null)
                {
                    TempData["AlertMessage"] = $"Category with Id {Id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(categoryVm);
            }

            return View(new CategoryVm());
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in CreateOrEdit for Id={Id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("category/createoredit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(CategoryVm categoryVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(categoryVm);
        }

        try
        {
            var result = await categoryRepository.CreateOrUpdateCategoryAsync(categoryVm, HttpContext.RequestAborted);

            if (result == null)
            {
                TempData["AlertMessage"] = $"Category with Id {categoryVm.Id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = categoryVm.Id > 0
                ? "Category updated successfully!"
                : "Category created successfully!";
            TempData["AlertType"] = "Success";

            // Redirect ensures Index fetches fresh data
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError("Error while creating/updating category", ex);
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
                return NotFound();
            }

            TempData["AlertMessage"] = "Category deleted successfully!";
            TempData["AlertType"] = "Success";

            // Redirect ensures Index fetches fresh data
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
