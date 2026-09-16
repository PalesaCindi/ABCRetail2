using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Orders";

        public string RowKey
        {
            get => Id;
            set => Id = value;
        }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string Id { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public double TotalAmount { get; set; }

        public string Status { get; set; } = "Received";

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
    }
}