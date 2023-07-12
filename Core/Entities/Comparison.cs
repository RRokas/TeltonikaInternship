using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class Comparison
    {
        public List<ConfigurationParameterComparison> Results { get; set; }
        public DeviceConfiguration Source { get; set; }
        public DeviceConfiguration Target { get; set; }

        public Comparison(DeviceConfiguration source, DeviceConfiguration target)
        {
            Source = source;
            Target = target;
            Results = Compare();
        }
        
        private List<ConfigurationParameterComparison> Compare()
        {
            var result = new List<ConfigurationParameterComparison>();
            if(Source == null || Target == null) throw new ArgumentException("Source or Target is null");
            if (Source.Parameters.Count == 0 && Target.Parameters.Count == 0) return result;

            foreach (var sourceParameter in Source.Parameters)
            {
                var targetParameter = Target.Parameters.Find(x => x.Id.ToString() == sourceParameter.Id.ToString());
                result.Add(new ConfigurationParameterComparison(sourceParameter, targetParameter));
            }
            
            foreach (var targetParameter in Target.Parameters)
            {
                var sourceParameter = Source.Parameters.Find(x => x.Id.ToString() == targetParameter.Id.ToString());
                if (sourceParameter == null) result.Add(new ConfigurationParameterComparison(null, targetParameter));
            }
            
            return result;
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