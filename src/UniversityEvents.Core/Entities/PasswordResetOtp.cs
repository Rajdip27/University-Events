using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class PasswordResetOtp:AuditableEntity
{
    public long UserId { get; set; } 
    public string Otp { get; set; } = null!;
    public DateTimeOffset ExpireAt { get; set; }
    public bool IsUsed { get; set; }
}
