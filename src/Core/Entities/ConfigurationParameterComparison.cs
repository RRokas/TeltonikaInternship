using System;

namespace Core.Entities
{
    public class ConfigurationParameterComparison
    {
        public ConfigurationParameter? Source { get; set; }
        public ConfigurationParameter? Target { get; set; }
        public ComparisonResult Result { get; set; }

        public ConfigurationParameterComparison(ConfigurationParameter? source, ConfigurationParameter? target)
        {
            Source = source;
            Target = target;
            Result = Compare();
        }

        private ComparisonResult Compare()
        {
            if (Source == null && Target != null) return ComparisonResult.Added;
            if (Source != null && Target == null) return ComparisonResult.Removed;
            if (!Source!.Id.Equals(Target!.Id)) throw new ArgumentException("Source and Target Ids are not equal");
            if (Source.Value.Equals(Target.Value)) return ComparisonResult.Unchanged;
            return ComparisonResult.Modified;
        }
    }
}