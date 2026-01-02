using Microsoft.AspNetCore.Identity;

namespace UsersService.Application.Services;

public static class PasswordService
{
    private static readonly PasswordHasher<object> _hasher = new();

    public static string Hash(this string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public static bool Verify(string hash, string password)
    {
        var result = _hasher.VerifyHashedPassword(null!, hash, password);
        return result == PasswordVerificationResult.Success;
    }
}
