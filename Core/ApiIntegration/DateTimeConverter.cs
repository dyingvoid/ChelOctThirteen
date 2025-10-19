using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.ApiIntegration;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format = "dd.MM.yyyy";
    
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateTime.TryParseExact(value, _format, null, DateTimeStyles.None, out var datetime))
        {
            return datetime;
        }
        
        throw new JsonException($"Unable to parse {value}, expected format {_format}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}