using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Application.SSLCommerz.Services;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Web.Controllers;

[Route("payment")]
public class PaymentController(ISSLCommerzService _sslService,IStudentRegistrationRepository studentRegistration) : Controller
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
            return View("PaymentFail", "Error");
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
            var registerId = Request.Query["id"].ToString();
            var validation = await _sslService.ValidatePaymentAsync(validationId);

            return View(validation); // Pass model to view
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

}
