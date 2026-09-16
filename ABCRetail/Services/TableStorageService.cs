using ABCRetail.Models;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class TableStorageService
    {
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;
        private readonly TableClient _orderTable;

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

            _orderTable = new TableClient(
                connectionString,
                "Orders");
        }


        // INITIALIZE AZURE TABLES


        public async Task InitializeAsync()
        {
            await _customerTable.CreateIfNotExistsAsync();
            await _productTable.CreateIfNotExistsAsync();
            await _orderTable.CreateIfNotExistsAsync();
        }


        // CUSTOMER FUNCTIONS


        public async Task AddCustomerAsync(Customer customer)
        {
            customer.PartitionKey = "Customers";

            // Get all existing customers
            var customers = await GetCustomersAsync();

            // Start Customer IDs at 1
            int nextId = 1;

            // Find existing numeric IDs
            var numericIds = customers
                .Select(c =>
                    int.TryParse(c.Id, out int id)
                        ? id
                        : 0)
                .Where(id => id > 0);

            // Generate the next Customer ID
            if (numericIds.Any())
            {
                nextId = numericIds.Max() + 1;
            }

            // Assign numeric Customer ID
            customer.Id = nextId.ToString();

            // RowKey uses the same ID
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

            // Get the existing customer from Azure Table Storage
            var existingCustomer =
                await _customerTable.GetEntityAsync<Customer>(
                    "Customers",
                    customer.Id);

            // Use the existing entity's ETag
            customer.ETag = existingCustomer.Value.ETag;

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



        // PRODUCT FUNCTIONS


        public async Task AddProductAsync(Product product)
        {
            product.PartitionKey = "Products";

            // Get all existing products
            var products = await GetProductsAsync();

            // Start IDs at 1
            int nextId = 1;

            // Find existing numeric IDs
            var numericIds = products
                .Select(p =>
                    int.TryParse(p.Id, out int id)
                        ? id
                        : 0)
                .Where(id => id > 0);

            // Generate the next ID
            if (numericIds.Any())
            {
                nextId = numericIds.Max() + 1;
            }

            // Assign numeric ID
            product.Id = nextId.ToString();

            // RowKey uses the same ID
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
        // ORDER FUNCTIONS

        public async Task AddOrderAsync(Order order)
        {
            order.PartitionKey = "Orders";

            var orders = await GetOrdersAsync();

            int nextId = 1;

            var numericIds = orders
                .Select(o => int.TryParse(o.Id, out int id) ? id : 0)
                .Where(id => id > 0);

            if (numericIds.Any())
            {
                nextId = numericIds.Max() + 1;
            }

            order.Id = nextId.ToString();
            order.RowKey = order.Id;

            await _orderTable.AddEntityAsync(order);
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();

            await foreach (var order in _orderTable.QueryAsync<Order>())
            {
                orders.Add(order);
            }

            return orders;
        }

        public async Task<Order?> GetOrderAsync(string id)
        {
            try
            {
                var response = await _orderTable.GetEntityAsync<Order>(
                    "Orders",
                    id);

                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}

