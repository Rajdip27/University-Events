using Microsoft.AspNetCore.Mvc;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Application.SSLCommerz.Services;

namespace UniversityEvents.Web.Controllers;

[Route("payment")]
public class PaymentController(ISSLCommerzService _sslService) : Controller
{
    [HttpGet("initiate")]
    public async Task<IActionResult> InitiatePayment()
    
    {
        try
        {
            var host = $"{Request.Scheme}://{Request.Host}";

            var request = new SSLCommerzPaymentRequest
            {
                Amount = 1000, // You can pass dynamically
                TransactionId = Guid.NewGuid().ToString(),
                SuccessUrl = $"{host}/payment/success",
                FailUrl = $"{host}/payment/fail",
                CancelUrl = $"{host}/payment/cancel",
                CustomerName = "Test User",
                CustomerEmail = "test@mail.com",
                CustomerPhone = "01700000000",
                CustomerAddress = "Dhaka",
                CustomerCity = "Dhaka",
                CustomerCountry = "Bangladesh"

            };

            var response = await _sslService.CreatePaymentAsync(request);

            // Redirect customer to SSLCommerz gateway
            return Redirect(response.GatewayPageURL);
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
