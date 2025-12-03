using E_commerce.Application.Interfaces;
using E_commerce.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly IMyCheckoutService _checkoutService;
        public CheckoutController(IMyCheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
        {
            var (clientSecret, order) = await _checkoutService.CreatePaymentIntentFromCartAsync(dto.CartId, dto.Currency ?? "usd");
            return Ok(new { clientSecret, orderId = order.Id, amount = order.Amount });
        }
    }
}
