﻿using System.IO;
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
    }
}