using DotNetCore.Results;

namespace DotNetCore.Services;

public interface IHttpService
{
    Task<Result> DeleteAsync(string uri);

    Task<Result<T>> GetAsync<T>(string uri);

    Task<Result> PostAsync(string uri, object request);

    Task<Result<TResponse>> PostAsync<TResponse>(string uri, object request);

    Task<Result> PutAsync(string uri, object request);
}
