using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Repositories;

namespace UniversityEvents.Web.Controllers;

public class CategoryController(ICategoryRepository categoryRepository) : Controller
{
    public async Task<IActionResult> Index(string? search, int page, int pageSize)
    {
        var filter = new Filter()
        {
            Search = search,
            IsDelete = false,
            Page = page,
            PageSize = pageSize
        };
        var pagination = await categoryRepository.GetCategoriesAsync(filter);
        return View(pagination);
    }
    [HttpGet]
    public async Task<IActionResult> CreateOrEdit()
    {
        return View();
    }

}
