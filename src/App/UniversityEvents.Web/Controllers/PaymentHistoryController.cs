using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;

namespace UniversityEvents.Web.Controllers;

[Route("PaymentHistory")]
[Authorize]
public class PaymentHistoryController(IPaymentHistoryRepository paymentHistoryRepository, IAppLogger<PaymentHistoryController> logger) : Controller
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
            logger.LogInfo($"Fetching categories. Search={search}, Page={page}, PageSize={pageSize}");
            // Always fetch fresh data (real-time)
            var pagination = await paymentHistoryRepository.GetPaymentHistoryAsync(filter, HttpContext.RequestAborted);
            logger.LogInfo($"Fetched {pagination.Items.Count()} Payment History");
            return View(pagination);
        }
        catch (Exception ex)
        {
            logger.LogError("Error while fetching Payment History", ex);
            return StatusCode(500, "An error occurred while fetching Payment History.");
        }
    }
}
