using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WorkshopRSVP.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }

    public class BlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private BlobContainerClient? _containerClient;
        private bool _containerCreated = false;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"] ?? "";
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "event-banners";
        }

        private async Task<BlobContainerClient> GetContainerClientAsync()
        {
            if (_containerClient == null)
                _containerClient = new BlobContainerClient(_connectionString, _containerName);

            if (!_containerCreated)
            {
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                _containerCreated = true;
            }

            return _containerClient;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var container = await GetContainerClientAsync();

            // generate unique file name so there are no conflicts
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = container.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return blobClient.Uri.ToString();
        }
    }
}
