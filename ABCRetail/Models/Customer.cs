using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = "Customers";

        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset? Timestamp { get; set; }

        [NotMapped]
        public ETag ETag { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}

