using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.DTOs
{
    public class ConfigurationParameterComparisonDto
    {
        public string? Id { get; init; }
        public string? SourceValue { get; init; }
        public string? TargetValue { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComparisonResult ComparisonResult { get; init; }
    }
}