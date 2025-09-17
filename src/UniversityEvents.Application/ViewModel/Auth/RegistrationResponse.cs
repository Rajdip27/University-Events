﻿namespace UniversityEvents.Application.ViewModel.Auth;

public class RegistrationResponse
{
    public bool Success { get; set; } = true;
    public long UserId { get; set; }
    public List<string> Errors { get; set; } = new();
}
