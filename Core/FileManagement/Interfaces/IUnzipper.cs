namespace Core.FileManagement.Interfaces;

public interface IUnzipper
{
    Task UnarchiveAsync(string filepath, string storePath);
}