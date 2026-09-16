using Azure.Storage.Files.Shares;
using System.Text;

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

        // INITIALIZE FILE SHARE
        

        public async Task InitializeAsync()
        {
            await _shareClient.CreateIfNotExistsAsync();
        }

        
        // WRITE USER LOG
        

        public async Task WriteUserLogAsync(
            string userId,
            string logMessage)
        {
            await _shareClient.CreateIfNotExistsAsync();

            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            string fileName = $"user-{userId}.log";

            ShareFileClient file =
                directory.GetFileClient(fileName);

            string timestamp =
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            string newLog =
                $"{timestamp} | {logMessage}{Environment.NewLine}";

            string existingContent = string.Empty;

            if (await file.ExistsAsync())
            {
                var download =
                    await file.DownloadAsync();

                using var reader =
                    new StreamReader(
                        download.Value.Content);

                existingContent =
                    await reader.ReadToEndAsync();
            }

            string completeContent =
                existingContent + newLog;

            byte[] content =
                Encoding.UTF8.GetBytes(completeContent);

            if (await file.ExistsAsync())
            {
                await file.DeleteAsync();
            }

            await file.CreateAsync(content.Length);

            using var stream =
                new MemoryStream(content);

            await file.UploadAsync(stream);
        }

        
        // WRITE LOG USING A USERNAME/EMAIL
        

        public async Task WriteLogAsync(
            string fileName,
            string logMessage)
        {
            await _shareClient.CreateIfNotExistsAsync();

            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient file =
                directory.GetFileClient(fileName);

            string timestamp =
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            string newLog =
                $"{timestamp} | {logMessage}{Environment.NewLine}";

            string existingContent = string.Empty;

            if (await file.ExistsAsync())
            {
                var download =
                    await file.DownloadAsync();

                using var reader =
                    new StreamReader(
                        download.Value.Content);

                existingContent =
                    await reader.ReadToEndAsync();
            }

            string completeContent =
                existingContent + newLog;

            byte[] content =
                Encoding.UTF8.GetBytes(completeContent);

            if (await file.ExistsAsync())
            {
                await file.DeleteAsync();
            }

            await file.CreateAsync(content.Length);

            using var stream =
                new MemoryStream(content);

            await file.UploadAsync(stream);
        }

        // READ USER LOG
        

        public async Task<string> ReadUserLogAsync(
            string userId)
        {
            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            string fileName = $"user-{userId}.log";

            ShareFileClient file =
                directory.GetFileClient(fileName);

            if (!await file.ExistsAsync())
            {
                return string.Empty;
            }

            var download =
                await file.DownloadAsync();

            using var reader =
                new StreamReader(
                    download.Value.Content);

            return await reader.ReadToEndAsync();
        }

       
        // READ LOG BY FILE NAME
        

        public async Task<string> ReadLogAsync(
            string fileName)
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
                new StreamReader(
                    download.Value.Content);

            return await reader.ReadToEndAsync();
        }

        
        // DOWNLOAD USER LOG
       

        public async Task<byte[]?> DownloadUserLogAsync(
            string userId)
        {
            ShareDirectoryClient directory =
                _shareClient.GetRootDirectoryClient();

            string fileName = $"user-{userId}.log";

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

            await download.Value.Content.CopyToAsync(
                memoryStream);

            return memoryStream.ToArray();
        }

        
        // DOWNLOAD LOG BY FILE NAME
        

        public async Task<byte[]?> DownloadLogAsync(
            string fileName)
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

            await download.Value.Content.CopyToAsync(
                memoryStream);

            return memoryStream.ToArray();
        }
    }
}