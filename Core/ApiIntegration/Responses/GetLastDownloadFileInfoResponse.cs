using System.Text.Json.Serialization;

namespace Core.ApiIntegration;

public record GetLastDownloadFileInfoResponse
{
    public required int VersionId { get; init; }
    public required string TextVersion { get; init; }
    public required string FiasCompleteDbfUrl { get; init; }
    public required string FiasCompleteXmlUrl { get; init; }
    public required string FiasDeltaDbfUrl { get; init; }
    public required string FiasDeltaXmlUrl { get; init; }
    public required string Kladr4ArjUrl { get; init; }
    public required string Kladr47ZUrl { get; init; }
    public required string GarXMLFullURL { get; init; }
    public required string GarXMLDeltaURL { get; init; }
    public required DateTime ExpDate { get; init; }
    
    [JsonConverter(typeof(DateTimeConverter))]
    public required DateTime Date { get; init; }
}