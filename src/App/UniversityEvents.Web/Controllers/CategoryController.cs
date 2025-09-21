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
                IsDelete = true,
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
   
}
