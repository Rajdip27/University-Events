using System.ComponentModel.DataAnnotations;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Core.Entities;
using UniversityEvents.Core.Entities.BaseEntities;
namespace UniversityEvents.Application.ViewModel;
public class EventVm:BaseEntity
{
    [Required(ErrorMessage = "Category is required.")]
    public long CategoryId { get; set; }

    public Category Category { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.DateTime)]
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTimeOffset EndDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Registration fee must be a positive number.")]
    public decimal RegistrationFee { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens.")]
    [StringLength(150, ErrorMessage = "Slug cannot exceed 150 characters.")]
    public string Slug { get; set; } = default!;
}
