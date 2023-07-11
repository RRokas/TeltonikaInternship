namespace Core.Entities
{
    public class Comparison
    {
        public ComparisonResult Result { get; set; }
        public ConfigurationParameter Source { get; set; }
        public ConfigurationParameter Target { get; set; }
    }
    
    public enum ComparisonResult
    {
        Unchanged,
        Modified,
        Added,
        Removed
    }
}