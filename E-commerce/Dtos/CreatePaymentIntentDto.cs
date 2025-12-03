namespace E_commerce.Dtos
{
    public class CreatePaymentIntentDto
    {
        public Guid CartId { get; set; }
        public string? Currency { get; set; }
    }
}
