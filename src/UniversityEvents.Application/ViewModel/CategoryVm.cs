using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Application.ViewModel;

public class CategoryVm: BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; }
}
