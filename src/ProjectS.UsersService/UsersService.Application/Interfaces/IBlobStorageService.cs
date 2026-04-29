using Microsoft.AspNetCore.Http;

namespace UsersService.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string fileName);
    Task DeleteFileAsync(string fileName);
    Task UpdateFileAsync(IFormFile file, string fileName);
}
