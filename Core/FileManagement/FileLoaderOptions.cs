namespace Core.FileManagement;

public record FileLoaderOptions
{
    public required string StoragePath { get; init; }
}