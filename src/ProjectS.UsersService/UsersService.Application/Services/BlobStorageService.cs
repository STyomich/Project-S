using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UsersService.Application.Interfaces;

namespace UsersService.Application.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        var connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING") ?? configuration["AzureBlobStorage:ConnectionString"];
        var containerName = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONTAINER_NAME") ?? configuration["AzureBlobStorage:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        var response = await blobClient.UploadAsync(stream, overwrite: true);

        if (response.GetRawResponse().Status >= 200 && response.GetRawResponse().Status < 300)
        {
            _logger.LogInformation("File {FileName} uploaded successfully.", fileName);
        }
        else
        {
            _logger.LogError("Failed to upload file {FileName}. Status: {StatusCode}", fileName, response.GetRawResponse().Status);
            throw new Exception($"Failed to upload file. Status code: {response.GetRawResponse().Status}");
        }

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var response = await blobClient.DeleteIfExistsAsync();

        if (response.Value)
        {
            _logger.LogInformation("File {FileName} deleted successfully.", fileName);
        }
        else
        {
            _logger.LogWarning("File {FileName} not found for deletion.", fileName);
        }
    }

    public async Task UpdateFileAsync(IFormFile file, string fileName)
    {
        await UploadFileAsync(file, fileName);
    }
}
