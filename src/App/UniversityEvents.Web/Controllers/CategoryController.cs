using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.Logging;

namespace UniversityEvents.Web.Controllers;

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
            var pagination = await categoryRepository.GetCategoriesAsync(filter);
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
    public async Task<IActionResult> CreateOrEdit()
    {
        logger.LogInfo("Opening CreateOrEdit view for Category");
        return View();
    }
}
