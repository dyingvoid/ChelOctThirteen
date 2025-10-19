using Core.ApiIntegration;
using Core.FileManagement.Interfaces;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.FileManagement;

public class FileManager
{
    private readonly AppDbContext _context;
    private readonly IApiClient _client;
    private readonly IFileLoader _fileLoader;
    private readonly IUnzipper _unzipper;

    public FileManager(
        AppDbContext context,
        IApiClient client,
        IFileLoader fileLoader,
        IUnzipper unzipper)
    {
        _context = context;
        _client = client;
        _fileLoader = fileLoader;
        _unzipper = unzipper;
    }

    public async Task LoadLatest(CancellationToken ct = default)
    {
        var latestArchive = await _context.Archives
            .OrderByDescending(a => a.VersionId)
            .FirstOrDefaultAsync(ct);
        var res = await _client.GetLastDownloadFileInfo(ct);

        if (latestArchive == null || latestArchive.VersionId < res.VersionId)
            latestArchive = await DownloadArchive(res, ct);

        if (latestArchive.UnarchivedAt == null)
            await UnarchiveAsync(ct, latestArchive);
    }

    private async Task UnarchiveAsync(CancellationToken ct, Archive latestArchive)
    {
        var archiveDir = Path.GetDirectoryName(latestArchive.FilePath);
        var archiveName = Path.GetFileNameWithoutExtension(latestArchive.FilePath);
        ArgumentNullException.ThrowIfNull(archiveDir);
        ArgumentNullException.ThrowIfNull(archiveName);

        var unzippedDir = Path.Combine(archiveDir, $"{archiveName}_unzipped");
        Directory.CreateDirectory(unzippedDir);
        await _unzipper.UnarchiveAsync(latestArchive.FilePath, unzippedDir);
        
        latestArchive.UnzippedFilesPath = unzippedDir;
        latestArchive.UnarchivedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    private async Task<Archive> DownloadArchive(GetLastDownloadFileInfoResponse res, CancellationToken ct)
    {
        var file = await _fileLoader.DownloadFileAsync(
            uriStr: res.GarXMLDeltaURL,
            outFileName: "delta_" + res.VersionId + ".zip",
            overwrite: false,
            ct: ct
        );

        if (!file.Exists)
            throw new FileNotFoundException($"file downloaded, but not found {res.GarXMLDeltaURL}");

        var latestArchive = new Archive(
            res.VersionId, res.GarXMLDeltaURL,
            DateOnly.FromDateTime(res.ExpDate), 
            file.FullName, DateTime.UtcNow
        );
        _context.Archives.Add(latestArchive);
        await _context.SaveChangesAsync(ct);

        return latestArchive;
    }
}