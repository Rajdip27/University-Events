using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UniversityEvents.Application.ViewModel.Utilities;

namespace UniversityEvents.Application.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an email message asynchronously to the specified recipient with the given subject and HTML content.
    /// </summary>
    /// <param name="to">The email address of the recipient. Cannot be null or empty.</param>
    /// <param name="subject">The subject line of the email message. Cannot be null or empty.</param>
    /// <param name="htmlContent">The HTML-formatted content of the email body. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendEmailAsync(string to, string subject, string htmlContent);
}
public class EmailService : IEmailService
{
    private readonly SMTPSettings _settings;
    public EmailService(IOptions<SMTPSettings> options) => _settings = options.Value;
    /// <summary>
    /// Sends an email message asynchronously to the specified recipient using the configured SMTP settings.
    /// </summary>
    /// <remarks>The email is sent using the SMTP server and credentials specified in the configuration. The
    /// message body is sent as HTML. This method does not validate the recipient address format; invalid addresses may
    /// cause the operation to fail.</remarks>
    /// <param name="to">The email address of the recipient. Cannot be null or empty.</param>
    /// <param name="subject">The subject line of the email message. Cannot be null.</param>
    /// <param name="htmlContent">The HTML content to include in the body of the email message. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };


        var mail = new MailMessage
        {
            From = new MailAddress(_settings.Username, _settings.FromName),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };
        mail.To.Add(to);


        await client.SendMailAsync(mail);
    }
}
