using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Web.Models;

namespace UniversityEvents.Web.Controllers;

public class HomeController(ILogger<HomeController> logger, IEventRepository eventRepository) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    public async Task<IActionResult> Index()
    
    
    {
        var data= await eventRepository.GetAllAsync(x=>x.Category);
        return View(data);
    }
    public async Task<IActionResult> EventDetails(long id) 
    {
        var data = await eventRepository.GetByIdAsync(id,x => x.Category);
        return View(data);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult PageNotFound()
    {
        return View();
    }
    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpPost]
    public IActionResult SetTimeZone([FromBody] TimeZoneRequest request)
    {
        HttpContext.Session.SetString("UserTimeZone", request.TimeZone);
        return Ok();
    }

}
