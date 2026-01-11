using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Helpers;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.Services;
using UniversityEvents.Application.Services.Pdf;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Application.SSLCommerz.Services;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Web.Controllers;


[Route("payment")]
public class PaymentController(ISSLCommerzService _sslService, IEmailService emailService, IStudentRegistrationRepository studentRegistration, IPaymentRepository paymentRepository, IPaymentHistoryRepository paymentHistoryRepository, IPdfService _pdfService, IRazorViewToStringRenderer _razorViewToStringRenderer) : Controller
{
    [HttpGet("initiate")]
    public async Task<IActionResult> InitiatePayment(long registerId)

    {
        try
        {
            var registration = await studentRegistration.GetRegistrationByIdAsync(registerId, CancellationToken.None);

            if (registration != null)
            {
                var host = $"{Request.Scheme}://{Request.Host}";

                var request = new SSLCommerzPaymentRequest
                {
                    Amount = registration.Event.RegistrationFee, // You can pass dynamically
                    TransactionId = Guid.NewGuid().ToString(),
                    SuccessUrl = $"{host}/payment/success?id={registerId}",
                    FailUrl = $"{host}/payment/fail",
                    CancelUrl = $"{host}/payment/cancel",
                    CustomerName = registration.FullName,
                    CustomerEmail = registration.Email,
                    CustomerPhone = registration.PhoneNumber,
                    CustomerAddress = "Dhaka",
                    CustomerCity = "Dhaka",
                    CustomerCountry = "Bangladesh"
                };

                var response = await _sslService.CreatePaymentAsync(request);

                // Redirect customer to SSLCommerz gateway
                return Redirect(response.GatewayPageURL);
            }
            return View("PaymentFail", "Data Not Found");
        }
        catch (Exception ex)
        {
            // Log exception
            return View("PaymentFail", ex.Message);
        }
    }
    [HttpPost("success")]
    public async Task<IActionResult> PaymentSuccess(IFormCollection form)
    {
        try
        {
            var validationId = form["val_id"].ToString();
            if (string.IsNullOrWhiteSpace(validationId))
                return View("PaymentFail", "Validation id missing");
            if (!long.TryParse(Request.Query["id"], out var registrationId))
                return View("PaymentFail", "Invalid registration id");
            var registration = await studentRegistration
                .GetRegistrationByIdAsync(registrationId, CancellationToken.None);
            if (registration == null)
                return View("PaymentFail", "Registration not found");
            var validation = await _sslService.ValidatePaymentAsync(validationId);
            if (validation == null)
                return View("PaymentFail", "Payment validation failed");

            if (validation.Status.Equals("VALID", StringComparison.OrdinalIgnoreCase) ||
                validation.Status.Equals("VALIDATED", StringComparison.OrdinalIgnoreCase))
            {
                var payment = PaymentHelper.UpdateFromSslResponse(validation);

                if (payment == null)
                    return View("PaymentFail", "Unable to create payment record");
                payment.RegistrationId = registration.Id;
                var insertedPayment = await paymentRepository.AddAsync(payment, CancellationToken.None);
                if (insertedPayment == null)
                    return View("PaymentFail", "Payment record insert failed");
                var paymentHistory = PaymentHistoryHelper.CreateFromSslResponse(validation);
                if (paymentHistory != null)
                {
                    paymentHistory.PaymentId = insertedPayment.Id;
                    await paymentHistoryRepository.AddAsync(paymentHistory, CancellationToken.None);
                }
                registration.PaymentStatus = "Paid";
                registration.UserId = registration.UserId;
                await studentRegistration.CreateOrUpdateRegistrationAsync(registration, CancellationToken.None);
                await SendPaymentPaidInvoiceEmail(registration);
            }
            return View(validation);
        }
        catch (Exception ex)
        {
            return View("PaymentFail", ex.Message);
        }
    }


    [HttpPost("fail")]
    public IActionResult PaymentFail(IFormCollection form)
    {
        var tranId = form["tran_id"].ToString();
        return View("PaymentFail", tranId); // Pass Transaction ID
    }

    [HttpPost("cancel")]
    public IActionResult PaymentCancel(IFormCollection form)
    {
        var tranId = form["tran_id"].ToString();
        return View("PaymentCancel", tranId); // Pass Transaction ID
    }


    private async Task SendPaymentPaidInvoiceEmail(StudentRegistrationVm result)
    {
        try
        {

            if(string.IsNullOrWhiteSpace(result.Email) && result is null)
                return;

            var htmlContent =
         await _razorViewToStringRenderer
             .RenderViewToStringAsync(
                 "EmailTemplates/PaymentSuccessfulEmailTemplate",
                 result
             );
            var pdfBytes = await GeneratePaymentPaidInvoicePdfBytes(result.Id);

            var emailMessage = new EmailMessage
            {
                To = new List<string> { result.Email },
                Subject = "Payment Successful 🎉",
                HtmlFilePath = htmlContent,
                Attachments = new List<EmailAttachment>
        {
            new EmailAttachment
            {
                FileName = "Payment_Invoice.pdf",
                Content = pdfBytes,
                ContentType = "application/pdf"
            }
        }
            };

            await emailService.SendEmailAsync(emailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }


    private async Task<byte[]> GeneratePaymentPaidInvoicePdfBytes(long registerId)
    {
        try
        {
            // Example data
            var data = await paymentRepository.GetByIdAsync(registerId, CancellationToken.None);
            // Render Razor view to string
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("PdfTemplates/PaymentPaidInvoicePdf", data);
            var pdfOptions = new PdfOptions
            {
                PageSize = "A4",
                Landscape = false,
                MarginTop = 30,
                MarginBottom = 10,
                MarginLeft = 10,
                MarginRight = 10,
                ShowPageNumbers = false
            };
            return _pdfService.GeneratePdf(htmlContent, pdfOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }


}
