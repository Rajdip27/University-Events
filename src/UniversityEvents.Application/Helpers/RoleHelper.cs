using System.Security.Claims;
using UniversityEvents.Application.Security;

namespace UniversityEvents.Application.Helpers;

public static class RoleHelper
{
    public static bool CanManage(ClaimsPrincipal user) =>
   user.IsInRole(AppRoles.Admin) || user.IsInRole(AppRoles.Editor);

    public static bool CanDelete(ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Admin);
}
