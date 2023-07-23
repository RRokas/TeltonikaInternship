using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.DTOs
{
    public class ComparisonFilterDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FilterType FilterType { get; set; }
        public string? FilterValue { get; set; }
    }
}