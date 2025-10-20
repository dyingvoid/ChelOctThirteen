using Core.ApiIntegration;
using Core.FileManagement.Interfaces;
using Core.Parsing;
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

        await ParseToCsv(latestArchive, ct);
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

    /// <summary>
    /// Just as an example
    /// </summary>
    /// <param name="archive"></param>
    /// <param name="ct"></param>
    private async Task ParseToCsv(Archive archive, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(archive.UnzippedFilesPath);
        var steadsPath = FindFirstFileStartingWith(
            Path.Combine(archive.UnzippedFilesPath, "01"),
            "AS_STEADS_",
            "AS_STEADS_PARAMS_"
        );
        ArgumentNullException.ThrowIfNull(steadsPath);

        var steads = XmlParsing.Parse<STEADS>(steadsPath);
        ArgumentNullException.ThrowIfNull(steads);

        var outDir = Path.GetDirectoryName(steadsPath);
        ArgumentNullException.ThrowIfNull(outDir);
        
        var outFileName = Path.Combine(outDir, Path.GetFileNameWithoutExtension(steadsPath) + ".csv");
        await CsvExporter.ToCsv(steads.STEAD, outFileName, ct: ct);
    }

    private static string? FindFirstFileStartingWith(string dirPath, string nameStart, string nameExclude)
    {
        if (!Directory.Exists(dirPath))
            throw new DirectoryNotFoundException($"Directory not found: {dirPath}");

        var dirs = new Stack<string>();
        dirs.Push(dirPath);

        while (dirs.Count > 0)
        {
            var currentDir = dirs.Pop();

            try
            {
                foreach (var file in Directory.GetFiles(currentDir))
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.StartsWith(nameStart, StringComparison.OrdinalIgnoreCase) &&
                        !fileName.StartsWith(nameExclude, StringComparison.OrdinalIgnoreCase))
                        return file;
                }

                foreach (var subDir in Directory.GetDirectories(currentDir))
                {
                    dirs.Push(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return null;
    }
}