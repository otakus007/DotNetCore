using DotNetCore.Results;

namespace DotNetCore.Security;

public interface IHashService
{
    string Create(string value, string salt);

    Result Validate(string hash, string value, string salt);
}
