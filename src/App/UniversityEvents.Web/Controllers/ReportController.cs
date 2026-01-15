using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.Services;
using UniversityEvents.Application.Services.Pdf;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Web.Controllers;

public class ReportController(IEventRepository _eventRepository, IRazorViewToStringRenderer _razorViewToStringRenderer, IPdfService _pdfService, IStudentRegistrationRepository studentRegistrationRepository) : Controller
{
    public async Task<IActionResult> EventRegistrationReport()
    {
        ViewData["Event"] = await _eventRepository.EventDropdown();
        ViewData["Student"] = await studentRegistrationRepository.StudentRegistrationDropdown();
        return View();
    }

    public async Task<IActionResult> EventRegistrationReportView(Filter filte)
    {
        var registrations = await studentRegistrationRepository.GetRegistrationsAsync(filte, CancellationToken.None);
        return View(registrations);
    }

    public async Task<IActionResult> EventRegistrationReportPdf(Filter filte)
    {
        try
        {
            // Example data
            var registrations = await studentRegistrationRepository.GetRegistrationsAsync(filte, CancellationToken.None);

            // Render Razor view to string
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("PdfTemplates/EventRegistrationReport", registrations);

            var pdfOptions = new PdfOptions
            {
                PageSize = "A4",
                Landscape = false,
                MarginTop = 10,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                ShowPageNumbers = false,
                ColorMode = true
            };

            var pdfBytes = _pdfService.GeneratePdf(htmlContent, pdfOptions);

            // Return PDF inline (open in browser)
            Response.Headers.Add("Content-Disposition", "inline; filename=DepartmentReport.pdf");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<IActionResult> PrintMealTokens()
    {
        ViewData["Event"] = await _eventRepository.EventDropdown();
        ViewData["Student"] = await studentRegistrationRepository.StudentRegistrationDropdown();
        return View();
    }

    public async Task<IActionResult> PrintMealTokensView(Filter filter)
    {
        var mealTokens = await _eventRepository.PrintMealTokens(filter);
        return View(mealTokens);
    }


    public async Task<IActionResult> PrintMealTokensViewPdf(Filter filte)
    {
        try
        {
            // Example data
            var registrations = await _eventRepository.PrintMealTokens(filte);

            // Render Razor view to string
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("PdfTemplates/PrintMealTokensPdf", registrations);

            var pdfOptions = new PdfOptions
            {
                PageSize = "A4",
                Landscape = false,
                MarginTop = 10,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                ShowPageNumbers = false
            };

            var pdfBytes = _pdfService.GeneratePdf(htmlContent, pdfOptions);

            // Return PDF inline (open in browser)
            Response.Headers.Add("Content-Disposition", "inline; filename=DepartmentReport.pdf");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }


    public async Task<IActionResult> EventMealTokensPdf(Filter filter)
    {
        try
        {
            // Example data
            var data = await studentRegistrationRepository.GetRegistrationsAsync(filter, CancellationToken.None);

            // Render Razor view to string
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("PdfTemplates/AllFoodTokenPdf", data);

            var pdfOptions = new PdfOptions
            {
                PageSize = "A4",
                Landscape = false,
                MarginTop = 10,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                ShowPageNumbers = false
            };

            var pdfBytes = _pdfService.GeneratePdf(htmlContent, pdfOptions);

            // Return PDF inline (open in browser)
            Response.Headers.Add("Content-Disposition", "inline; filename=DepartmentReport.pdf");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
