namespace E_commerce.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string StripeEventId { get; set; } = "";
        public string StripePaymentIntentId { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
