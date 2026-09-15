using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

public class CustomersController : Controller
{
    private readonly TableStorageService _tableStorageService;

    public TableStorageService TableStorageService => _tableStorageService;

    public CustomersController(TableStorageService tableStorageService)
    {
        _tableStorageService = tableStorageService;
    }

    // GET: CUSTOMERS
    public async Task<IActionResult> Index()
    {
        var customers = await TableStorageService.GetCustomersAsync();
        return View(customers);
    }

    // GET: CUSTOMERS/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customer = await TableStorageService.GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CUSTOMERS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("PartitionKey,RowKey,Timestamp,ETag,Id,Name,Email,Phone,Address")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            await TableStorageService.AddCustomerAsync(customer);

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customer = await TableStorageService.GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: CUSTOMERS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string? id,
        [Bind("PartitionKey,RowKey,Timestamp,ETag,Id,Name,Email,Phone,Address")] Customer customer)
    {
        if (string.IsNullOrEmpty(id) || id != customer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingCustomer =
                await TableStorageService.GetCustomerAsync(customer.Id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            await TableStorageService.UpdateCustomerAsync(customer);

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customer = await TableStorageService.GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: CUSTOMERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customer = await TableStorageService.GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        await TableStorageService.DeleteCustomerAsync(customer.Id);

        return RedirectToAction(nameof(Index));
    }
}