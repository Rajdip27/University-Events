namespace UniversityEvents.Application.ViewModel.ForgotPassword;

public class OtpEmailViewModel
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; }= null!;
    public string OTP { get; set; } = null!;
}
