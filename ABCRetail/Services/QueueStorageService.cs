using ABCRetail.Models;
using Azure.Storage.Queues;
using System.Text.Json;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {
        private readonly string _connectionString;

        // Queue names
        private const string OrderQueueName = "order-processing";
        private const string InventoryQueueName = "inventory-processing";
        private const string ImageQueueName = "image-processing";

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public QueueStorageService(IConfiguration configuration)
        {
            _connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");
        }

        // ==========================================
        // CREATE QUEUE CLIENT
        // ==========================================

        private QueueClient GetQueue(string queueName)
        {
            return new QueueClient(
                _connectionString,
                queueName,
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
        }

        // ==========================================
        // CREATE ALL QUEUES
        // ==========================================

        public async Task CreateQueuesAsync()
        {
            QueueClient orderQueue =
                GetQueue(OrderQueueName);

            QueueClient inventoryQueue =
                GetQueue(InventoryQueueName);

            QueueClient imageQueue =
                GetQueue(ImageQueueName);

            await orderQueue.CreateIfNotExistsAsync();
            await inventoryQueue.CreateIfNotExistsAsync();
            await imageQueue.CreateIfNotExistsAsync();
        }

        // ==========================================
        // SEND GENERIC MESSAGE
        // ==========================================

        public async Task SendMessageAsync(
            string queueName,
            object message)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.CreateIfNotExistsAsync();

            string json =
                JsonSerializer.Serialize(
                    message,
                    new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });

            await queue.SendMessageAsync(json);
        }

        // ==========================================
        // SEND ORDER STATUS MESSAGE
        // ==========================================

        public async Task SendOrderStatusMessageAsync(
            Order order,
            string status)
        {
            var message = new
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                Status = status,
                OrderDate = order.OrderDate,
                ProcessedAt = DateTime.UtcNow
            };

            await SendMessageAsync(
                OrderQueueName,
                message);
        }

        // ==========================================
        // SEND INVENTORY MESSAGE
        // ==========================================

        public async Task SendInventoryMessageAsync(
            Order order)
        {
            var message = new
            {
                OrderId = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                ProcessedAt = DateTime.UtcNow
            };

            await SendMessageAsync(
                InventoryQueueName,
                message);
        }

        // ==========================================
        // SEND IMAGE PROCESSING MESSAGE
        // ==========================================

        public async Task SendImageProcessingMessageAsync(
            string productId,
            string imageUrl)
        {
            var message = new
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                ProcessedAt = DateTime.UtcNow
            };

            await SendMessageAsync(
                ImageQueueName,
                message);
        }

        // ==========================================
        // PEEK ORDER MESSAGES
        // DOES NOT DELETE MESSAGES
        // ==========================================

        public async Task<List<string>> PeekOrderMessagesAsync()
        {
            return await PeekMessagesAsync(
                OrderQueueName);
        }

        // ==========================================
        // PEEK INVENTORY MESSAGES
        // ==========================================

        public async Task<List<string>> PeekInventoryMessagesAsync()
        {
            return await PeekMessagesAsync(
                InventoryQueueName);
        }

        // ==========================================
        // PEEK IMAGE MESSAGES
        // ==========================================

        public async Task<List<string>> PeekImageMessagesAsync()
        {
            return await PeekMessagesAsync(
                ImageQueueName);
        }

        // ==========================================
        // PEEK QUEUE MESSAGES
        // DOES NOT DELETE THEM
        // ==========================================

        public async Task<List<string>> PeekMessagesAsync(
            string queueName)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.CreateIfNotExistsAsync();

            var messages =
                new List<string>();

            var response =
                await queue.PeekMessagesAsync(
                    maxMessages: 32);

            foreach (var message in response.Value)
            {
                messages.Add(
                    message.MessageText);
            }

            return messages;
        }

        // ==========================================
        // GET QUEUE MESSAGE COUNT
        // ==========================================

        public async Task<int> GetMessageCountAsync(
            string queueName)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.CreateIfNotExistsAsync();

            var properties =
                await queue.GetPropertiesAsync();

            return properties.Value.ApproximateMessagesCount;
        }

        // ==========================================
        // GET ORDER QUEUE COUNT
        // ==========================================

        public async Task<int> GetOrderQueueCountAsync()
        {
            return await GetMessageCountAsync(
                OrderQueueName);
        }

        // ==========================================
        // GET INVENTORY QUEUE COUNT
        // ==========================================

        public async Task<int> GetInventoryQueueCountAsync()
        {
            return await GetMessageCountAsync(
                InventoryQueueName);
        }

        // ==========================================
        // GET IMAGE QUEUE COUNT
        // ==========================================

        public async Task<int> GetImageQueueCountAsync()
        {
            return await GetMessageCountAsync(
                ImageQueueName);
        }

        // ==========================================
        // RECEIVE MESSAGES
        // WARNING:
        // THIS DOES NOT DELETE THEM AUTOMATICALLY
        // ==========================================

        public async Task<List<QueueMessageData>> ReceiveMessagesAsync(
            string queueName,
            int maxMessages = 32)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.CreateIfNotExistsAsync();

            var result =
                new List<QueueMessageData>();

            var response =
                await queue.ReceiveMessagesAsync(
                    maxMessages: maxMessages);

            foreach (var message in response.Value)
            {
                result.Add(
                    new QueueMessageData
                    {
                        MessageId =
                            message.MessageId,

                        PopReceipt =
                            message.PopReceipt,

                        MessageText =
                            message.MessageText,

                        InsertedOn =
                            message.InsertedOn,

                        ExpiresOn =
                            message.ExpiresOn,

                        DequeueCount =
                            message.DequeueCount
                    });
            }

            return result;
        }

        // ==========================================
        // DELETE A MESSAGE AFTER PROCESSING
        // ==========================================

        public async Task DeleteMessageAsync(
            string queueName,
            string messageId,
            string popReceipt)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.DeleteMessageAsync(
                messageId,
                popReceipt);
        }

        // ==========================================
        // CLEAR QUEUE
        // USE CAREFULLY
        // ==========================================

        public async Task ClearQueueAsync(
            string queueName)
        {
            QueueClient queue =
                GetQueue(queueName);

            await queue.CreateIfNotExistsAsync();

            await queue.ClearMessagesAsync();
        }
    }

    // ==========================================
    // QUEUE MESSAGE MODEL
    // ==========================================

    public class QueueMessageData
    {
        public string MessageId { get; set; } = string.Empty;

        public string PopReceipt { get; set; } = string.Empty;

        public string MessageText { get; set; } = string.Empty;

        public DateTimeOffset? InsertedOn { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }

        public long DequeueCount { get; set; }
    }
}