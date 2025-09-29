using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using System.Security.Cryptography;
using System.Text;
using UniversityEvents.Application.Services;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Application.Repositories.Auth;

public interface IPasswordResetService
{
    Task<OperationResult> GenerateResetCodeAsync(string email);
    Task<OperationResult> VerifyResetCodeAsync(string email, string code);
    Task<OperationResult> ResetPasswordAsync(string email, string newPassword);
}
