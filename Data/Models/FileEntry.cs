using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class FileEntry
{
    public string Name { get; set; } = null!;
    public Archive Archive { get; } = null!;

    private readonly Lazy<FileInfo> _fileInfo;
    
    [NotMapped]
    public FileInfo FileInfo => _fileInfo.Value;

    private FileEntry()
    {
        // EF
        _fileInfo = new Lazy<FileInfo>(() => new FileInfo(Name));
    }

    public FileEntry(string name)
    {
        Name = name;
        _fileInfo = new Lazy<FileInfo>(() => new FileInfo(Name));
    }

    public FileEntry(FileInfo fileInfo)
    {
        Name = fileInfo.Name;
        _fileInfo = new Lazy<FileInfo>(() => fileInfo);
    }
}