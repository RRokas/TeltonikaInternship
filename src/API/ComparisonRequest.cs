using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.DTOs;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API
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