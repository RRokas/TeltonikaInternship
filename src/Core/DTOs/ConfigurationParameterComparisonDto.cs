using System.Text.Json.Serialization;
using Core.Entities;

namespace Core.DTOs
{
    public class ConfigurationParameterComparisonDto
    {
        public string Id { get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComparisonResult Result { get; set; }
    }
}