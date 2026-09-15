using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail.Models
{
    public class Product : ITableEntity
    {
        public string PartitionKey { get; set; } = "Products";

        public string RowKey
        {
            get => Id;
            set => Id = value;
        }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}

