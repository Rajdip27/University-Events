using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UniversityEvents.Application.ViewModel.Auth;

namespace UniversityEvents.Application.Mappers;

public static class ExternalLoginMapper
{
    public static ExternalLoginViewModel MapToViewModel(ExternalLoginInfo info, string returnUrl = null)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        return new ExternalLoginViewModel
        {
            Provider = info.LoginProvider,
            ReturnUrl = returnUrl,
            Email = email,
            FullName = name,
            Claims = info.Principal.Claims.ToDictionary(c => c.Type, c => c.Value)
        };
    }
}
