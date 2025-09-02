﻿using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class StudentRegistration: AuditableEntity
{
    public long EventId { get; set; }
    public Event Event { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string IdCardNumber { get; set; } = default!;
    public string Department { get; set; } = default!; 
    public string PhotoPath { get; set; } = default!;
    public Payment Payment { get; set; }
    public FoodToken FoodToken { get; set; }
}
