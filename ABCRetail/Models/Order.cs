namespace ABCRetail.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CustomerId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
