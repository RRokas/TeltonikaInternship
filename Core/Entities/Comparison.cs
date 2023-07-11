namespace Core.Entities
{
    public class Comparison
    {
        public ComparisonResult Result { get; set; }
        public ConfigurationParameter Source { get; set; }
        public ConfigurationParameter Target { get; set; }

        public Comparison(ConfigurationParameter source, ConfigurationParameter target)
        {
            Source = source;
            Target = target;
            Result = Compare();
        }
        
        private ComparisonResult Compare()
        {
            if (Source == null && Target == null) return ComparisonResult.Unchanged;
            if (Source == null) return ComparisonResult.Added;
            if (Target == null) return ComparisonResult.Removed;
            if (Source.Value == Target.Value) return ComparisonResult.Unchanged;
            return ComparisonResult.Modified;
        }
    }

    public enum ComparisonResult
    {
        Unchanged,
        Modified,
        Added,
        Removed
    }
}