using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Repositories;

namespace UniversityEvents.Web.Controllers;

public class CategoryController(ICategoryRepository categoryRepository) : Controller
{
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        var pagination = await categoryRepository.GetCategoriesAsync(search, page, pageSize);
        return View(pagination);
    }
}
