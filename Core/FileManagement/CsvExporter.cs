using System.Globalization;
using CsvHelper;

namespace Core.FileManagement;

public static class CsvExporter
{
    public static async Task ToCsv<T>(IEnumerable<T> data, string filePath, string delimiter = ",",
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        await using var writer = new StreamWriter(filePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(data, ct);
        await writer.FlushAsync(ct);
    }
}