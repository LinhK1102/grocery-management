using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositories.Services;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSService _paymentService;
        public PaymentController(PayOSService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest req)
        {
            try
            {
                var response = await _paymentService.CreatePaymentAsync(req);
                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create payment: {ex.Message}");
            }
        }
    }
}
