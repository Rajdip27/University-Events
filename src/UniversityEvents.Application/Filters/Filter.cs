namespace UniversityEvents.Application.Filters;

public class Filter
{
    public string Search { get; set; }
    public bool IsDelete { get; set; }=false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public long UserId { get; set; } 
    
    public long EventId { get; set; }
    public long StudentId { get; set; }
    public string Status { get; set; }
}
