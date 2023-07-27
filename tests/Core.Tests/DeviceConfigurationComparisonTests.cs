using System.Linq;
using Core.Entities;
using Core.Enums;
using Core.Tests.Utilities;
using Xunit;

namespace Core.Tests
{
    public class DeviceConfigurationComparisonTests
    {
        [Fact]
        public void CompareIdenticalConfigs()
        {
            var configFile = TestDataDirectory.GetFile("FMB920-default.cfg");
            var source = new DeviceConfiguration().LoadFromFile(configFile);
            var target = new DeviceConfiguration().LoadFromFile(configFile);
            var comparison = new DeviceConfigurationComparison(source, target);
            var results = comparison.Results.Select(x=>x.Result).ToList();
            Assert.All(results, x => Assert.Equal(ComparisonResult.Unchanged, x));
        }

        [Fact]
        public void DetectAddedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var addedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Added).ToList();
            
            Assert.Single(addedParameterResults);
            Assert.Equal("0", addedParameterResults[0].Target!.Id);
            Assert.Equal("1", addedParameterResults[0].Target.Value);
        }
        
        [Fact]
        public void DetectRemovedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var removedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Removed).ToList();
            
            Assert.Single(removedParameterResults);
            Assert.Equal("0", removedParameterResults[0].Source!.Id);
            Assert.Equal("1", removedParameterResults[0].Source.Value);
        }
        
        [Fact]
        public void DetectModifiedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:2";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var modifiedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Modified).ToList();
            
            Assert.Single(modifiedParameterResults);
            Assert.Equal("0", modifiedParameterResults[0].Source!.Id);
            Assert.Equal("1", modifiedParameterResults[0].Source.Value);
            Assert.Equal("2", modifiedParameterResults[0].Target!.Value);
        }
        
        [Fact]
        public void DetectModifiedTypes()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;0:vienas";
            var source = new DeviceConfiguration().LoadFromString(sourceConfigString);
            var target = new DeviceConfiguration().LoadFromString(targetConfigString);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            var modifiedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Modified).ToList();
            
            Assert.Single(modifiedParameterResults);
            Assert.Equal("0", modifiedParameterResults[0].Source!.Id);
            Assert.Equal("1", modifiedParameterResults[0].Source.Value);
            Assert.Equal("vienas", modifiedParameterResults[0].Target!.Value);
        }
        
        [Fact]
        public void ThrowArgumentExceptionWhenSourceOrTargetIsNull()
        {
            var config = new DeviceConfiguration().LoadFromString("Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:1");
            Assert.Throws<System.ArgumentNullException>(() => new DeviceConfigurationComparison(null!, config));
            Assert.Throws<System.ArgumentNullException>(() => new DeviceConfigurationComparison(config, null!));
        }

        [Fact]
        public void ThrowArgumentExceptionWhenParameterIdsDontMatch()
        {
            var sourceParameter = new ConfigurationParameter("SomeId", "SomeValue");
            var targetParameter = new ConfigurationParameter("SomeOtherId", "SomeValue");
            
            Assert.Throws<System.ArgumentException>(() => 
                new ConfigurationParameterComparison(sourceParameter, targetParameter)
            );
        }

        [Fact]
        public void ReturnEmptyComparisonListIfNoParametersInConfig()
        {
            var config = new DeviceConfiguration().LoadFromString("");
            var comparison = new DeviceConfigurationComparison(config, config);
            Assert.Empty(comparison.Results);
        }
        
                [Fact]
        public void FilterByResultEnumWorks()
        {
            var sourceConfig = new DeviceConfiguration().LoadFromString("1:20;2:30");
            var targetConfig = new DeviceConfiguration().LoadFromString("1:20;2:40");
            var comparison = new DeviceConfigurationComparison(sourceConfig, targetConfig);
            
            comparison.ApplyResultFilter(ComparisonResult.Modified.ToString());
            
            Assert.Single(comparison.Results);
            Assert.Equal("2", comparison.Results[0].Source.Id);
            Assert.Equal("30", comparison.Results[0].Source.Value);
            Assert.Equal("2", comparison.Results[0].Target.Id);
            Assert.Equal("40", comparison.Results[0].Target.Value);
            
            Assert.True(comparison.ResultsFiltered);
        }
        
        [Fact]
        public void FilterByIdStartWorks()
        {
            var sourceConfig = new DeviceConfiguration().LoadFromString("10:20;22:30;23:20");
            var targetConfig = new DeviceConfiguration().LoadFromString("10:20;22:40;23:21");
            var comparison = new DeviceConfigurationComparison(sourceConfig, targetConfig);
            
            comparison.ApplyIdStartFilter("2");
            
            Assert.Equal(2, comparison.Results.Count);
            Assert.Equal("22", comparison.Results[0].Source.Id);
            Assert.Equal("30", comparison.Results[0].Source.Value);
            Assert.Equal("22", comparison.Results[0].Target.Id);
            Assert.Equal("40", comparison.Results[0].Target.Value);
            Assert.Equal("23", comparison.Results[1].Source.Id);
            Assert.Equal("20", comparison.Results[1].Source.Value);
            Assert.Equal("23", comparison.Results[1].Target.Id);
            Assert.Equal("21", comparison.Results[1].Target.Value);
            
            Assert.True(comparison.ResultsFiltered);
        }
        
        [Fact]
        public void FilterCanReturnEmptyResults()
        {
            var sourceConfig = new DeviceConfiguration().LoadFromString("10:20;22:30;23:20");
            var targetConfig = new DeviceConfiguration().LoadFromString("10:20;22:40;23:21");
            var comparison = new DeviceConfigurationComparison(sourceConfig, targetConfig);
            
            comparison.ApplyIdStartFilter("3");
            
            Assert.Empty(comparison.Results);
            Assert.True(comparison.ResultsFiltered);
            Assert.True(comparison.ResultsFiltered);
        }
    }
}