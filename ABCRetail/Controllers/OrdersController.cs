using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly QueueStorageService _queueStorageService;
        private readonly TableStorageService _tableStorageService;

        public OrdersController(
            QueueStorageService queueStorageService,
            TableStorageService tableStorageService)
        {
            _queueStorageService = queueStorageService;
            _tableStorageService = tableStorageService;
        }

        // GET: ORDERS
        public IActionResult Index()
        {
            return View();
        }

        // GET: ORDERS/Create
        public async Task<IActionResult> Create()
        {
            // Get customers from Azure Table Storage
            var customers =
                await _tableStorageService.GetCustomersAsync();

            // Get products from Azure Table Storage
            var products =
                await _tableStorageService.GetProductsAsync();

            ViewBag.Customers = customers;
            ViewBag.Products = products;

            return View();
        }

        // POST: ORDERS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CustomerId,ProductId,Quantity,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid().ToString();
                order.Status = "Pending";
                order.OrderDate = DateTime.UtcNow;

                var orderMessage = new
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate
                };

                await _queueStorageService.SendMessageAsync(
                    "order-processing",
                    orderMessage);

                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if validation fails
            ViewBag.Customers =
                await _tableStorageService.GetCustomersAsync();

            ViewBag.Products =
                await _tableStorageService.GetProductsAsync();

            return View(order);
        }
    }
}