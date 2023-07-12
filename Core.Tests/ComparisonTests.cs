using System;
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
    }
}