using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Application.ViewModel;


public class StudentRegistrationVm : BaseEntity
{
    public long EventId { get; set; }
    public EventVm Event { get; set; } 

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = default!;

    [Required(ErrorMessage = "Phone Number is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Phone number must be 10-15 digits and can start with +.")]
    public string PhoneNumber { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Student ID is required.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Student ID must be between 3 and 20 characters.")]
    public string IdCardNumber { get; set; } = default!;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
    public string Department { get; set; } = default!;

    [StringLength(250)]
    public string PhotoPath { get; set; } = default!;
    public string PaymentStatus { get; set; } = "Pending";

    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
