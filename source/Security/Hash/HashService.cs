using DotNetCore.Results;

namespace DotNetCore.Security;

public class HashService : IHashService
{
    public string Create(string value, string salt)
    {
        using var key = KeyGenerator.Generate(value, salt);

        return Convert.ToBase64String(key.GetBytes(512));
    }

    public Result Validate(string hash, string value, string salt)
    {
        return hash == Create(value, salt) ? Result.Success() : Result.Error();
    }
}
