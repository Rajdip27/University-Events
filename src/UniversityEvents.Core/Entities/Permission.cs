using UniversityEvents.Core.Entities.BaseEntities;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Core.Entities;

public class Permission:AuditableEntity
{
    public long RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public long MenuId { get; set; }
    public Menu Menu { get; set; } = null!;

    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
    public bool CanApprove { get; set; } = false;
}
