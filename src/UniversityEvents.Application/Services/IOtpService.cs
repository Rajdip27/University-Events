using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Application.Services;

public interface IOtpService
{
    Task SendOtpAsync(string email);
    Task<bool> VerifyOtpAsync(string email, string otp);
}

public class OtpService : IOtpService
{
    private readonly UserManager<User> _userManager;
    private readonly UniversityDbContext _context;
    private readonly IEmailService _emailService;

    public OtpService(
        UserManager<User> userManager,
        UniversityDbContext context,
        IEmailService emailService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task SendOtpAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var oldOtps = await _context.PasswordResetOtp
                .Where(x => x.UserId == user.Id && !x.IsUsed)
                .ToListAsync();

            if (oldOtps.Count > 0)
                _context.PasswordResetOtp.RemoveRange(oldOtps);

            var otp = Random.Shared.Next(100000, 999999).ToString();

            await _context.PasswordResetOtp.AddAsync(new PasswordResetOtp
            {
                UserId = user.Id,
                Otp = otp,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            await _context.SaveChangesAsync();

            // Optional: send email
            // await _emailService.SendAsync(email, "OTP", $"Your OTP is {otp}");
        }
        catch (Exception ex)
        {
            // Log exception here if you have a logger
            // _logger.LogError(ex, "Error sending OTP");
            throw new Exception("Failed to send OTP. " + ex.Message, ex);
        }
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var otpEntity = await _context.PasswordResetOtp
                .Where(x =>
                    x.UserId == user.Id &&
                    x.Otp == otp &&
                    !x.IsUsed &&
                    x.ExpireAt >= DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (otpEntity == null)
                return false;

            otpEntity.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log exception here if you have a logger
            // _logger.LogError(ex, "Error verifying OTP");
            return false;
        }
    }
}
