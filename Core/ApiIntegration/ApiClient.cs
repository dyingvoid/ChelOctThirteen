using System.Text.Json;

namespace Core.ApiIntegration;

public class ApiClient : IApiClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient client)
    {
        _client = client;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<GetLastDownloadFileInfoResponse> GetLastDownloadFileInfo(
        CancellationToken ct = default)
    {
        using var response = await _client.GetAsync(
            "/WebServices/Public/GetLastDownloadFileInfo", ct
        );
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<GetLastDownloadFileInfoResponse>(content, _jsonOptions) ??
               throw new InvalidOperationException("Failed to deserialize api response");
    }
}