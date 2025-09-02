using Microsoft.AspNetCore.Mvc;

namespace UniversityEvents.Application.CommonModel;

public class PaginationViewComponent :ViewComponent
{
    public IViewComponentResult Invoke(dynamic model)
    {
        return View(model);
    }
}
