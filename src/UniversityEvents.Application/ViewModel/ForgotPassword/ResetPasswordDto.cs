namespace UniversityEvents.Application.ViewModel.ForgotPassword;

public class ResetPasswordDto
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
