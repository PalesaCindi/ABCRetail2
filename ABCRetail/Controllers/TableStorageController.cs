using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly TableStorageService _tableStorage;

        public TableStorageController(TableStorageService tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorage.GetProductsAsync();

            return View(products);
        }
    }
}