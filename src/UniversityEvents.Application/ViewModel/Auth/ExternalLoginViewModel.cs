namespace UniversityEvents.Application.ViewModel.Auth;

public class ExternalLoginViewModel
{

    public string Provider { get; set; }
    public string ReturnUrl { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
}
