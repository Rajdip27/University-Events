using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityEvents.Core.Entities;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Application.ViewModel;

public class StudentRegistrationVm:BaseEntity
{
    public long EventId { get; set; }
    public Event Event { get; set; }
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string IdCardNumber { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string PhotoPath { get; set; } = default!;
    // PaymentStatus can be "Pending", "Completed", "Failed"
    public string PaymentStatus { get; set; } = "Pending";
    [NotMapped]
    public IFormFile ImageFile {  get; set; }
}
