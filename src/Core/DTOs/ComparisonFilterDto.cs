using System.Text.Json.Serialization;

namespace Core.DTOs
{
    public class ComparisonFilterDto
    {
        public FilterType FilterType { get; set; }
        public string FilterValue { get; set; }
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FilterType
    {
        ComparisonResult,
        ParameterIdStartsWith
    }
}