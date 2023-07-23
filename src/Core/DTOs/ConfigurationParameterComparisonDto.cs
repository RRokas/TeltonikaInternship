using System.Text.Json.Serialization;
using Core.Entities;
using Core.Enums;

namespace Core.DTOs
{
    public class ConfigurationParameterComparisonDto
    {
        public string Id { get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComparisonResult ComparisonResult { get; set; }
    }
}