using ABCRetail.Models;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class TableStorageService
    {
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

        public TableStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            _customerTable = new TableClient(
                connectionString,
                "Customers");

            _productTable = new TableClient(
                connectionString,
                "Products");
        }

        
        // INITIALIZE AZURE TABLES
       
        public async Task InitializeAsync()
        {
            await _customerTable.CreateIfNotExistsAsync();
            await _productTable.CreateIfNotExistsAsync();
        }

        // =====================================================
        // CUSTOMER FUNCTIONS
        // =====================================================

        public async Task AddCustomerAsync(Customer customer)
        {
            customer.PartitionKey = "Customers";

            if (string.IsNullOrEmpty(customer.Id))
            {
                customer.Id = Guid.NewGuid().ToString();
            }

            customer.RowKey = customer.Id;

            await _customerTable.AddEntityAsync(customer);
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();

            await foreach (var customer
                in _customerTable.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }

            return customers;
        }

        public async Task<Customer?> GetCustomerAsync(string id)
        {
            await foreach (var customer
                in _customerTable.QueryAsync<Customer>(
                    x => x.PartitionKey == "Customers"
                      && x.RowKey == id))
            {
                return customer;
            }

            return null;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            customer.PartitionKey = "Customers";
            customer.RowKey = customer.Id;

            await _customerTable.UpdateEntityAsync(
                customer,
                customer.ETag,
                TableUpdateMode.Replace);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var customer = await GetCustomerAsync(id);

            if (customer != null)
            {
                await _customerTable.DeleteEntityAsync(
                    customer.PartitionKey,
                    customer.RowKey);
            }
        }

        // =====================================================
        // PRODUCT FUNCTIONS
        // =====================================================

        public async Task AddProductAsync(Product product)
        {
            product.PartitionKey = "Products";

            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();
            }

            product.RowKey = product.Id;

            await _productTable.AddEntityAsync(product);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();

            await foreach (var product
                in _productTable.QueryAsync<Product>())
            {
                products.Add(product);
            }

            return products;
        }

        public async Task<Product?> GetProductAsync(string id)
        {
            await foreach (var product
                in _productTable.QueryAsync<Product>(
                    x => x.PartitionKey == "Products"
                      && x.RowKey == id))
            {
                return product;
            }

            return null;
        }

        public async Task UpdateProductAsync(Product product)
        {
            product.PartitionKey = "Products";
            product.RowKey = product.Id;

            await _productTable.UpdateEntityAsync(
                product,
                product.ETag,
                TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string id)
        {
            var product = await GetProductAsync(id);

            if (product != null)
            {
                await _productTable.DeleteEntityAsync(
                    product.PartitionKey,
                    product.RowKey);
            }
        }
    }
}