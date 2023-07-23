using System.Text.Json.Serialization;

namespace Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComparisonResult
    {
        Unchanged,
        Modified,
        Added,
        Removed
    }
}