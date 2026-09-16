using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetail.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueStorageService _queueStorageService;

        public QueueController(
            QueueStorageService queueStorageService)
        {
            _queueStorageService = queueStorageService;
        }

        // ==========================================
        // QUEUE PROCESSING PAGE
        // ==========================================

        public async Task<IActionResult> Index()
        {
            // Make sure all queues exist
            await _queueStorageService.CreateQueuesAsync();

            // Get order messages
            var orderMessages =
                await _queueStorageService.PeekMessagesAsync(
                    "order-processing");

            // Get inventory messages
            var inventoryMessages =
                await _queueStorageService.PeekMessagesAsync(
                    "inventory-processing");

            // Get image messages
            var imageMessages =
                await _queueStorageService.PeekMessagesAsync(
                    "image-processing");

            // Convert JSON messages into dictionaries
            ViewBag.OrderMessages =
                DeserializeMessages(orderMessages);

            ViewBag.InventoryMessages =
                DeserializeMessages(inventoryMessages);

            ViewBag.ImageMessages =
                DeserializeMessages(imageMessages);

            // Queue counts
            ViewBag.OrderCount =
                await _queueStorageService.GetMessageCountAsync(
                    "order-processing");

            ViewBag.InventoryCount =
                await _queueStorageService.GetMessageCountAsync(
                    "inventory-processing");

            ViewBag.ImageCount =
                await _queueStorageService.GetMessageCountAsync(
                    "image-processing");

            return View();
        }

        // ==========================================
        // DESERIALIZE QUEUE MESSAGES
        // ==========================================

        private List<Dictionary<string, object>>
            DeserializeMessages(List<string> messages)
        {
            var result =
                new List<Dictionary<string, object>>();

            foreach (var message in messages)
            {
                try
                {
                    var json =
                        JsonSerializer.Deserialize<
                            Dictionary<string, object>>(
                                message);

                    if (json != null)
                    {
                        result.Add(json);
                    }
                }
                catch
                {
                    // Ignore invalid JSON messages
                }
            }

            return result;
        }
    }
}