using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.DTOs
{
    public class ComparisonFilterDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter)), Required]
        public FilterType FilterType { get; init; }
        public string? FilterValue { get; init; }
    }
}