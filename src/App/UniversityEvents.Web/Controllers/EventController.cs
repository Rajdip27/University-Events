using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;

[Authorize]
public class EventController(IEventRepository _eventRepository , IAppLogger<EventController> _logger) : Controller
{
    // GET: Event
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
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
            _logger.LogInfo("Start Watch");
            var stopwatch = Stopwatch.StartNew();
            #endif
            _logger.LogInfo($"Fetching events. Search={search}, Page={page}, PageSize={pageSize}");
            var pagination = await _eventRepository.GetEventsAsync(filter, cancellationToken);
            #if DEBUG
            _logger.LogInfo($"GetEventsAsync took {stopwatch.ElapsedMilliseconds}ms");
            #endif
            _logger.LogInfo($"Fetched {pagination.Items.Count()} events");
            return View(pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching events", ex);
            return StatusCode(500, "An error occurred while fetching events.");
        }
    }

    [HttpGet]
    [Route("event/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            // Pass to view via ViewData
            ViewData["Categories"] = await _eventRepository.CategoryDropdown();

            if (id > 0)
            {
                _logger.LogInfo($"Editing Event Id={id}");
                var eventVm = await _eventRepository.GetEventByIdAsync(id, cancellationToken);

                if (eventVm == null)
                {
                    TempData["AlertMessage"] = $"Event with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return RedirectToAction(nameof(Index));
                }

                return View(eventVm);
            }

            return View(new EventVm());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("event/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(EventVm eventVm, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(eventVm);
        }
        try
        {
            var result = await _eventRepository.CreateOrUpdateEventAsync(eventVm, cancellationToken);

            if (result == null)
            {
                TempData["AlertMessage"] = $"Event with Id {eventVm.Id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = eventVm.Id > 0
                ? "Event updated successfully!"
                : "Event created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating event", ex);
            TempData["AlertMessage"] = "An error occurred while saving the event.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("event/delete/{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _eventRepository.DeleteEventAsync(id, cancellationToken);

            if (!deleted)
            {
                TempData["AlertMessage"] = $"Event with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = "Event deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting event Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the event.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

}
