using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enums;

namespace Core.Entities
{
    public class DeviceConfigurationComparison
    {
        public List<ConfigurationParameterComparison> Results { get; private set;}
        public bool ResultsFiltered { get; set; }
        public DeviceConfiguration Source { get; init; }
        public DeviceConfiguration Target { get; init; }

        public DeviceConfigurationComparison(DeviceConfiguration source, DeviceConfiguration target)
        {
            Source = source;
            Target = target;
            Results = Compare();
        }
        
        private List<ConfigurationParameterComparison> Compare()
        {
            _ = Source ?? throw new ArgumentNullException(nameof(Source));
            _ = Target ?? throw new ArgumentNullException(nameof(Target));

            if (Source.Parameters.Count == 0 && Target.Parameters.Count == 0) 
                return new List<ConfigurationParameterComparison>();

            var result = new List<ConfigurationParameterComparison>();
            foreach (var sourceParameter in Source.Parameters)
            {
                var targetParameter = Target.Parameters.Find(x => x.Id == sourceParameter.Id);
                result.Add(new ConfigurationParameterComparison(sourceParameter, targetParameter));
            }
            
            foreach (var targetParameter in Target.Parameters)
            {
                var sourceParameter = Source.Parameters.Find(x => x.Id.ToString() == targetParameter.Id);
                if (sourceParameter == null) result.Add(new ConfigurationParameterComparison(null, targetParameter));
            }
            
            return result;
        }
        
        public void ApplyResultFilter(string resultToFilter)
        {
            ResultsFiltered = true;
            Results = Results.Where(x => x.Result.ToString() == resultToFilter).ToList();
        }
        
        public void ApplyIdStartFilter(string subStringToSearchFor)
        {
            ResultsFiltered = true;
            Results = Results.Where(x => x.Source?.Id.ToString().StartsWith(subStringToSearchFor) ?? false).ToList();
        }
    
    }
}