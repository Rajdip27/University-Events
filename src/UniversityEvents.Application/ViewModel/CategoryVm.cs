using System.ComponentModel.DataAnnotations;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Application.ViewModel;

public class CategoryVm: BaseEntity
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = default!;
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }
}
