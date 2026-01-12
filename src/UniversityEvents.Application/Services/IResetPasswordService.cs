namespace UniversityEvents.Application.Services;

public interface IResetPasswordService
{
    Task ResetPasswordAsync(string email, string newPassword);
}
