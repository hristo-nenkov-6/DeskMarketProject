namespace DeskMarket.Models
{
    public class AllViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsSeller = false;

        public bool HasBought = false;
    }
}
