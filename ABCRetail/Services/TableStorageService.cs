using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class TableStorageService
    {
        private readonly string _connectionString;

        public TableStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is missing.");
        }

        public async Task CreateTableAsync(string tableName)
        {
            var serviceClient = new TableServiceClient(_connectionString);

            await serviceClient.CreateTableIfNotExistsAsync(tableName);
        }

        public async Task<TableClient> GetTableClientAsync(string tableName)
        {
            var serviceClient = new TableServiceClient(_connectionString);

            await serviceClient.CreateTableIfNotExistsAsync(tableName);

            return serviceClient.GetTableClient(tableName);
        }
    }
}

