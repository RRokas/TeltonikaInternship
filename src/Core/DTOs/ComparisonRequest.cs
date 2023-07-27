using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class ComparisonRequest
    {
        [Required] 
        public IFormFile SourceFile { get; set; }
        [Required] 
        public IFormFile TargetFile { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComparisonFilterDto Filter { get; set; }
    }
}