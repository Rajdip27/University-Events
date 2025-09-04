namespace UniversityEvents.Application.Filters;

public class Filter
{
    public string Search { get; set; }
    public bool IsDelete { get; set; }
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 10;
}
