namespace Data.Models;

public class Archive
{
    public int VersionId { get; set; }
    public string ArchiveLink { get; set; } = null!;
    public DateOnly ExpDate { get; set; }
    public string FilePath { get; set; } = null!;
    public DateTime DownloadedAt { get; set; }
    public string? UnzippedFilesPath { get; set; }
    public DateTime? UnarchivedAt { get; set; }

    private Archive()
    {
        // EF
    }

    public Archive(
        int versionId, string archiveLink,
        DateOnly expDate, string filePath,
        DateTime downloadedAt)
    {
        VersionId = versionId;
        ArchiveLink = archiveLink;
        ExpDate = expDate;
        FilePath = filePath;
        DownloadedAt = downloadedAt;
    }
}