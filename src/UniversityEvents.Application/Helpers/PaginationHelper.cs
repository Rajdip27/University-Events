using UniversityEvents.Application.CommonModel;

namespace UniversityEvents.Application.Helpers;

public static class PaginationHelper
{
    public static List<object> GeneratePageList<T>(PaginationModel<T> model, int pageNeighbours = 1)
    {
        var pages = new List<object>();
        int totalPages = model.TotalPages;
        int currentPage = model.PageNumber;

        if (totalPages == 0) return pages;

        pages.Add(1); // first page

        int start = Math.Max(2, currentPage - pageNeighbours);
        int end = Math.Min(totalPages - 1, currentPage + pageNeighbours);

        if (start > 2) pages.Add("...");

        for (int i = start; i <= end; i++)
            pages.Add(i);

        if (end < totalPages - 1) pages.Add("...");

        if (totalPages > 1) pages.Add(totalPages); // last page

        return pages;
    }
}
