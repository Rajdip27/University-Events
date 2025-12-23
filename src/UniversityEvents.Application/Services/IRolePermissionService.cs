namespace UniversityEvents.Application.Services;

public interface IRolePermissionService
{
    bool CanCreate(List<string> roles);
    bool CanEdit(List<string> roles);
    bool CanDelete(List<string> roles);
    bool CanView(List<string> roles);
}
public class RolePermissionService : IRolePermissionService
{
    // Define permissions for each role
    private readonly Dictionary<string, (bool Create, bool Edit, bool Delete, bool View)> _rules =
        new()
        {
          { "Administrator", (true, true, true, true) },
          { "Manager",       (true, true, false, true) },
          { "Student",       (true, false, false, true) }
        };


    private bool HasPermission(List<string> roles, Func<(bool Create, bool Edit, bool Delete, bool View), bool> selector)
    {
        if (roles == null || roles.Count == 0)
            return false;

        foreach (var role in roles)
        {
            if (_rules.TryGetValue(role, out var perm))
            {
                if (selector(perm))
                    return true;
            }
        }

        return false;
    }

    public bool CanCreate(List<string> roles) =>
        HasPermission(roles, p => p.Create);

    public bool CanEdit(List<string> roles) =>
        HasPermission(roles, p => p.Edit);

    public bool CanDelete(List<string> roles) =>
        HasPermission(roles, p => p.Delete);

    public bool CanView(List<string> roles) =>
        HasPermission(roles, p => p.View);
}

