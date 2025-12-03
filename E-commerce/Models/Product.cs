namespace E_commerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public long Price { get; set; } // smallest currency unit (cents)
        public string Currency { get; set; } = "usd";
    }
}
