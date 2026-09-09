using Azure.Storage.Blobs;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is missing.");
        }

        public async Task<string> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string containerName)
        {
            BlobServiceClient blobServiceClient =
                new BlobServiceClient(_connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient =
                containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(
                fileStream,
                overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(
            string fileName,
            string containerName)
        {
            BlobServiceClient blobServiceClient =
                new BlobServiceClient(_connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient =
                containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
