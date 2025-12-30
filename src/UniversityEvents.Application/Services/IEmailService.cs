using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using UniversityEvents.Application.CommonModel;

namespace UniversityEvents.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}
public class EmailService(IConfiguration _config) : IEmailService
{
    public async Task SendEmailAsync(EmailMessage emailMessage)
    {
        try
        {
            if (emailMessage.To.Count == 0)
                throw new ArgumentException("Email must have at least one recipient.");

            using var smtp = new SmtpClient(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]))
            {
                Credentials = new NetworkCredential(_config["EmailSettings:Username"], _config["EmailSettings:Password"]),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]),
                Subject = emailMessage.Subject,
                IsBodyHtml = !string.IsNullOrEmpty(emailMessage.HtmlFilePath) // true if HTML file exists
            };

            emailMessage.To.ForEach(to => message.To.Add(to));
            emailMessage.CC?.ForEach(cc => message.CC.Add(cc));
            emailMessage.BCC?.ForEach(bcc => message.Bcc.Add(bcc));

            // Determine body
            if (!string.IsNullOrEmpty(emailMessage.HtmlFilePath) && File.Exists(emailMessage.HtmlFilePath))
            {
                message.Body = await File.ReadAllTextAsync(emailMessage.HtmlFilePath);
            }
            else if (!string.IsNullOrEmpty(emailMessage.Message))
            {
                message.Body = emailMessage.Message;
                message.IsBodyHtml = false;
            }
            else
            {
                message.Body = "";
            }

            // Add attachments
            if (emailMessage.Attachments != null)
            {
                foreach (var file in emailMessage.Attachments)
                {
                    if (File.Exists(file))
                        message.Attachments.Add(new Attachment(file));
                }
            }

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}

//public class TestController : Controller
//{
//    private readonly IEmailService _emailService;

//    public TestController(IEmailService emailService)
//    {
//        _emailService = emailService;
//    }

//    public async Task<IActionResult> SendEmailExample()
//    {
//        // Example 1: HTML email with attachments
//        var htmlEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            CC = new List<string> { "cc@gmail.com" },
//            BCC = null,
//            Subject = "HTML Email Example",
//            HtmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate.html"),
//            Attachments = new List<string> { @"C:\Temp\File1.pdf", @"C:\Temp\File2.txt" }
//        };
//        await _emailService.SendEmailAsync(htmlEmail);

//        // Example 2: Plain message with single attachment
//        var plainEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            Subject = "Plain Message Example",
//            Message = "Hello! This is a plain text email.",
//            Attachments = new List<string> { @"C:\Temp\Document.pdf" }
//        };
//        await _emailService.SendEmailAsync(plainEmail);

//        // Example 3: Plain message without attachment
//        var simpleEmail = new EmailMessage
//        {
//            To = new List<string> { "to@gmail.com" },
//            Subject = "Simple Message",
//            Message = "Just a simple text message."
//        };
//        await _emailService.SendEmailAsync(simpleEmail);

//        return Ok("All emails sent dynamically!");
//    }
//}

