using E_commerce.Models;
using Stripe;
namespace E_commerce.Application.Interfaces
{
    public interface IMyCheckoutService
    {
        Task<(string clientSecret, Order order)> CreatePaymentIntentFromCartAsync(Guid cartId, string currency = "usd");
        Task HandleStripeEventAsync(Event stripeEvent);
    }
}
