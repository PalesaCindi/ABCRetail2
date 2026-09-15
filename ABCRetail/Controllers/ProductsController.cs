
using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace ABCRetail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableStorage;
        private readonly BlobStorageService _blobStorage;

        public ProductsController(
            TableStorageService tableStorage,
            BlobStorageService blobStorage)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
        }

        // =====================================================
        // GET: Products
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorage.GetProductsAsync();

            return View(products);
        }

        // =====================================================
        // GET: Products/Details/5
        // =====================================================
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _tableStorage.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =====================================================
        // GET: Products/Create
        // =====================================================
        public IActionResult Create()
        {
            return View();
        }

        // =====================================================
        // POST: Products/Create
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Product product,
            IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                // Upload image to Azure Blob Storage
                if (image != null && image.Length > 0)
                {
                    product.ImageUrl =
                        await _blobStorage.UploadImageAsync(image);
                }

                // Save product to Azure Table Storage
                await _tableStorage.AddProductAsync(product);

                TempData["SuccessMessage"] =
                    "Product created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to save product: " + ex.Message);

                return View(product);
            }
        }

        // =====================================================
        // GET: Products/Edit/5
        // =====================================================
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _tableStorage.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =====================================================
        // POST: Products/Edit/5
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string? id,
            Product product,
            IFormFile? image)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (id != product.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                // Get the existing product first
                var existingProduct =
                    await _tableStorage.GetProductAsync(id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Preserve the existing image if no new image is uploaded
                product.ImageUrl = existingProduct.ImageUrl;

                // Upload new image if selected
                if (image != null && image.Length > 0)
                {
                    product.ImageUrl =
                        await _blobStorage.UploadImageAsync(image);
                }

                // Keep Azure Table Storage values
                product.PartitionKey = existingProduct.PartitionKey;
                product.RowKey = existingProduct.RowKey;
                product.ETag = existingProduct.ETag;
                product.Timestamp = existingProduct.Timestamp;

                // Update product
                await _tableStorage.UpdateProductAsync(product);

                TempData["SuccessMessage"] =
                    "Product updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update product: " + ex.Message);

                return View(product);
            }
        }

        // =====================================================
        // GET: Products/Delete/5
        // =====================================================
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _tableStorage.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =====================================================
        // POST: Products/Delete/5
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                await _tableStorage.DeleteProductAsync(id);

                TempData["SuccessMessage"] =
                    "Product deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete product: " + ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Image(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product =
                await _tableStorage.GetProductAsync(id);

            if (product == null ||
                string.IsNullOrEmpty(product.ImageUrl))
            {
                return NotFound();
            }

            var image =
                await _blobStorage.GetImageAsync(product.ImageUrl);

            if (image == null)
            {
                return NotFound();
            }

            return File(
                image.Value.Content,
                image.Value.ContentType);
        }
    }
}
