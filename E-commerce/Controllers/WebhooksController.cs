using E_commerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace E_commerce.Controllers
{
    public class WebhooksController : ControllerBase
    {
        private readonly IMyCheckoutService _checkoutService;
        private readonly IConfiguration _configuration;

        public WebhooksController(IMyCheckoutService checkoutService, IConfiguration configuration)
        {
            _checkoutService = checkoutService;
            _configuration = configuration;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];
            var webhookSecret = _configuration["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            await _checkoutService.HandleStripeEventAsync(stripeEvent);
            return Ok();
        }
    }
}
