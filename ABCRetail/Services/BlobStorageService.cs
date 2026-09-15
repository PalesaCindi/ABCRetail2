using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            _container = new BlobContainerClient(
                connectionString,
                "product-images");
        }

        public async Task InitializeAsync()
        {
            await _container.CreateIfNotExistsAsync(
                PublicAccessType.Blob);
        }

        public async Task<string> UploadImageAsync(
            IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException(
                    "Please select an image.");
            }

            string extension =
                Path.GetExtension(image.FileName);

            string fileName =
                $"{Guid.NewGuid()}{extension}";

            BlobClient blob =
                _container.GetBlobClient(fileName);

            using Stream stream =
                image.OpenReadStream();

            await blob.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                });

            return blob.Uri.ToString();
        }
        public async Task<(byte[] Content, string ContentType)?> GetImageAsync(
    string imageUrl)
{
    if (string.IsNullOrEmpty(imageUrl))
    {
        return null;
    }

    Uri uri = new Uri(imageUrl);

    string blobName =
        uri.AbsolutePath
           .Substring(uri.AbsolutePath.IndexOf(
               "/product-images/") + "/product-images/".Length);

    BlobClient blob =
        _container.GetBlobClient(blobName);

    if (!await blob.ExistsAsync())
    {
        return null;
    }

    var download =
        await blob.DownloadAsync();

    using var memoryStream =
        new MemoryStream();

    await download.Value.Content.CopyToAsync(memoryStream);

    return (
        memoryStream.ToArray(),
        download.Value.ContentType
    );
}
    }
}