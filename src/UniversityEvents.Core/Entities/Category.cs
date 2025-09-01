using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class Category: AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; }
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
