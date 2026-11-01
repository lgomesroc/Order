using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Order.Services;

public sealed class OrderApiClient(HttpClient httpClient)
{
    public async Task<T?> GetAsync<T>(string path, string token)
    {
        using var request = CreateRequest(HttpMethod.Get, path, token);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string path, object body, string token)
    {
        using var request = CreateRequest(HttpMethod.Post, path, token);
        request.Content = JsonContent.Create(body);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccess(response);
        if (response.Content.Headers.ContentLength == 0) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PostAsync(string path, object body, string token)
    {
        using var request = CreateRequest(HttpMethod.Post, path, token);
        request.Content = JsonContent.Create(body);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccess(response);
    }

    public async Task PutAsync(string path, object body, string token)
    {
        using var request = CreateRequest(HttpMethod.Put, path, token);
        request.Content = JsonContent.Create(body);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccess(response);
    }

    public async Task DeleteAsync(string path, string token)
    {
        using var request = CreateRequest(HttpMethod.Delete, path, token);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccess(response);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string path, string token)
    {
        var request = new HttpRequestMessage(method, path);
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"API returned {(int)response.StatusCode} ({response.ReasonPhrase}): {body}");
    }
}

public sealed class ApiEnvelope<T>
{
    public T? Data { get; set; }
    public List<ApiReport>? Report { get; set; }
}
public sealed class ApiReport { public string? Message { get; set; } }
public sealed class ApiAuthResponse { public string? Token { get; set; } public string? Type { get; set; } public int ExpireIn { get; set; } }
public sealed class ProductViewModel { public string Id { get; set; } = ""; public string Description { get; set; } = ""; public decimal SellValue { get; set; } public int Stock { get; set; } }
