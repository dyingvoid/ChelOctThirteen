using Core.FileManagement.Interfaces;
using Microsoft.Extensions.Options;

namespace Core.FileManagement;

public class FileLoader : IFileLoader, IDisposable
{
    private readonly FileLoaderOptions _options;
    private readonly DirectoryInfo _storePath;
    private readonly HttpClient _httpClient;

    private bool _disposed;

    public FileLoader(IOptions<FileLoaderOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        _storePath = new DirectoryInfo(_options.StoragePath);
        if(!_storePath.Exists)
            throw new DirectoryNotFoundException($"The directory '{_options.StoragePath}' does not exist.");
        
        _httpClient = httpClient;
    }

    public async Task<FileInfo> DownloadFileAsync(
        string uriStr, string? outFileName = null, bool overwrite = false, CancellationToken ct = default)
    {
        outFileName ??= Path.GetRandomFileName() + ".zip";
        var outFilePath = Path.Combine(_storePath.FullName, outFileName);

        if (File.Exists(outFilePath) && !overwrite)
            return new FileInfo(outFilePath);

        await CoreDownloadFileAsync(uriStr, outFilePath, ct);
        return new FileInfo(outFilePath);
    }

    private async Task CoreDownloadFileAsync(string uriStr, string filePath, CancellationToken ct)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        try
        {
            using var response = await _httpClient.GetAsync(
                uriStr,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync(ct);
            await using var fileStream = new FileStream(
                filePath,
                FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 8192, useAsync: true
            );

            await contentStream.CopyToAsync(fileStream, ct);
            await fileStream.FlushAsync(ct);
        }
        catch
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~FileLoader()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }
}