using DotNetCore.Results;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

namespace DotNetCore.Services;

public abstract record HttpService : IHttpService
{
    private readonly HttpClient _client;

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public HttpService(HttpOptions options)
    {
        _retryPolicy = Policy.Handle<HttpRequestException>().OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode).WaitAndRetryAsync(options.RetryCount, retryAttempt => TimeSpan.FromSeconds(options.RetrySeconds));

        _client = new HttpClient { Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds) };

        _client.DefaultRequestHeaders.Clear();

        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(nameof(HttpService), string.Empty));

        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        if (options.BaseAddress is not null) _client.BaseAddress = new Uri(options.BaseAddress);

        if (options.Authentication is not null) _client.DefaultRequestHeaders.Authorization = options.Authentication;
    }

    public async Task<Result> DeleteAsync(string uri) => Response(await _retryPolicy.ExecuteAsync(() => _client.DeleteAsync(uri)));

    public async Task<Result<T>> GetAsync<T>(string uri) => await Response<T>(await _retryPolicy.ExecuteAsync(() => _client.GetAsync(uri)));

    public async Task<Result> PostAsync(string uri, object request) => Response(await _retryPolicy.ExecuteAsync(() => _client.PostAsJsonAsync(uri, request)));

    public async Task<Result<T>> PostAsync<T>(string uri, object request) => await Response<T>(await _retryPolicy.ExecuteAsync(() => _client.PostAsJsonAsync(uri, request)));

    public async Task<Result> PutAsync(string uri, object request) => Response(await _retryPolicy.ExecuteAsync(() => _client.PutAsJsonAsync(uri, request)));

    private static async Task<Result<T>> Response<T>(HttpResponseMessage response) => response.IsSuccessStatusCode ? Result<T>.Success(await response.Content.ReadFromJsonAsync<T>()) : Result<T>.Error(response.StatusCode.ToString());

    private static Result Response(HttpResponseMessage response) => response.IsSuccessStatusCode ? Result.Success() : Result.Error(response.StatusCode.ToString());
}
