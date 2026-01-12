using System.ComponentModel.DataAnnotations;

namespace UniversityEvents.Application.ViewModel.ForgotPassword;

public class VerifyOtpDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
    public string Otp { get; set; } = null!;
}
