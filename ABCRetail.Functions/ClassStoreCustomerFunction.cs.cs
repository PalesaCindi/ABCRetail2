using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace ABCRetail.Functions
{
    public class StoreCustomerFunction
    {
        private readonly TableClient _customerTable;

        public StoreCustomerFunction()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString")
                ?? throw new InvalidOperationException(
                    "AzureStorageConnectionString is not configured.");

            _customerTable = new TableClient(
                connectionString,
                "Customer");
        }

        [Function("StoreCustomerFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestData req)
        {
            var customer =
                await JsonSerializer.DeserializeAsync<CustomerRequest>(
                    req.Body);

            if (customer == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                await badResponse.WriteStringAsync(
                    "Invalid customer information.");

                return badResponse;
            }

            await _customerTable.CreateIfNotExistsAsync();

            string customerId = Guid.NewGuid().ToString();

            var entity = new TableEntity
            {
                PartitionKey = "Customers",
                RowKey = customerId,

                ["Name"] = customer.Name,
                ["Email"] = customer.Email,
                ["Phone"] = customer.Phone,
                ["Address"] = customer.Address
            };

            await _customerTable.AddEntityAsync(entity);

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new
            {
                Message = "Customer stored successfully.",
                CustomerId = customerId
            });

            return response;
        }
    }

    public class CustomerRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}