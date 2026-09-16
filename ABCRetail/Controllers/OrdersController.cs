using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly QueueStorageService _queueStorageService;
        private readonly TableStorageService _tableStorageService;
        private readonly AuditLogService _auditLogService;
        private readonly FileStorageService _fileStorageService;

        public OrdersController(
            QueueStorageService queueStorageService,
            TableStorageService tableStorageService,
            AuditLogService auditLogService,
            FileStorageService fileStorageService)
        {
            _queueStorageService = queueStorageService;
            _tableStorageService = tableStorageService;
            _auditLogService = auditLogService;
            _fileStorageService = fileStorageService;
        }

        // ==========================================
        // ORDERS INDEX
        // ==========================================

        public async Task<IActionResult> Index()
        {
            var orders =
                await _tableStorageService.GetOrdersAsync();

            return View(orders);
        }

        // ==========================================
        // CREATE ORDER
        // ==========================================

        public async Task<IActionResult> Create()
        {
            ViewBag.Customers =
                await _tableStorageService.GetCustomersAsync();

            ViewBag.Products =
                await _tableStorageService.GetProductsAsync();

            return View();
        }

        // ==========================================
        // CREATE ORDER - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CustomerId,ProductId,Quantity")]
            Order order)
        {
            if (ModelState.IsValid)
            {
                // ----------------------------------
                // GET SELECTED PRODUCT
                // ----------------------------------

                var product =
                    await _tableStorageService
                        .GetProductAsync(order.ProductId);

                if (product == null)
                {
                    ModelState.AddModelError(
                        "ProductId",
                        "The selected product could not be found.");

                    ViewBag.Customers =
                        await _tableStorageService
                            .GetCustomersAsync();

                    ViewBag.Products =
                        await _tableStorageService
                            .GetProductsAsync();

                    return View(order);
                }

                
                // CALCULATE TOTAL AMOUNT
               

                order.TotalAmount =
                    product.Price * order.Quantity;


                // ORDER STATUS
                order.Status = "Received";

                order.OrderDate = DateTime.UtcNow;



                // ORDER PROCESSING QUEUE
                await _queueStorageService
                    .SendOrderStatusMessageAsync(
                        order,
                        "Received");

                //SAVE ORDER TO AZURE TABLE STORAGE
               
                // AddOrderAsync automatically generates:
                // 1, 2, 3, 4, 5...

                await _tableStorageService
                    .AddOrderAsync(order);

                // GET LOGGED-IN USER ID

                var userId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                // SAVE USER ACTIVITY TO AZURE FILE

                if (!string.IsNullOrEmpty(userId))
                {
                    await _fileStorageService
                        .WriteUserLogAsync(
                            userId,
                            $"Created order {order.Id}. " +
                            $"Product: {product.ProductName}. " +
                            $"Quantity: {order.Quantity}. " +
                            $"Total: R{order.TotalAmount:N2}");
                }


                // INVENTORY QUEUE

                var inventoryMessage = new
                {
                    OrderId = order.Id,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    Action = "Reduce inventory",
                    ProcessedAt = DateTime.UtcNow
                };

                await _queueStorageService
                    .SendMessageAsync(
                        "inventory-processing",
                        inventoryMessage);

                
                // AUDIT LOG
                

                await _auditLogService.LogAsync(
                    $"Created order {order.Id}. " +
                    $"Product: {product.ProductName}. " +
                    $"Quantity: {order.Quantity}. " +
                    $"Total: R{order.TotalAmount:N2}. " +
                    $"Status: Received");

                return RedirectToAction(
                    nameof(Index));
            }

            
            // RELOAD DROPDOWNS IF VALIDATION FAILS
            

            ViewBag.Customers =
                await _tableStorageService
                    .GetCustomersAsync();

            ViewBag.Products =
                await _tableStorageService
                    .GetProductsAsync();

            return View(order);
        }

        // UPDATE ORDER STATUS
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            string orderId,
            string customerId,
            string productId,
            int quantity,
            decimal totalAmount,
            string status)
        {
            var allowedStatuses =
                new[]
                {
                    "Received",
                    "Processing",
                    "Packing",
                    "Shipped",
                    "Delivered"
                };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest(
                    "Invalid order status.");
            }

            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                TotalAmount = Convert.ToDouble(totalAmount),
                Status = status,
                OrderDate = DateTimeOffset.UtcNow
            };

            // ----------------------------------
            // SEND STATUS TO QUEUE
            // ----------------------------------

            await _queueStorageService
                .SendOrderStatusMessageAsync(
                    order,
                    status);

            // ----------------------------------
            // AUDIT LOG
            // ----------------------------------

            await _auditLogService.LogAsync(
                $"Order {orderId} moved to {status}");

            return RedirectToAction(
                nameof(Index));
        }
    }
}