using E_commerce.Application.Interfaces;
using E_commerce.Models;
using Microsoft.EntityFrameworkCore;
using E_commerce.Data;
using Stripe;
using Stripe.Checkout;
using Stripe.Events;

namespace E_commerce.Application.Services
{
    public class MyCheckoutService : IMyCheckoutService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public MyCheckoutService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<(string clientSecret, Order order)> CreatePaymentIntentFromCartAsync(Guid cartId, string currency = "usd")
        {
            var cart = await _db.Carts.Include(c => c.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
            try
            {

            if (cart == null) throw new InvalidOperationException("Cart not found");
            }
            catch
            {
                return ("cart is not found", new Order());
            }

            long amount = cart.Items.Sum(i=> i.Product!.Price * i.Quantity);

            if(amount <= 0)
            {
                var freeOrder = new Order
                {
                    Amount = 0,
                    Currency = currency,
                    Status = "Paid",
                };
                
                foreach(var ci in cart.Items)
                {
                    freeOrder.Items.Add(new OrderItem
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product!.Name,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price
                    });
                }

                _db.Orders.Add(freeOrder);

                _db.CartItems.RemoveRange(cart.Items);
                _db.Carts.Remove(cart);

                await _db.SaveChangesAsync();

                return ("FREE_ORDER", freeOrder);
            }


            var order = new Order
            {
                Amount = amount,
                Currency = currency,
                Status = "Pending"
            };

            foreach(var ci in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product!.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Metadata = new Dictionary<string, string>
                {
                    {"order_id", order.Id.ToString() },
                    {"cart_id", cart.Id.ToString() }
                },
                // optionally: PaymentMethodTypes = new List<string> { "card" },
                // optionally: SetupFutureUsage = "off_session" // if saving for recurring
            };


            var service = new PaymentIntentService();
            var pi = await service.CreateAsync(options);

            order.StripePaymentIntentId = pi.Id;
            await _db.SaveChangesAsync();

            return (pi.ClientSecret, order);
        }

        public async Task HandleStripeEventAsync(Event stripeEvent)
        {
            if(stripeEvent.Type == "payment_intent.succeeded")
            {
                var pi = stripeEvent.Data.Object as PaymentIntent;
                var orderIdStr = pi?.Metadata.GetValueOrDefault("order_id");
                if(Guid.TryParse(orderIdStr, out var orderId))
                {
                    var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                    if(order != null)
                    {
                        order.Status = "Paid";
                        await _db.SaveChangesAsync();
                    }
                }

                _db.Payments.Add(new Payment
                {
                    StripeEventId = stripeEvent.Id,
                    StripePaymentIntentId = pi!.Id,
                    Status = "succeeded"
                });
                await _db.SaveChangesAsync();
            }
            else
            {
                // handle other events if needed
            }
        }
        
    }
}
