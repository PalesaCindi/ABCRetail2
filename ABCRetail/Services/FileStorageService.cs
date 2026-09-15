using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetail.Services
{
    public class FileStorageService
    {
        private readonly ShareClient _shareClient;

        public FileStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            _shareClient = new ShareClient(
                connectionString,
                "application-logs");
        }

        // Create the file share if it does not exist
        public async Task InitializeAsync()
        {
            await _shareClient.CreateIfNotExistsAsync();
        }

        // Write a log entry to Azure Files
        public async Task WriteLogAsync(string fileName, string logMessage)
        {
            await _shareClient.CreateIfNotExistsAsync();

            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient file =
                directory.GetFileClient(fileName);

            string content = logMessage + Environment.NewLine;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(content));

            await file.CreateAsync(stream.Length);

            await file.UploadAsync(stream);
        }

        // Read a log file from Azure Files
        public async Task<string> ReadLogAsync(string fileName)
        {
            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient file =
                directory.GetFileClient(fileName);

            if (!await file.ExistsAsync())
            {
                return string.Empty;
            }

            var download =
                await file.DownloadAsync();

            using var reader =
                new StreamReader(download.Value.Content);

            return await reader.ReadToEndAsync();
        }
        // Download a log file from Azure Files
        public async Task<byte[]?> DownloadLogAsync(string fileName)
        {
            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient file =
                directory.GetFileClient(fileName);

            if (!await file.ExistsAsync())
            {
                return null;
            }

            var download =
                await file.DownloadAsync();

            using var memoryStream =
                new MemoryStream();

            await download.Value.Content.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }
    }
}