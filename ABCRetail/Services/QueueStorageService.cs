using Azure.Storage.Queues;
using System.Text.Json;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {

        private readonly string _connectionString;

        public QueueStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is missing.");
        }

        public async Task SendMessageAsync(
            string queueName,
            object message)
        {
            QueueClient queueClient =
                new QueueClient(
                    _connectionString,
                    queueName);

            await queueClient.CreateIfNotExistsAsync();

            string jsonMessage =
                JsonSerializer.Serialize(message);

            await queueClient.SendMessageAsync(jsonMessage);
        }
    }
}
