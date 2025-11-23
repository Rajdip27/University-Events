using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Application.ModelSpecification;

public class EventSpecification: BaseSpecification<Event>
{
    public EventSpecification(Filter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            ApplyCriteria(p => p.Name.Contains(filter.Search));
        if (filter.IsDelete)
            ApplyCriteria(c => !c.IsDelete);

        ApplyOrderByDescending(c => c.Id);
    }
}
