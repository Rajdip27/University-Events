using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Application.ViewModel;

public class StudentRegistrationVm : BaseEntity
{
    public long EventId { get; set; }
    public EventVm Event { get; set; } = new EventVm();

    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string IdCardNumber { get; set; } = default!;

    [Required]
    public string Department { get; set; } = default!;

    public string PhotoPath { get; set; } = default!;

    // PaymentStatus can be "Pending", "Completed", "Failed"

    public string PaymentStatus { get; set; } = "Pending";

    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
