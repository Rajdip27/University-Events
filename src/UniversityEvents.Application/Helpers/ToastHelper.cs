using Microsoft.AspNetCore.Mvc.RazorPages;
using UniversityEvents.Application.Enums;

namespace UniversityEvents.Application.Helpers;

public static class ToastHelper
{
    public static void ShowToast(this PageModel page, string message, AlertType type)
    {
        page.TempData["AlertMessage"] = message;
        page.TempData["AlertType"] = type.ToString();
    }
}
