namespace Core.FluentValidations.Configuration
{
    public class ComparisonRequestSettings
    {
        public long MaxSize { get; init; }

        public string[] AllowedExtensions { get; init; }
    }
}