namespace UniversityEvents.Application.Filters;

public class Filter
{
    public string Search { get; set; }
    public bool IsDelete { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
