namespace Core.ApiIntegration;

public interface IApiClient
{
    Task<GetLastDownloadFileInfoResponse> GetLastDownloadFileInfo(CancellationToken ct = default);
}