using System.IO.Compression;
using Core.FileManagement.Interfaces;

namespace Core.FileManagement;

public class Unzipper : IUnzipper
{
    public async Task UnarchiveAsync(string filepath, string storePath)
    {
        if (!File.Exists(filepath))
            throw new FileNotFoundException(filepath);
        if (!Directory.Exists(storePath))
            Directory.CreateDirectory(storePath);

        await Task.Run(() =>
            ZipFile.ExtractToDirectory(
                filepath,
                storePath,
                overwriteFiles: true
            )
        );
    }
}