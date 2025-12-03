namespace E_commerce.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public long Amount { get; set; } // cents
        public string Currency { get; set; } = "usd";
        public string? StripePaymentIntentId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
