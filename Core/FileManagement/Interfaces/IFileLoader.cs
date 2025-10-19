namespace Core.FileManagement.Interfaces;

public interface IFileLoader
{
    public Task<FileInfo> DownloadFileAsync(
        string uriStr, string? outFileName = null, bool overwrite = false, CancellationToken ct = default);
}