using System.IO;
using System.Linq;
using Core.Entities;
using Xunit;

namespace Core.Tests
{
    public class ComparisonTests
    {
        [Fact]
        public void CompareIdenticalConfigs()
        {
            var configFile = new FileInfo("../../../../Core.Tests/test_data/FMB920-default.cfg");
            var source = new DeviceConfiguration(configFile);
            var target = new DeviceConfiguration(configFile);
            var comparison = new Comparison(source, target);
            var results = comparison.Results.Select(x=>x.Result).ToList();
            Assert.All(results, x => Assert.Equal(ComparisonResult.Unchanged, x));
        }

        [Fact]
        public void DetectAddedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;AddedParameter:1";
            var source = new DeviceConfiguration(sourceConfigString);
            var target = new DeviceConfiguration(targetConfigString);
            
            var comparison = new Comparison(source, target);
            var addedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Added).ToList();
            
            Assert.Single(addedParameterResults);
            Assert.Equal("AddedParameter", addedParameterResults[0].Target.Id.ToString());
            Assert.Equal("1", addedParameterResults[0].Target.Value.ToString());
        }
        
        [Fact]
        public void DetectRemovedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;RemovedParameter:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2";
            var source = new DeviceConfiguration(sourceConfigString);
            var target = new DeviceConfiguration(targetConfigString);
            
            var comparison = new Comparison(source, target);
            var removedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Removed).ToList();
            
            Assert.Single(removedParameterResults);
            Assert.Equal("RemovedParameter", removedParameterResults[0].Source.Id.ToString());
            Assert.Equal("1", removedParameterResults[0].Source.Value.ToString());
        }
        
        [Fact]
        public void DetectModifiedParameters()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:2";
            var source = new DeviceConfiguration(sourceConfigString);
            var target = new DeviceConfiguration(targetConfigString);
            
            var comparison = new Comparison(source, target);
            var modifiedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Modified).ToList();
            
            Assert.Single(modifiedParameterResults);
            Assert.Equal("ModifiedParameter", modifiedParameterResults[0].Source.Id.ToString());
            Assert.Equal("1", modifiedParameterResults[0].Source.Value.ToString());
            Assert.Equal("2", modifiedParameterResults[0].Target.Value.ToString());
        }
        
        [Fact]
        public void DetectModifiedTypes()
        {
            var sourceConfigString = "Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:1";
            var targetConfigString = "Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:vienas";
            var source = new DeviceConfiguration(sourceConfigString);
            var target = new DeviceConfiguration(targetConfigString);
            
            var comparison = new Comparison(source, target);
            var modifiedParameterResults = comparison.Results
                .Where(x=>x.Result == ComparisonResult.Modified).ToList();
            
            Assert.Single(modifiedParameterResults);
            Assert.Equal("ModifiedParameter", modifiedParameterResults[0].Source.Id);
            Assert.Equal(1, modifiedParameterResults[0].Source.Value);
            Assert.Equal("vienas", modifiedParameterResults[0].Target.Value);
        }
        
        [Fact]
        public void ThrowArgumentExceptionWhenSourceOrTargetIsNull()
        {
            var config = new DeviceConfiguration("Name:SomeName;Version:1;Multiplier:1.2;ModifiedParameter:1");
            Assert.Throws<System.ArgumentException>(() => new Comparison(null, config));
            Assert.Throws<System.ArgumentException>(() => new Comparison(config, null));
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
            var config = new DeviceConfiguration("");
            var comparison = new Comparison(config, config);
            Assert.Empty(comparison.Results);
        }
    }
}